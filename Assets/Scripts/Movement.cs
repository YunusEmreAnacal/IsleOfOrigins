using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Movement : ThirdPersonController
{
    private Animator anim;
    private CharacterController controller;
    [SerializeField] private GameObject HeadPosition;
    [SerializeField] private bool isCrouch = false;
    [SerializeField] private bool Canstand ;
    

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Crouching();
        
    }


    void Crouching()
    {
        if (Physics.Raycast(HeadPosition.transform.position , Vector3.up, 0.5f))
        {
            Canstand = false;
            Debug.DrawRay(HeadPosition.transform.position,Vector3.up, Color.green);
        }
        else
        {
            Canstand = true;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isCrouch==true && Canstand==true)
            {
                isCrouch = false;
                anim.SetBool("Crouch", false);
                controller.height = 1.8f;
                controller.center = new Vector3(0f, 1f,0f);
                
            }
            else
            {
                isCrouch = true;
                anim.SetBool("Crouch", true);
                controller.height = 1f;
                controller.center = new Vector3(0f, 0.55f,0f);
                
                
            }
        }              

    }
    
}
