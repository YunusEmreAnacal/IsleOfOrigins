using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private GameObject controllerUI;


    private bool isGameOver = false;

    private void Awake()
    {
        // GameManager'in tekil olmasını sağla
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
            gameOverUI.SetActive(false);

    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        controllerUI.SetActive(false);
        gameOverUI.SetActive(true);
        

        // Oyunu durdur
        Time.timeScale = 0f;
    }


    

}
