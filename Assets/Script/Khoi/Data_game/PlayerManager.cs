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

    public void ResetSceneAndLoadSavedData()
    {
        sceneLoaded = true;
        SceneManager.sceneLoaded += OnSceneLoaded_ExcludeHPMPPos;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded_ExcludeHPMPPos(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerScript>();

        // Lấy SkillManager từ LogicManager
        GameObject logicManager = GameObject.FindWithTag("LogicManager");
        if (logicManager != null)
        {
            Transform skillObj = logicManager.transform.Find("SkillManager");
            if (skillObj != null)
            {
                skillManager = skillObj.GetComponent<SkillManager>();
            }
        }

        // Lấy skillUnlockManager từ player
        skillUnlockManager = player.GetComponent<SkillUnlockManager>();

        // Lấy Destroywall từ scene bằng tên GameObject hoặc tag
        GameObject destroyWallObj = GameObject.Find("DestroyableWall"); // tên đúng GameObject
        if (destroyWallObj != null)
        {
            Destroywall = destroyWallObj.GetComponent<BreakableWall>();
        }

        PlayerData data = saveManager.LoadPlayer();
        ApplyLoadedData_ExcludeHPMPPos(data);
    }

    private void ApplyLoadedData_ExcludeHPMPPos(PlayerData data)
    {
        // KHÔNG cập nhật máu, mana và vị trí
        skillUnlockManager.isSkillLUnlocked = data.unlockSkill_L;
        skillUnlockManager.isSkillEUnlocked = data.unlockSkill_E;
        skillUnlockManager.isSkillQUnlocked = data.unlockSkill_Q;
        skillUnlockManager.isDoubleJumpUnlocked = data.unlockSkill_DoubleJump;
        Destroywall.isDestroyWall = data.door;

        if (data.unlockSkill_L == true)
        {
            GameObject L = GameObject.Find("DashStone");
            Destroy(L);
            if (Ltable == null && Ltable_2 == null)
            {
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    var t1 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/DASH/L");
                    var t2 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/DASH/L_2");

                    if (t2 != null) t2.gameObject.SetActive(true);
                    if (t1 != null) t1.gameObject.SetActive(false);
                }
            }
            else
            {
                Ltable.SetActive(false);
                Ltable_2.SetActive(true);
            }
        }
        if (data.unlockSkill_E == true)
        {
            GameObject E = GameObject.Find("HomingBulletStone");
            Destroy(E);
            if (Etable == null && Etable_2 == null)
            {
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    var v1 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/HomingBullet/E");
                    var v2 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/HomingBullet/E_2");

                    if (v2 != null) v2.gameObject.SetActive(true);
                    if (v1 != null) v1.gameObject.SetActive(false);
                }
            }
            else
            {
                Etable.SetActive(false);
                Etable_2.SetActive(true);
            }

        }
        if (data.unlockSkill_Q == true)
        {
            GameObject Q = GameObject.Find("FlamethrowerStone");
            Destroy(Q);
            if (Etable == null && Etable_2 == null)
            {
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    var b1 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/Flamethrower/Q");
                    var b2 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/Flamethrower/Q_2");

                    if (b2 != null) b2.gameObject.SetActive(true);
                    if (b1 != null) b1.gameObject.SetActive(false);
                }
            }
            else
            {
                Qtable.SetActive(false);
                Qtable_2.SetActive(true);
            }
        }
        if (data.unlockSkill_DoubleJump == true)
        {
            GameObject DoubleJump = GameObject.Find("DoubleJumpStone");
            Destroy(DoubleJump);
            if (Etable == null && Etable_2 == null)
            {
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    var u1 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/DoubleJump/SPACE");
                    var u2 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/DoubleJump/SPACE_2");

                    if (u2 != null) u2.gameObject.SetActive(true);
                    if (u1 != null) u1.gameObject.SetActive(false);
                }
            }
            else
            {
                DoubleJumptable.SetActive(false);
                DoubleJumptable_2.SetActive(true);
            }
        }
        if (data.door == true)
        {
            GameObject Door = GameObject.Find("DestroyableWall");
            Destroy(Door);
        }
    }


    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();

        PlayerData data = saveManager.LoadPlayer();
    }
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

        // Lấy SkillManager từ LogicManager
        GameObject logicManager = GameObject.FindWithTag("LogicManager");
        if (logicManager != null)
        {
            Transform skillObj = logicManager.transform.Find("SkillManager");
            if (skillObj != null)
            {
                skillManager = skillObj.GetComponent<SkillManager>();
            }
        }

        // Lấy skillUnlockManager từ player
        skillUnlockManager = player.GetComponent<SkillUnlockManager>();

        // Lấy Destroywall từ scene bằng tên GameObject hoặc tag
        GameObject destroyWallObj = GameObject.Find("DestroyableWall"); // tên đúng GameObject
        if (destroyWallObj != null)
        {
            Destroywall = destroyWallObj.GetComponent<BreakableWall>();
        }

        // Áp dụng dữ liệu đã load
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
        skillUnlockManager.isDestroyWall = data.door;

        HealthUIManager.Instance.UpdateHealthUI(playerScript.currentHealth);
        ManaUIManager.Instance.UpdateManaUI(skillManager.currentMana, skillManager.maxMana);
        if (data.unlockSkill_L == true)
        {
            GameObject L = GameObject.Find("DashStone");
            Destroy(L);
            if (Ltable == null && Ltable_2 == null)
            {
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    var t1 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/DASH/L");
                    var t2 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/DASH/L_2");

                    if (t2 != null) t2.gameObject.SetActive(true);
                    if (t1 != null) t1.gameObject.SetActive(false);
                }
            }
            else
            {
                Ltable.SetActive(false);
                Ltable_2.SetActive(true);
            } 
        }
        if (data.unlockSkill_E == true)
        {
            GameObject E = GameObject.Find("HomingBulletStone");
            Destroy(E);
            if (Etable == null && Etable_2 == null)
            {
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    var v1 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/HomingBullet/E");
                    var v2 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/HomingBullet/E_2");

                    if (v2 != null) v2.gameObject.SetActive(true);
                    if (v1 != null) v1.gameObject.SetActive(false);
                }
            }
            else
            {
                Etable.SetActive(false);
                Etable_2.SetActive(true);
            }

        }
        if (data.unlockSkill_Q == true)
        {
            GameObject Q = GameObject.Find("FlamethrowerStone");
            Destroy(Q);
            if (Etable == null && Etable_2 == null)
            {
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    var b1 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/Flamethrower/Q");
                    var b2 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/Flamethrower/Q_2");

                    if (b2 != null) b2.gameObject.SetActive(true);
                    if (b1 != null) b1.gameObject.SetActive(false);
                }
            }
            else
            {
                Qtable.SetActive(false);
                Qtable_2.SetActive(true);
            }
        }
        if (data.unlockSkill_DoubleJump == true)
        {
            GameObject DoubleJump = GameObject.Find("DoubleJumpStone");
            Destroy(DoubleJump);
            if (Etable == null && Etable_2 == null)
            {
                var canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    var u1 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/DoubleJump/SPACE");
                    var u2 = canvas.transform
                                   .Find("UI/MidUI/HowToPlayUI/HowToPlayUI/DoubleJump/SPACE_2");

                    if (u2 != null) u2.gameObject.SetActive(true);
                    if (u1 != null) u1.gameObject.SetActive(false);
                }
            }
            else
            {
                DoubleJumptable.SetActive(false);
                DoubleJumptable_2.SetActive(true);
            }
        }
        if (data.door == true)
        {
            GameObject Door = GameObject.Find("DestroyableWall");
            Destroy(Door);
        }

    }
}
