using UnityEngine;

public class GameController : MonoBehaviour
{
    public Player player;

    public void SaveGame()
    {
        player.SavePlayerData();
    }

    public void LoadGame()
    {
        player.LoadPlayerData();
    }
}