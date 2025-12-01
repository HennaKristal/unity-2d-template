using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


// Example Usage inside PlayerController
/*
public void Save(ref PlayerSaveData data)
{
    data.playerPosition = transform.position;
}

public void Save(PlayerSaveData data)
{
    transform.position = data.playerPosition;
}

public struct PlayerSaveData
{
    public Vector3 playerPosition;
}
*/


public class SaveManager
{
    private static SaveData saveData = new SaveData();
    private static bool encryptData = false;


    public struct SaveData
    {
        public string lastSceneName;
        //public PlayerSaveData playerSaveData;
        //public PlayerUpgradeSaveData playerUpgradeSaveData;
    }

    public static string SaveFileName(int slotIndex)
    {
        string directory = Application.persistentDataPath + "/save/";

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string saveFile = directory + "slot_" + slotIndex + ".save";
        return saveFile;
    }

    public static void Save(int slotIndex)
    {
        HandleSaveData();

        string json = JsonUtility.ToJson(saveData, true);

        if (encryptData)
        {
            json = EncryptionUtility.EncryptString(json);
        }

        File.WriteAllText(SaveFileName(slotIndex), json);
    }

    private static void HandleSaveData()
    {
        // GameManager.Instance.Player.Save(ref _saveData.PlayerData)
        // GameManager.Instance.PlayerUpgrades.Save(ref _saveData.PlayerUpgradeData)

        // Scene name save
        saveData.lastSceneName = SceneManager.GetActiveScene().name;
    }

    public static void Load(int slotIndex)
    {
        string saveFile = SaveFileName(slotIndex);

        if (!File.Exists(saveFile))
        {
            return;
        }

        string json = File.ReadAllText(saveFile);

        if (encryptData)
        {
            json = EncryptionUtility.DecryptString(json);
        }

        saveData = JsonUtility.FromJson<SaveData>(json);


        HandleLoadData();
    }

    public static void HandleLoadData()
    {
        // GameManager.Instance.Player.Load(_saveData.PlayerData)
        // GameManager.Instance.PlayerUpgrades.Load(_saveData.PlayerUpgradeData)

        // Scene name load
        if (!string.IsNullOrEmpty(saveData.lastSceneName))
        {
            SceneManager.LoadScene(saveData.lastSceneName);
        }
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt("preference-1", 1);
    }

    public void LoadPlayerPrefs()
    {
        int saveFileFound = PlayerPrefs.GetInt("preference-1", 0);
    }
}
