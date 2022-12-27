//  Froggy's Tool // Copyright 2020 Froggy //  Contect - cyshenfrog@gmail.com
//  For more infomation https://www.patreon.com/frogskin

using UnityEngine;

/// <summary>
/// Singleton that load prefab named as <typeparamref name="T"/> type name from path [Resources/prefab/] with DontDestroyOnLoad()
/// </summary>
public class UnitySingleton_DR<T> : MonoBehaviour where T : Component
{
    protected static string PrefabPath = "prefab/" + typeof(T).Name;

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = (GameObject)Resources.Load(PrefabPath);

                if (obj != null)
                    instance = Instantiate(obj).GetComponent<T>();
                else
                {
                    obj = new GameObject("UnitySingletonT");
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (!instance.Equals(this))
                Destroy(gameObject);
        }
    }

    public virtual void Creation()
    {
    }
}