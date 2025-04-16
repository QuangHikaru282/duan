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

    private bool sceneLoaded = false;

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();

        PlayerData data = saveManager.LoadPlayer();


    }
    public void OnSaveGameData()
    {
        Vector2 position = player.transform.position;
        saveManager.SavePlayer(playerScript.currentHealth, skillManager.currentMana, position);
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

        // ✅ Gán lại player và các script sau khi scene load xong
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
        HealthUIManager.Instance.UpdateHealthUI(playerScript.currentHealth);
        ManaUIManager.Instance.UpdateManaUI(skillManager.currentMana, skillManager.maxMana);
    }
}
