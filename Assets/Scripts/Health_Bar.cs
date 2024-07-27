using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Bar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    private float lerpSpeed = 0.05f;

    private float timer = 0f;
    public float interval = 5f; // 5 saniyelik aralýk

    public GameObject character; // Karakter referansý
    public Animator animator; // Animator referansý

    void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (healthSlider.value != health)
        {
            healthSlider.value = health;
        }

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            TakeDamage(10);
            timer = 0f;
        }
        

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value,health,lerpSpeed);
        }
    }

    void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die"); // Ölüm animasyonu

        }
    }
}
