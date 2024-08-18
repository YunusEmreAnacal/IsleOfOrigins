using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Meal : MonoBehaviour
{
    public int foodIncrease = 25; // Etin karakterin sağlığını ne kadar artıracağı
    [SerializeField]private AudioClip eatVoice;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            Character.Instance.IncreaseFood(foodIncrease);
            other.GetComponent<AudioSource>().PlayOneShot(eatVoice);
            // Et objesini yok et
            Destroy(gameObject);
            
        }
    }
}
