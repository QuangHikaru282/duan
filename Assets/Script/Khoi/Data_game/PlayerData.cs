using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public int mana;
    public int bulletCount;
    public float x;
    public float y;
    public string sceneName;

    public PlayerData(int health, int mana, int bulletCount, Vector2 position, string sceneName)
    {
        this.health = health;
        this.mana = mana;
        this.bulletCount = bulletCount;
        this.x = position.x;
        this.y = position.y;
        this.sceneName = sceneName;
    }

    public Vector2 position => new Vector2(x, y);
}

