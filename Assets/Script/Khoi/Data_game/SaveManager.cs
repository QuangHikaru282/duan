using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SaveManager : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Application.dataPath + "/data_game/playerdata.json";
    }

    public void SavePlayer(int health, int mana, Vector2 position, bool unlockSkill_L, bool unlockSkill_E, bool unlockSkill_Q, bool unlockSkill_DoubleJump, bool door)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerData data = new PlayerData(health, mana, position, currentScene, unlockSkill_L, unlockSkill_E, unlockSkill_Q, unlockSkill_DoubleJump, door);
        string json = JsonUtility.ToJson(data, true);

        string directory = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(savePath, json);
    }

    public PlayerData LoadPlayer()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            return data;
        }
        else
        {
            return new PlayerData(10, 50, Vector2.zero, "StartScene", false, false, false, false, false); // Scene mặc định
        }
    }
}
