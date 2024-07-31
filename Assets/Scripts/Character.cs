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

    public float MaxHealth { get; private set; } = 100f;
    private float health ;

    private float maxFood = 100f;
    private float food ;

    public Animator animator;

    private float lastYPosition;
    private bool isFalling = false;
    public float minFallHeightForDamage = 3f;
    public float fallDamageMultiplier = 0.1f; // Y�ksekli�e ba�l� hasar �arpan�

    private float hungerTimer = 0f;
    private float healthTimer = 0f;

    public float hungerTime = 4f;
    public float healthIncreaseTime = 3f;

    public AudioClip damageHurtVoice;
    private AudioSource Audio;

    [SerializeField] private CharacterController characterController;
    private bool isDead = false;


    void Start()
    {
        health = MaxHealth;
        food = maxFood;

        lastYPosition = transform.position.y;
        Audio = GetComponent<AudioSource>();
        Debug.Log("prevHe�ight:" + lastYPosition);
        Debug.Log("maxh:" + MaxHealth);
        Debug.Log("health:" + health);
    }

    void Update()
    {

        ReduceHunger();
        IncreaseHealth();

        

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

    private void IncreaseHealth()
    {
        if (isDead) return;
        healthTimer += Time.deltaTime;
        if (healthTimer > healthIncreaseTime)
        {
            if (food >= maxFood && health < MaxHealth)
            {
                Debug.Log("ıncrsesaaaa ");
                health = Mathf.Min(MaxHealth,health +10);
                OnHealthChanged?.Invoke(health);
                healthTimer = 0f;
            }
            
        }
 
        
    }



    private void ReduceHunger()
    {
        
        if (isDead) return;

        hungerTimer += Time.deltaTime;
        if (hungerTimer >= hungerTime)
        {
            if (food > 0)
            {
                Debug.Log("xxx");
                food = Mathf.Max(0, food-10);
                OnFoodChanged?.Invoke(food);
                
            }
            else
            {
                TakeDamage(10);
                
            }
            hungerTimer = 0f;
        }
        

    }

    public void IncreaseFood(float foodIncrease)
    {
        if (isDead) return;
        if (food < maxFood)
        {
            food += foodIncrease;//revize
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


    public float GetFood() => food;
    public float GetMaxFood() => maxFood;
}


