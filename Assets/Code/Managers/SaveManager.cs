using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    protected override void Awake()
    {
        base.Awake();
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
