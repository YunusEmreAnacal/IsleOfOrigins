using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;

    private float maxHealth = 100f;
    private float health;

    public Animator animator;

    private float lastYPosition;
    private bool isFalling = false;
    public float minFallHeightForDamage = 3f;
    public float fallDamageMultiplier = 0.1f; // Yüksekliðe baðlý hasar çarpaný

    public AudioClip damageHurtVoice;
    private AudioSource Audio;

    [SerializeField] private CharacterController characterController;
    private bool isDead = false;


    void Start()
    {
        health = maxHealth;
        lastYPosition = transform.position.y;
        Audio = GetComponent<AudioSource>();
        Debug.Log("prevHeþight:" + lastYPosition);
    }

    void Update()
    {
        // Karakterin düþüþe geçtiðini belirle
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
                lastYPosition = transform.position.y; // Düþüþ baþlamadan önceki yüksekliði kaydet
            }
        }
    }

    private void CalculateFallDamage()
    {
        float currentYPosition = transform.position.y;
        float fallHeight = lastYPosition - currentYPosition; // Düþüþ yüksekliðini hesapla

        if (fallHeight > minFallHeightForDamage) // Düþüþ yüksekliði pozitifse hasar uygula
        {
            float fallDamage = (fallHeight- minFallHeightForDamage) * fallDamageMultiplier; // Düþüþe baðlý hasarý hesapla
            TakeDamage(fallDamage); // Hesaplanan hasarý uygula
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


        // Ölüm animasyonu tetikle
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


