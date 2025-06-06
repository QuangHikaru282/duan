﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject pauseMenuPanel;
    public GameObject musicMenuPanel;

    private bool isGameOver = false;
    private bool isPaused = false;

    void Start()
    {
  
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        if (musicMenuPanel != null)
        {
            musicMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isGameOver)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (pauseMenuPanel != null && !gameOverPanel.activeSelf)
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null && !musicMenuPanel.activeSelf)
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }
    public void ResumeMusicGame()
    {
        if (musicMenuPanel != null)
        {
            musicMenuPanel.SetActive(false);
        }
    }
    public void OnMusicGame()
    {
        musicMenuPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            // Bắt đầu Coroutine để thêm thời gian chờ
            StartCoroutine(WaitAndShowGameOver());
        }
    }

    private IEnumerator WaitAndShowGameOver()
    {
       
        yield return new WaitForSeconds(2f);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        isGameOver = true;
    }

    public void RespawnGame()
    {
   
        playerScript player = FindObjectOfType<playerScript>();
        if (player != null)
        {
            player.Respawn();
        }

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        isPaused = false;
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    public void MenuGame()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        SceneLoadData.nextSceneName = "scene_Begin";
        SceneManager.LoadScene("scene_Begin");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
