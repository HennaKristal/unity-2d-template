using UnityEngine;


public class DontDestroyOnLoadObjects : MonoBehaviour
{
    public static DontDestroyOnLoadObjects Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }
}
