using UnityEngine;

public class SingletonSO<T> : ScriptableObject where T : ScriptableObject
{
    protected static string Path = "ScriptableObject/" + typeof(T).Name;
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = Resources.Load<T>(Path);
                if (!_instance)
                    Debug.LogWarning("No instance of " + typeof(T).Name + " is loaded. Please create a " + typeof(T).Name + " asset file.");
            }
            return _instance;
        }
    }
}