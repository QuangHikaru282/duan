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
    public bool unlockSkill_L;
    public bool unlockSkill_E;
    public bool unlockSkill_Q;
    public bool unlockSkill_DoubleJump;
    public bool door;

    public PlayerData(int health, int mana, Vector2 position, string sceneName,bool unlockSkill_L, bool unlockSkill_E, bool unlockSkill_Q, bool unlockSkill_DoubleJump, bool door)
    {
        this.health = health;
        this.mana = mana;
        this.x = position.x;
        this.y = position.y;
        this.sceneName = sceneName;
        this.unlockSkill_L = unlockSkill_L;
        this.unlockSkill_E = unlockSkill_E;
        this.unlockSkill_Q = unlockSkill_Q;
        this.unlockSkill_DoubleJump = unlockSkill_DoubleJump;
        this.door = door;

    }

    public Vector2 position => new Vector2(x, y);
}

