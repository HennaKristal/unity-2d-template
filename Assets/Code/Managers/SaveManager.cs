using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance => _instance;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void SaveData()
    {
        PlayerPrefs.SetInt("saveFileFound", 1);
    }

    public void LoadData()
    {
        int saveFileFound = PlayerPrefs.GetInt("saveFileFound", 0);
    }

    public void ResetData()
    {
        PlayerPrefs.SetInt("saveFileFound", 0);
    }

    public void DeleteData(string name)
    {
        PlayerPrefs.DeleteKey(name);
    }

    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }
}
