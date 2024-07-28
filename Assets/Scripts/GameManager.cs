using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverUI;

    public GameObject controllerUI;


    private bool isGameOver = false;

    private void Awake()
    {
        // GameManager'in tekil olmas�n� sa�la
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne ge�i�lerinde yok olmas�n� engeller
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Oyun ba�lang�� ayarlar�
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (controllerUI != null)
        {
            controllerUI.SetActive(false);

        }

        // Game Over UI'yi g�ster
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Oyunu durdur
        Time.timeScale = 0f;
    }


    

}
