using System;

[Serializable]
public class DataGame
{
    // Vị trí người chơi
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    // Số điểm của người chơi
    public int currentDiamond;

    // Tên scene hiện tại
    public string currentSceneName;
}
