using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Meal : MonoBehaviour
{
    public int foodIncrease = 25; // Etin karakterin sağlığını ne kadar artıracağı
    //public Character character;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            Debug.Log("girdi." + other.gameObject.name);
            
                // Karakterin sağlığını artır
                Debug.Log("girdiiiiiii." + other.gameObject.name);
                Character.Instance.IncreaseFood(foodIncrease);

                // Et objesini yok et
                Destroy(gameObject);
            
        }
    }
}
