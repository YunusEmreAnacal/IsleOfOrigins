using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class Character : MonoBehaviour
{
    public event Action<float> OnHealthChanged;
    public event Action<float> OnFoodChanged;
    public event Action OnDeath;

    private float maxHealth = 100f;
    private float health = 100f;

    private float maxFood = 100f;
    private float food = 100f;

    public Animator animator;

    private float lastYPosition;
    private bool isFalling = false;
    public float minFallHeightForDamage = 3f;
    public float fallDamageMultiplier = 0.1f; // Y�ksekli�e ba�l� hasar �arpan�

    private float timer = 0f;
    private float timer2 = 0f;
    public float hungerTime = 4f;

    public float healthIncreaseTime = 3f;

    public AudioClip damageHurtVoice;
    private AudioSource Audio;

    [SerializeField] private CharacterController characterController;
    private bool isDead = false;


    void Start()
    {
        health = maxHealth;
        food = maxFood;

        lastYPosition = transform.position.y;
        Audio = GetComponent<AudioSource>();
        Debug.Log("prevHe�ight:" + lastYPosition);
        Debug.Log("maxh:" + maxHealth);
        Debug.Log("health:" + health);
    }

    void Update()
    {

        ReduceHunger();

        timer2 += Time.deltaTime;
        if (timer2 > healthIncreaseTime)
        {
            IncreaseHealth();
            timer2 = 0f;
        }

        // Karakterin düşüşee geçtiğini belirle
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
                lastYPosition = transform.position.y; // Düşüş başlamadan önceki yüksekliği kaydet
            }
        }
    }

    private void CalculateFallDamage() // fallDamage ile can azalma hesaplaması 
    {
        float currentYPosition = transform.position.y;
        float fallHeight = lastYPosition - currentYPosition; // D���� y�ksekli�ini hesapla

        if (fallHeight > minFallHeightForDamage) // D���� y�ksekli�i pozitifse hasar uygula
        {
            float fallDamage = (fallHeight - minFallHeightForDamage) * fallDamageMultiplier; // D����e ba�l� hasar� hesapla
            TakeDamage(fallDamage); // Hesaplanan hasar� uygula
        }
    }

    public void TakeDamage(float damage) // karakterin canını düşüren fonksiyon
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

    public void IncreaseHealth()
    {
        if (isDead) return;
        
            if (food >= 100f && health != 100f)
            {
                Debug.Log("ıncrsesaaaa ");
                health += 10;

            }
        
        OnHealthChanged?.Invoke(health);
    }



    public void ReduceHunger()
    {
        
        if (isDead) return;

        timer += Time.deltaTime;
        if (timer >= hungerTime)
        {
            if (food > 0)
            {
                Debug.Log("xxx");
                food -= 10;
                timer = 0f;
            }
            else
            {
                TakeDamage(10);
                timer = 0f;
            }
        }
        OnFoodChanged?.Invoke(food);

    }

    public void IncreaseFood(float foodIncrease)
    {
        if (isDead) return;
        if (food >= 0 && food < 100)
        {
            food += foodIncrease;
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

    public float GetFood() => food;
    public float GetMaxFood() => maxFood;
}


