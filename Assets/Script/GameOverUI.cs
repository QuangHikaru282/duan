using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Tham chiếu đến GameOverPanel
    public GameObject pauseMenuPanel; // Tham chiếu đến PauseMenuPanel

    private bool isGameOver = false;
    private bool isPaused = false;

    void Start()
    {
        // Đảm bảo các panel bị tắt khi bắt đầu game
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Khôi phục thời gian
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Kiểm tra nếu người chơi nhấn phím Tab
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

    // Hàm để tạm dừng game
    public void PauseGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    // Hàm để tiếp tục game
    public void ResumeGame()
    {
        Debug.Log("resume game dc goi!");
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    // Hàm để hiển thị màn hình Game Over
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

        // Hiển thị màn hình Game Over
        gameOverPanel.SetActive(true);

        // Tạm dừng thời gian
        Time.timeScale = 0;
        isGameOver = true;
    }

    public void RespawnGame()
    {
        // Tìm đối tượng Player
        playerScript player = FindObjectOfType<playerScript>();
        if (player != null)
        {
            // Gọi hàm Respawn() trong playerScript
            player.Respawn();
        }
        else
        {
            Debug.LogError("Player object not found! Respawn failed.");
        }

        // Tắt UI của pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Tiếp tục game
        Time.timeScale = 1f;
        isPaused = false;
    }


    public void RestartGame()
    {
        // Khôi phục thời gian
        Time.timeScale = 1f;
        // Tải lại scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Hàm được gọi khi nhấn nút Exit
    public void ExitGame()
    {
        // Khôi phục thời gian
        Time.timeScale = 1f;
        // Thoát game
        Application.Quit();
        // Nếu đang trong Unity Editor, dừng play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
