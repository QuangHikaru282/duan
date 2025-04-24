using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnlockManager : MonoBehaviour
{
    public bool isSkillLUnlocked = false;
    public bool isSkillEUnlocked = false;
    public bool isSkillQUnlocked = false;
    public bool isDoubleJumpUnlocked = false;
    public bool isDestroyWall = false;

    [Header("Drop Item Settings")]
    //public GameObject L;
    //public GameObject E;
    //public GameObject Q;
    //public GameObject DoubleJump;
    //public GameObject Door;
    [Header("Skill on table Settings")]
    public GameObject Ltable;
    public GameObject Ltable_2;
    public GameObject Etable;
    public GameObject Etable_2;
    public GameObject Qtable;
    public GameObject Qtable_2;
    public GameObject DoubleJumptable;
    public GameObject DoubleJumptable_2;

    //public void start()
    //{
    //    GameObject L = GameObject.Find("DashStone");
    //    if (isSkillLUnlocked = true)
    //    {
    //        Destroy(L);
    //    }
    //    GameObject E = GameObject.Find("HomingBulletStone");
    //    if (isSkillEUnlocked = true)
    //    {
    //        Destroy(E);
    //    }
    //    GameObject Q = GameObject.Find("FlamethrowerStone");
    //    if (isSkillLUnlocked = true)
    //    {
    //        Destroy(Q);
    //    }
    //    GameObject DoubleJump = GameObject.Find("DoubleJumpStone");
    //    if (isDoubleJumpUnlocked = true)
    //    {
    //        Destroy(DoubleJump);
    //    }
    //    GameObject Door = GameObject.Find("DestroyableWall");
    //    if (isDestroyWall = true)
    //    {
    //        Destroy(Door);
    //    }
    //}

    // Có thể thêm phương thức để unlock kỹ năng
    public void UnlockSkill(string skillName)
    {
        switch (skillName)
        {
            case "L":         
                isSkillLUnlocked = true;
                Ltable.SetActive(false);
                Ltable_2.SetActive(true);
                break;
            case "E":                
                isSkillEUnlocked = true;
                Etable.SetActive(false);
                Etable_2.SetActive(true);
                break;
            case "Q":   
                isSkillQUnlocked = true;
                Qtable.SetActive(false);
                Qtable_2.SetActive(true);
                break;
            case "DoubleJump":
                isDoubleJumpUnlocked = true;
                DoubleJumptable.SetActive(false);
                DoubleJumptable_2.SetActive(true);
                break;
        }

        Debug.Log($"Skill {skillName} unlocked!");
    }
}
