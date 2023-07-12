using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    GameObject temp = new GameObject(typeof(T).ToString());
                    _instance = temp.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}