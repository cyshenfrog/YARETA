//  Froggy's Tool // Copyright 2020 Froggy //  Contect - cyshenfrog@gmail.com
//  For more infomation https://www.patreon.com/frogskin

using UnityEngine;

/// <summary>
/// Singleton With DontDestroyOnLoad()
/// </summary>
/// <typeparam name="T"></typeparam>
public class UnitySingleton_D<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
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