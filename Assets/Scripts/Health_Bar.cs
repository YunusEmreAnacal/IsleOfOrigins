using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]

public class Health_Bar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;

    public Slider foodSlider;
    public Slider easefoodSlider;

    public float lerpSpeed = 0.5f;

    public Character character; // Karakter referans�

    private void Start()
    {
        Debug.Log("bar health1:" + character.GetHealth());
        if (character != null) 
        {
            character.OnHealthChanged += UpdateHealthUI;
            character.OnFoodChanged += UpdateFoodUI;
        }

        healthSlider.maxValue = character.MaxHealth;
        easeHealthSlider.maxValue = character.MaxHealth;

        foodSlider.maxValue = character.GetMaxFood();
        easefoodSlider.maxValue = character.GetMaxFood();

        Debug.Log("bar health2:" + character.GetHealth());
        UpdateHealthUI(character.GetHealth());
        UpdateFoodUI(character.GetFood());

        Debug.Log("bar health3:" + character.GetHealth());
        Debug.Log("bar maxh:" + character.MaxHealth);
        Debug.Log("heslid:" + healthSlider.maxValue);
        Debug.Log("easslid:" + easeHealthSlider.maxValue);

        healthSlider.interactable = false;
        easeHealthSlider.interactable = false;
        foodSlider.interactable = false;
        easefoodSlider.interactable = false;

    }

    private void OnDestroy()
    {
        if (character != null)
        {
            character.OnHealthChanged -= UpdateHealthUI;
            character.OnFoodChanged -= UpdateFoodUI;
        }
        
    }

    private void UpdateHealthUI(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    private void UpdateFoodUI(float currentFood)
    {
        foodSlider.value = currentFood;
    }

    private void Update()
    {
        // Ease bar, healthSlider'�n yeni de�erine yava��a yakla��r
        if (easeHealthSlider.value != healthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed );

        }
        // Ease bar, foodSlider'�n yeni de�erine yava��a yakla��r
        if (easefoodSlider.value != foodSlider.value)
        {
            easefoodSlider.value = Mathf.Lerp(easefoodSlider.value, foodSlider.value, lerpSpeed);
        }
    }
}
