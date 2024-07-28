using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    private float maxHealth = 100f;
    private float health=100f;

    public Animator animator;

    private float lastYPosition;
    private bool isFalling = false;
    public float minFallHeightForDamage = 3f;
    public float fallDamageMultiplier = 0.1f; // Y�ksekli�e ba�l� hasar �arpan�

    public AudioClip damageHurtVoice;
    private AudioSource Audio;

    [SerializeField] private CharacterController characterController;
    private bool isDead = false;


    void Start()
    {
        health = maxHealth;

        lastYPosition = transform.position.y;
        Audio = GetComponent<AudioSource>();
        Debug.Log("prevHe�ight:" + lastYPosition);
        Debug.Log("maxh:" + maxHealth);
        Debug.Log("health:" + health);
    }

    void Update()
    {
        // Karakterin d����e ge�ti�ini belirle
        if (characterController.isGrounded)
        {
            if (isFalling)
            {
                CalculateFallDamage();
                isFalling = false;
            }
        }
        else
        {
            if (!isFalling)
            {
                isFalling = true;
                lastYPosition = transform.position.y; // D���� ba�lamadan �nceki y�ksekli�i kaydet
            }
        }
    }

    private void CalculateFallDamage()
    {
        float currentYPosition = transform.position.y;
        float fallHeight = lastYPosition - currentYPosition; // D���� y�ksekli�ini hesapla

        if (fallHeight > minFallHeightForDamage) // D���� y�ksekli�i pozitifse hasar uygula
        {
            float fallDamage = (fallHeight- minFallHeightForDamage) * fallDamageMultiplier; // D����e ba�l� hasar� hesapla
            TakeDamage(fallDamage); // Hesaplanan hasar� uygula
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        Audio.clip = damageHurtVoice;
        Audio.Play();
        health -= damage;

        OnHealthChanged?.Invoke(health);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();

        // Hareketi durdur
        if (characterController != null)
        {
            characterController.enabled = false;

        }


        // �l�m animasyonu tetikle
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Debug.Log("Character has died.");

    }

    public void OnDeathAnimationEnd()
    {
        GameManager.Instance.GameOver();
    }

    public float GetHealth() => health;
    public float GetMaxHealth() => maxHealth;
}


