using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Bar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float lerpSpeed = 0.05f;

    public Character character; // Karakter referansý

    private void Start()
    {
        if (character != null)
        {
            character.OnHealthChanged += UpdateHealthUI;
        }

        healthSlider.maxValue = character.GetMaxHealth();
        easeHealthSlider.maxValue = character.GetMaxHealth();
        UpdateHealthUI(character.GetHealth());
    }

    private void OnDestroy()
    {
        if (character != null)
        {
            character.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    private void Update()
    {
        // Ease bar, healthSlider'ýn yeni deðerine yavaþça yaklaþýr
        if (easeHealthSlider.value != healthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed);
        }
    }
}
