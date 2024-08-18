using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class SwimArea : MonoBehaviour
{
    [SerializeField] private AudioClip swimmingVoice;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<AudioSource>().PlayOneShot(swimmingVoice);
            other.GetComponent<ThirdPersonController>().isSwimming = true;
            other.GetComponent<Character>().inWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<ThirdPersonController>().isSwimming = false;
            other.GetComponent<Character>().inWater = false;
        }
    }

}
