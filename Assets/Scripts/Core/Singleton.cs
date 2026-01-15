using UnityEngine;

/// <summary>
/// Singleton pattern implementation for MonoBehaviour.
/// Use this for managers that should only have one instance in the scene.
/// 
/// Example: public class MyManager : Singleton<MyManager>
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool _applicationIsQuitting = false;

    /// <summary>
    /// Gets the singleton instance
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T)} already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // Find existing instance in scene
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        // Create new GameObject with the component
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"{typeof(T).Name} (Singleton)";

                        // Make it persist across scene loads
                        DontDestroyOnLoad(singletonObject);

                        Debug.Log($"[Singleton] Created instance of {typeof(T)}");
                    }
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// Called when the application quits
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    /// <summary>
    /// Called when the object is destroyed
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _applicationIsQuitting = true;
        }
    }
}
