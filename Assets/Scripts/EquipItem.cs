using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class EquipItem : MonoBehaviour
{
    [Header("BoxCast Settings")]
    [SerializeField][Range(0.0f, 2.0f)] private float boxCastLength = 2.0f; // BoxCast uzunluğu
    [SerializeField] private Vector3 boxSize = new Vector3(0.5f, 0.5f, 1.0f); // BoxCast boyutları
    [SerializeField] private Vector3 boxOffset = new Vector3(0, 0.175f, 0); // BoxCast'in konumu
    [SerializeField] private LayerMask itemMask;

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
    public ThirdPersonController controller;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        if (controller == null)
        {
            controller = GetComponent<ThirdPersonController>();
        }
    } 
    private void BoxCastHandler()
    {
        Vector3 origin = transform.position + boxOffset; // BoxCast'in başlangıç pozisyonu
        Vector3 direction = transform.forward; // BoxCast'in yönü

        // BoxCast tüm çarpan nesneleri saklamak için
        RaycastHit[] hits = Physics.BoxCastAll(origin, boxSize * 0.5f, direction, Quaternion.identity, boxCastLength, itemMask);

        foreach (RaycastHit hit in hits)
        {
            // Çarptığınız nesnelerle etkileşime geçin
            currentItem = hit.transform.GetComponent<GrabbableItems>();

        }
    }


    public void Equip()
    {
        BoxCastHandler();

        if (currentItem)
        {
            currentItem.transform.parent = equipPos.transform; // Eline alınca
            currentItem.transform.position = equipPos.position;
            currentItem.transform.rotation = equipPos.rotation;

            leftHandIK.weight = 0f;
        }

        if (currentItem != null)
        {
            currentItem.IsRotating = false;

            currentItem.ChangeItemBehaviour();
            currentItem.equipButton.SetActive(false);
            currentItem.unEquipButton.SetActive(true);
            controller.SetItemEquipped(true);
            IsEquiped = true;
        }
    }

    public void UnEquip() // buttona bağlı kalmasın
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

            currentItem.transform.rotation = Quaternion.identity;
            currentItem.GetComponent<Rigidbody>().isKinematic = false;
            currentItem.equipButton.SetActive(true);
            currentItem.unEquipButton.SetActive(false);
            controller.SetItemEquipped(false);
            currentItem = null;
        }
    }
}
