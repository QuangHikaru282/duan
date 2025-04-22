using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnlockManager : MonoBehaviour
{
    public bool isSkillLUnlocked = false;
    public bool isSkillEUnlocked = false;
    public bool isSkillQUnlocked = false;
    public bool isDoubleJumpUnlocked = false;

    // Có thể thêm phương thức để unlock kỹ năng
    public void UnlockSkill(string skillName)
    {
        switch (skillName)
        {
            case "L":
                isSkillLUnlocked = true;
                break;
            case "E":
                isSkillEUnlocked = true;
                break;
            case "Q":
                isSkillQUnlocked = true;
                break;
            case "DoubleJump":
                isDoubleJumpUnlocked = true;
                break;
        }

        Debug.Log($"Skill {skillName} unlocked!");
    }
}
