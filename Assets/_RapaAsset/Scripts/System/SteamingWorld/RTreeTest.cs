using RTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class RTreeTest : MonoBehaviour
{
    public AssetReference Prefab;
    public AssetReference PrefabVarient;
    private List<RTreeData> data;
    private List<RTreeData> searchResult;
    private RTree<RTreeData> rTree;
    public Vector2 WeithHeight;
    private Envelope envelope;
    private List<int> itemsToRemove = new List<int>();
    private Dictionary<int, AsyncOperationHandle> pool = new Dictionary<int, AsyncOperationHandle>();
    private Vector3 lastPos = Vector3.zero;

    // Start is called before the first frame update
    private void Start()
    {
        var settings = new ES3Settings();
        settings.location = ES3.Location.Resources;
        data = ES3.Load<List<RTreeData>>("RTreeData", "RTreeData.bytes", settings);
        //Addressables.InstantiateAsync("Assets/Prefabs/A (8) Variant.prefab");
        rTree = new RTree<RTreeData>();
        rTree.BulkLoad(data);
    }

    //private void OnGUI()
    //{
    //    //print data with bigger font
    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 60;

    //    if (data != null)
    //    {
    //        foreach (var item in data)
    //        {
    //            GUILayout.Label(item.PrefabPath, style);
    //            GUILayout.Label(item.Position.ToString(), style);
    //            GUILayout.Label(item.Rotation.ToString(), style);
    //        }
    //    }
    //}

    // Update is called once per frame
    private void Update()
    {
        if ((lastPos - transform.position).magnitude > 1)
            lastPos = transform.position;
        else
            return;
        envelope = new Envelope(transform.position.x - WeithHeight.x / 2, transform.position.z - WeithHeight.y / 2, transform.position.x + WeithHeight.x / 2, transform.position.z + WeithHeight.y / 2);

        searchResult = rTree.Search(envelope) as List<RTreeData>;
        // compare pool object and searchResult, keep the same and remove the different
        itemsToRemove.Clear();
        foreach (var item in pool)
        {
            if (!searchResult.Exists(x => x.Key == item.Key))
            {
                Addressables.ReleaseInstance(item.Value);
                itemsToRemove.Add(item.Key);
            }
        }

        foreach (var item in itemsToRemove)
        {
            pool.Remove(item);
        }

        foreach (var item in searchResult)
        {
            if (!pool.ContainsKey(item.Key))
            {
                print(item.PrefabPath);
                pool.Add(item.Key, Addressables.InstantiateAsync(item.PrefabPath, item.Position, item.Rotation));
            }
        }
    }
}