using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class EquipItem : MonoBehaviour
{
    [Header("Ray Settings")]
    [SerializeField][Range(0.0f, 2.0f)] private float rayLength;
    [SerializeField] private Vector3 rayOffset;
    [SerializeField] private LayerMask itemMask;
    private RaycastHit topRayHitInfo;
    private RaycastHit bottomRayHitInfo;

    private GrabbableItems currentItem;

    [SerializeField] private Transform equipPos;

    private Animator playerAnimator;


    [Header("Right Hand Target")]
    [SerializeField] private TwoBoneIKConstraint rightHandIK;
    [SerializeField] private Transform rightHandTarget;

    [Header("Left Hand Target")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;

    [SerializeField] private Transform IKRightHandPos;
    [SerializeField] private Transform IKLeftHandPos;

    public bool IsEquiped = false;

    

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.E))
        {
            Equip();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnEquip();
        }
        */

        if (currentItem)
        {

            currentItem.transform.parent = equipPos.transform;
            currentItem.transform.position = equipPos.position;
            currentItem.transform.rotation = equipPos.rotation;

            leftHandIK.weight = 0f;

        }

    }

    private void RaycastsHandler()
    {
        Ray topRay = new Ray(transform.position + rayOffset, transform.forward);
        Ray bottomRay = new Ray(transform.position + Vector3.up * 0.175f, transform.forward);

        Debug.DrawRay(transform.position + rayOffset, transform.forward * rayLength, Color.black);
        Debug.DrawRay(transform.position + Vector3.up * 0.175f, transform.forward * rayLength, Color.yellow);

        Physics.Raycast(topRay, out topRayHitInfo, rayLength, itemMask);
        Physics.Raycast(bottomRay, out bottomRayHitInfo, rayLength, itemMask);
    }

    public void Equip()
    {
        RaycastsHandler();

        if (topRayHitInfo.collider != null)
        {
            currentItem = topRayHitInfo.transform.gameObject.GetComponent<GrabbableItems>();
        }

        if (bottomRayHitInfo.collider)
        {
            currentItem = bottomRayHitInfo.transform.gameObject.GetComponent<GrabbableItems>();
        }

        if (!currentItem) return;

        // Stop weapon rotation.
        currentItem.IsRotating = false;

        currentItem.ChangeItemBehaviour();
        currentItem.equipButton.SetActive(false);
        currentItem.unEquipButton.SetActive(true);
        IsEquiped = true;//tpscontroller scriptine taþýýýýý
    }

    public void UnEquip()
    {
        if (IsEquiped)
        {
            rightHandIK.weight = 0.0f;

            if (IKLeftHandPos)
            {
                leftHandIK.weight = 0.0f;
            }

            IsEquiped = false;
            currentItem.transform.parent = null;

            currentItem.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            currentItem.GetComponent<Rigidbody>().isKinematic = false;
            currentItem.equipButton.SetActive(true);
            currentItem.unEquipButton.SetActive(false);
            currentItem = null;
        }
    }
}
