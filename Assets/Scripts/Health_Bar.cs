using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Bar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float lerpSpeed = 0.5f;

    public Character character; // Karakter referans�

    private void Start()
    {
        Debug.Log("bar health1:" + character.GetHealth());
        if (character != null) 
        {
            character.OnHealthChanged += UpdateHealthUI;
        }

        healthSlider.maxValue = character.GetMaxHealth();
        easeHealthSlider.maxValue = character.GetMaxHealth();
        Debug.Log("bar health2:" + character.GetHealth());
        UpdateHealthUI(character.GetHealth());

        Debug.Log("bar health3:" + character.GetHealth());
        Debug.Log("bar maxh:" + character.GetMaxHealth());
        Debug.Log("heslid:" + healthSlider.maxValue);
        Debug.Log("easslid:" + easeHealthSlider.maxValue);

        healthSlider.interactable = false;
        easeHealthSlider.interactable = false;
        
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
        // Ease bar, healthSlider'�n yeni de�erine yava��a yakla��r
        if (easeHealthSlider.value != healthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed );
        }
    }
}
