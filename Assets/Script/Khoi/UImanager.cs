using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private GameObject pauseUI;
    [SerializeField]
    private GameObject darkUI;
    [SerializeField]
    private GameObject logoUI;
    [SerializeField]
    private GameObject playUI;
    [SerializeField]
    private GameObject stopUI;
    [SerializeField]
    private GameObject playerUI;
    [SerializeField]
    private GameObject cameraUI;
    [SerializeField]
    private GameObject AudioPlay;
    [SerializeField]
    private GameObject AudioGame;
    public void OnRestartPress()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnGameExitPress()
    {
        Application.Quit();
    }
    public void OnStop()
    {
        if (!pauseUI.activeSelf)
        {
            pauseUI.SetActive(true);
            darkUI.SetActive(true);
            Time.timeScale = 0;
        }
        else 
        {
            pauseUI.SetActive(false);
            darkUI.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void OnPlay()
    {
        playUI.SetActive(false);
        logoUI.SetActive(false);
        stopUI.SetActive(true);
        playerUI.SetActive(true);
        cameraUI.SetActive(true);
        AudioPlay.SetActive(false);
        AudioGame.SetActive(true);
    }
}
