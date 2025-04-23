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

    public void SavePlayer(int health, int mana, int bulletCount, Vector2 position)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerData data = new PlayerData(health, mana, bulletCount, position, currentScene);
        string json = JsonUtility.ToJson(data, true);

        string directory = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(savePath, json);
        Debug.Log("Đã lưu dữ liệu tại: " + savePath);
    }

    public PlayerData LoadPlayer()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log($"Đã load: máu {data.health}, mana {data.mana}, vị trí ({data.x}, {data.y}), scene {data.sceneName}");
            return data;
        }
        else
        {
            Debug.Log("Không tìm thấy file, trả về dữ liệu mặc định");
            return new PlayerData(10, 50, 0, Vector2.zero, "StartScene");
        }
    }
}
