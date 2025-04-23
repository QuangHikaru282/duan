using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public GameObject player;
    public SaveManager saveManager;
    public playerScript playerScript;
    public SkillManager skillManager;
    public SkillUnlockManager skillUnlockManager;
    public BreakableWall Destroywall;

    [Header("Drop Item Settings")]
    public GameObject L;
    public GameObject E;
    public GameObject Q;
    public GameObject DoubleJump;
    public GameObject Door;
    [Header("Skill on table Settings")]
    public GameObject Ltable;
    public GameObject Ltable_2;
    public GameObject Etable;
    public GameObject Etable_2;
    public GameObject Qtable;
    public GameObject Qtable_2;
    public GameObject DoubleJumptable;
    public GameObject DoubleJumptable_2;
    
    

    private bool sceneLoaded = false;

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();

        PlayerData data = saveManager.LoadPlayer();
        Debug.Log(skillUnlockManager.isSkillLUnlocked);
        Debug.Log(skillUnlockManager.isSkillEUnlocked);
        Debug.Log(skillUnlockManager.isSkillQUnlocked);
        Debug.Log(skillUnlockManager.isDoubleJumpUnlocked);
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("aaaa");
    //        OnSaveGameData();
    //    }

    //}
    public void OnSaveGameData()
    {
        Vector2 position = player.transform.position;

        saveManager.SavePlayer(
            playerScript.currentHealth,
            skillManager.currentMana,
            position,
            skillUnlockManager.isSkillLUnlocked,
            skillUnlockManager.isSkillEUnlocked,
            skillUnlockManager.isSkillQUnlocked,
            skillUnlockManager.isDoubleJumpUnlocked,
            Destroywall.isDestroyWall
        );
    }

    public void OnLoadGameData()
    {
        PlayerData data = saveManager.LoadPlayer();

        if (SceneManager.GetActiveScene().name != data.sceneName && !sceneLoaded)
        {
            sceneLoaded = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(data.sceneName);
            
        }
        else
        {
            ApplyLoadedData(data);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        player = GameObject.FindWithTag("Player");
        
        playerScript = player.GetComponent<playerScript>();
        GameObject logicManager = GameObject.FindWithTag("LogicManager");
        if (logicManager != null)
        {
            Transform skillObj = logicManager.transform.Find("SkillManager");
            if (skillObj != null)
            {
                skillManager = skillObj.GetComponent<SkillManager>();
            }
        }

        PlayerData data = saveManager.LoadPlayer();
        ApplyLoadedData(data);
    }

    private void ApplyLoadedData(PlayerData data)
    {
        playerScript.currentHealth = data.health;
        skillManager.currentMana = data.mana;
        player.transform.position = data.position;

        // Khôi phục trạng thái kỹ năng đã mở khóa
        skillUnlockManager.isSkillLUnlocked = data.unlockSkill_L;
        skillUnlockManager.isSkillEUnlocked = data.unlockSkill_E;
        skillUnlockManager.isSkillQUnlocked = data.unlockSkill_Q;
        skillUnlockManager.isDoubleJumpUnlocked = data.unlockSkill_DoubleJump;
        Destroywall.isDestroyWall = data.door;

        HealthUIManager.Instance.UpdateHealthUI(playerScript.currentHealth);
        ManaUIManager.Instance.UpdateManaUI(skillManager.currentMana, skillManager.maxMana);
        if (data.unlockSkill_L == true)
        {
            Destroy(L);
            Ltable.SetActive(false);
            Ltable_2.SetActive(true);
        }
        if (data.unlockSkill_E == true)
        {
            Destroy(E);
            Etable.SetActive(false);
            Etable_2.SetActive(true);
        }
        if (data.unlockSkill_Q == true)
        {
            Destroy(Q);
            Qtable.SetActive(false);
            Qtable_2.SetActive(true);
        }
        if (data.unlockSkill_DoubleJump == true)
        {
            Destroy(DoubleJump);
            DoubleJumptable.SetActive(false);
            DoubleJumptable_2.SetActive(true);
        }
        if (data.door == true)
        {
            Destroy(Door);
        }
    }
}
