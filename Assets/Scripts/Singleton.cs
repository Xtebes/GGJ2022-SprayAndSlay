using UnityEngine;
using System.Collections;
public class Singleton<T> : MonoBehaviour where T : Component
{
    public bool destroyOnLoad;
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                /*if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }*/
            }
            return instance;
        }
    }
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (!destroyOnLoad) DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }
}
