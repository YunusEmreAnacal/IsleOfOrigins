using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;


public class Character : MonoBehaviour
{
    public static Character Instance;
    public event Action<float> OnHealthChanged;
    public event Action<float> OnFoodChanged;
    public event Action OnDeath;

    public float MaxHealth { get; private set; } = 100f;

    public float Health { get; private set; }

    public float MaxFood { get; private set; } = 100f;

    public float Food { get; private set; } 


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
    private bool isCrouch;

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
    
    void Start()
    {
        Health = MaxHealth;
        Food = MaxFood;

        lastYPosition = transform.position.y;
        Audio = GetComponent<AudioSource>();
        Debug.Log("prevHe�ight:" + lastYPosition);
        Debug.Log("maxh:" + MaxHealth);
        Debug.Log("health:" + Health);
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
        Health -= damage;

        OnHealthChanged?.Invoke(Health);

        if (Health <= 0)
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
            if (Food >= MaxFood && Health < MaxHealth)
            {
                Debug.Log("ıncrsesaaaa ");
                Health = Mathf.Min(MaxHealth, Health + 10);
                OnHealthChanged?.Invoke(Health);
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
            if (Food > 0)
            {
                Debug.Log("xxx");
                Food = Mathf.Max(0, Food - 10);
                OnFoodChanged?.Invoke(Food);

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
        if (Food < MaxFood)
        {
            Food = Mathf.Min(MaxFood, foodIncrease + Food);
            OnFoodChanged?.Invoke(Food);
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

}


