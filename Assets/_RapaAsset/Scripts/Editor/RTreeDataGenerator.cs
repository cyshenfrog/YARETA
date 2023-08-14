using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Sisus.HierarchyFolders;
using UnityEngine.SceneManagement;

public class RTreeDataGenerator : EditorWindow
{
    [MenuItem("Examples/Duplicate Prefab")]
    private static void DuplicatePrefab()
    {
        PropertyModification[] m = PrefabUtility.GetPropertyModifications(Selection.activeGameObject);
        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(Selection.activeGameObject);
        PrefabUtility.InstantiatePrefab(prefab);
        //PrefabUtility.SetPropertyModifications(PrefabUtility.InstantiatePrefab(prefab), m);
    }

    // Disable the menu item if no selection is in place.
    [MenuItem("Examples/Create Prefab", true)]
    private static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
    }

    private const string PrefabPath = "Assets/Resources/RTreePrefab";
    private static List<string> PrefabNames = new List<string>();

    [SerializeField]
    public static List<RTreeData> data = new List<RTreeData>();

    [MenuItem("YARETA/FlattenFolder")]
    private static void FlattenFolder()
    {
        foreach (var item in FindObjectsOfType<HierarchyFolder>())
        {
            HierarchyFolderUtility.FlattenAndDestroyHierarchyFolder(item);
        }
    }

    [MenuItem("YARETA/RandomCube")]
    private static void RandomCube()
    {
        GameObject parent = FindObjectsOfType<HierarchyFolder>()[0].gameObject;
        for (int i = 0; i < 20; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.parent = parent.transform;
            go.name = "X";
            go.transform.position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
            go.transform.localScale = new Vector3(10, 10, 10);
            go.AddComponent<StreamingObject>();
        }
        parent = FindObjectsOfType<HierarchyFolder>()[1].gameObject;
        for (int i = 0; i < 100; i++)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Y.prefab")) as GameObject;
            go.transform.parent = parent.transform;
            go.transform.position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        }
        parent = FindObjectsOfType<HierarchyFolder>()[2].gameObject;
        for (int i = 0; i < 100; i++)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Z.prefab")) as GameObject;
            go.transform.parent = parent.transform;
            go.transform.position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        }
    }

    [MenuItem("YARETA/Build RTree Data")]
    private static void BuildRTreeData()
    {
        PrefabNames.Clear();
        data.Clear();

        FlattenFolder();
        //check all root gameobject in active scene is a prefab or not
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        //check if there is any gameobject without StreamingObject component
        List<string> errorHandler = new List<string>();
        foreach (var item in rootGameObjects)
        {
            if (!item.GetComponent<StreamingObject>())
            {
                errorHandler.Add(item.name);
            }
        }
        if (errorHandler.Count > 0)
        {
            Debug.LogError("There are some gameobject without StreamingObject component");
            foreach (var item in errorHandler)
            {
                Debug.LogError(item);
            }
            return;
        }
        //get current scene name
        string sceneName = SceneManager.GetActiveScene().name;

        if (Directory.Exists("Assets/Prefabs/" + sceneName))
        {
            Directory.Delete("Assets/Prefabs/" + sceneName, true);
        }
        //check if there is any gameobject with StreamingObject component but not a prefab
        foreach (var item in rootGameObjects)
        {
            if (PrefabUtility.GetPrefabAssetType(item) != PrefabAssetType.MissingAsset &&
                PrefabUtility.GetPrefabAssetType(item) != PrefabAssetType.NotAPrefab)
            {
                if (!PrefabUtility.HasPrefabInstanceAnyOverrides(item, false))
                {
                    string[] s = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(item).Split("/");
                    string prefabName = s[s.Length - 1];
                    prefabName = prefabName.Split(".")[0];
                    Debug.Log(prefabName);
                    if (!PrefabNames.Contains(prefabName))
                    {
                        PrefabUtility.UnpackPrefabInstance(item, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                        CreatePrefabAndCacheRTreeData(item.GetComponent<StreamingObject>(), prefabName, sceneName);
                    }
                    else
                    {
                        data.Add(item.GetComponent<StreamingObject>().GetRTreeData("Assets/Prefabs/" + sceneName + "/" + s[s.Length - 1]));
                    }
                    continue;
                }
            }
            CreatePrefabAndCacheRTreeData(item.GetComponent<StreamingObject>(), item.name, sceneName);
        }
        SaveData(data);

        return;
        //List<RTreeData> data = new List<RTreeData>();
        //foreach (var item in FindAllStaticObjInScene())
        //{
        //    data.Add(item.GetRTreeData());
        //}
        //WriteDataToJson(data.ToArray());
    }

    private static void CreatePrefabAndCacheRTreeData(StreamingObject targetObj, string prefabName, string subfolderName)
    {
        // Create folder Prefabs and set the path as within the Prefabs folder,
        // and name it as the GameObject's name with the .Prefab format
        if (!Directory.Exists("Assets/Prefabs/" + subfolderName))
        {
            Directory.CreateDirectory("Assets/Prefabs/" + subfolderName);
        }

        string localPath = "Assets/Prefabs/" + subfolderName + "/" + prefabName + ".prefab";//.Split(" (")[0] + ".prefab";

        // Make sure the file name is unique, in case an existing Prefab has the same name.
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        // Create the new Prefab and log whether Prefab was saved successfully.
        bool prefabSuccess;
        //PrefabUtility.SaveAsPrefabAsset(gameObject, localPath, out prefabSuccess);
        PrefabUtility.SaveAsPrefabAssetAndConnect(targetObj.gameObject, localPath, InteractionMode.UserAction, out prefabSuccess);
        if (!prefabSuccess)
            Debug.LogError("Prefab failed to save:" + localPath);
        else
            PrefabNames.Add(prefabName);
        data.Add(targetObj.GetRTreeData(localPath));
    }

    private static void SaveData(List<RTreeData> data)
    {
        ES3.Save("RTreeData", data, Application.dataPath + "/Resources/RTreeData.bytes");
        AssetDatabase.Refresh();
        // Create an ES3Settings object to set the storage location to Resources.
        var settings = new ES3Settings();
        settings.location = ES3.Location.Resources;
        List<RTreeData> d = ES3.Load<List<RTreeData>>("RTreeData", "RTreeData.bytes", settings);
        foreach (RTreeData d2 in d)
        {
            Debug.Log(d2.PrefabPath + "|==|" + d2.Position + "|==|" + d2.Rotation);
        }
        //var json = JsonUtility.ToJson(data[0]);
        //Debug.Log(json);
        //File.WriteAllText("Assets/Resources/RTreeData.json", json);
    }

    private static StreamingObject[] FindAllStaticObjInScene()
    {
        var staticObjs = FindObjectsOfType<StreamingObject>();
        return staticObjs;
    }

    private void GeneratePrefat(GameObject go)
    {
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabPath + go.name);
        AssetDatabase.Refresh();
    }
}