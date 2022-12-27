using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorTool_ScriptableObject
{
    public static T FindInstance<T>() where T : ScriptableObject
    {
        T _instance;
        _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
        if (!_instance)
        {
            string filter = "t:" + typeof(T);
            var guids = AssetDatabase.FindAssets(filter);
            if (guids.Length > 0)
            {
                if (guids.Length > 1)
                    Debug.LogWarningFormat("Multiple {0} assets are found.", typeof(T));

                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                Debug.LogFormat("Using {0}.", assetPath);
                _instance = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            // no found any setting file.
            else
            {
                Debug.LogWarning("No instance of " + typeof(T).Name + " is loaded. Please create a " + typeof(T).Name + " asset file.");
            }
        }

        return _instance;
    }
}