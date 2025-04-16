using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public int mana;
    public float x;
    public float y;
    public string sceneName;

    public PlayerData(int health, int mana, Vector2 position, string sceneName)
    {
        this.health = health;
        this.mana = mana;
        this.x = position.x;
        this.y = position.y;
        this.sceneName = sceneName;
    }

    public Vector2 position => new Vector2(x, y);
}

