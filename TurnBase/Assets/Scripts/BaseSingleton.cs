using UnityEngine;

public class BaseSingleton<T>: MonoBehaviour where T:BaseSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<T>();
            }
            if(instance == null) Debug.LogError("Can't find singleton " + typeof(T).Name);
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(gameObject); 
        }
        else if (instance != this)
        {
            Debug.LogError("Another instance "+  typeof(T).Name + " exists");
            Destroy(gameObject);
        }
    }
}