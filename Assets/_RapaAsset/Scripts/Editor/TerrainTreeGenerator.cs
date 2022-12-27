using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TerrainTreeGenerator : EditorWindow
{
    private float heightFudge;
    private float scaleMax = 1.5f;

    private float scaleMin = .9f;

    private bool sortingByPrefab;
    private bool[] prefabToggles = new bool[0];

    private Terrain _terrain;

    public Terrain terrain
    {
        get { return _terrain; }
        set
        {
            if (_terrain != value)
                prefabToggles = new bool[value.terrainData.treePrototypes.Length];
            _terrain = value;
        }
    }

    private int treeDivisions = 5;

    private void OnGUI()
    {
        // The actual window code goes here
        GUILayout.Label("Replace Trees with Prefabs", EditorStyles.boldLabel);

        terrain = EditorGUILayout.ObjectField("Terrain:", terrain, typeof(Terrain), true) as Terrain;

        GUILayout.Label("Convert By Prefab or Grid(will convert all detail)");
        sortingByPrefab = GUILayout.Toggle(sortingByPrefab, "By Prefab");
        GUILayout.Space(10f);
        if (!sortingByPrefab)
        {
            GUILayout.Label("Tree groups " + treeDivisions);
            treeDivisions = (int)GUILayout.HorizontalSlider(treeDivisions, 1, 10);
            GUILayout.Space(10f);
        }
        else
        {
            for (int i = 0; i < prefabToggles.Length; i++)
            {
                prefabToggles[i] = GUILayout.Toggle(prefabToggles[i], terrain.terrainData.treePrototypes[i].prefab.name);
            }
        }

        GUILayout.Label("Tree small scale " + scaleMin);
        scaleMin = GUILayout.HorizontalSlider(scaleMin, .5f, 1f);
        GUILayout.Space(10f);

        GUILayout.Label("Tree large scale " + scaleMax);
        scaleMax = GUILayout.HorizontalSlider(scaleMax, 1f, 3f);
        GUILayout.Space(10f);

        GUILayout.Label("Height fudge " + heightFudge);
        heightFudge = GUILayout.HorizontalSlider(heightFudge, -2f, 2f);
        GUILayout.Space(20f);

        if (GUILayout.Button("Replace trees!")) Convert();
    }

    [MenuItem("Tools/Terrain Tree Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TerrainTreeGenerator));
    }

    private static GameObject CreateBlankPrefab(TreePrototype prototype)
    {
        GameObject go = new GameObject();
        PrefabUtility.SaveAsPrefabAsset(go, AssetDatabase.GetAssetPath(prototype.prefab).Replace(".prefab", "_blank.prefab"));
        DestroyImmediate(go);
        return AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(prototype.prefab).Replace(".prefab", "_blank.prefab"));
    }

    public void Convert()
    {
        if (terrain == null)
            return;

        var data = terrain.terrainData;

        var treeParent = new GameObject("Trees");
        var treegroups = new List<List<Transform>>();
        var treegroup = new List<Transform>();

        if (sortingByPrefab)
            for (var i = 0; i < data.treePrototypes.Length; i++)
            {
                var t = new GameObject(terrain.terrainData.treePrototypes[i].prefab.name + "_Group");
                t.transform.parent = treeParent.transform;
                treegroup.Add(t.transform);
            }
        else
            for (var i = 0; i < treeDivisions; i++)
            {
                treegroups.Add(new List<Transform>());
                for (var j = 0; j < treeDivisions; j++)
                {
                    var treeGroup = new GameObject("TreeGroup_" + i + "_" + j);
                    treeGroup.transform.parent = treeParent.transform;
                    treegroups[i].Add(treeGroup.transform);
                }
            }

        var width = data.size.x;
        var height = data.size.z;
        var y = data.size.y;

        var xDiv = data.size.x / treeDivisions;
        var zDiv = data.size.z / treeDivisions;

        foreach (var tree in data.treeInstances)
        {
            if (sortingByPrefab)
            {
                if (!prefabToggles[tree.prototypeIndex])
                    continue;
            }

            var position = new Vector3(tree.position.x * width, tree.position.y * y, tree.position.z * height);

            var xGroup = (int)(position.x / xDiv);
            var zGroup = (int)(position.z / zDiv);

            position += terrain.transform.position;

            var scale = Random.Range(scaleMin, scaleMax);
            position.y += heightFudge;

            var newTree = (GameObject)PrefabUtility.InstantiatePrefab(data.treePrototypes[tree.prototypeIndex].prefab);
            newTree.transform.SetPositionAndRotation(position, Quaternion.Euler(Random.Range(0f, 360f) * Vector3.up));
            newTree.transform.localScale = scale * Vector3.one;

            if (sortingByPrefab)
                newTree.transform.SetParent(treegroup[tree.prototypeIndex]);
            else
                newTree.transform.SetParent(treegroups[xGroup][zGroup]);
        }
        if (sortingByPrefab)
        {
            TreePrototype[] prototypes = data.treePrototypes;
            for (int i = 0; i < prefabToggles.Length; i++)
            {
                if (prefabToggles[i] && !prototypes[i].prefab.name.Contains("_blank"))
                    prototypes[i].prefab = CreateBlankPrefab(data.treePrototypes[i]);
            }
            data.treePrototypes = prototypes;
        }
        else
            terrain.drawTreesAndFoliage = false;
    }
}