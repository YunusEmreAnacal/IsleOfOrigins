using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverUI;
    public GameObject pauseMenuUI;
    public GameObject controllerUI;


    private bool isGameOver = false;

    private void Awake()
    {
        // GameManager'in tekil olmasýný saðla
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne geçiþlerinde yok olmasýný engeller
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Oyun baþlangýç ayarlarý
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (controllerUI != null)
        {
            controllerUI.SetActive(false);

        }

        // Game Over UI'yi göster
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Oyunu durdur
        Time.timeScale = 0f;
    }


    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    

}
