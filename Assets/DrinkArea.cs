using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class DrinkArea : MonoBehaviour
{
    [SerializeField] private GameObject drinkButton;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            drinkButton.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            drinkButton.SetActive(false);
        }
    }
}
