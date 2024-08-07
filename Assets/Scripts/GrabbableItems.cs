using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableItems : MonoBehaviour
{
   private Rigidbody itemBody;

    [SerializeField] private float rotationSpeed;

    public bool IsRotating { get; set; }

    void Start()
    {
        itemBody = GetComponent<Rigidbody>();

        if (itemBody)
        {
            itemBody.isKinematic = true;
        }

        IsRotating = true;
    }

    void Update()
    {
        if (!IsRotating) return;

        transform.Rotate(Vector3.forward * rotationSpeed * (1 - Mathf.Exp(-rotationSpeed * Time.deltaTime)));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            if (itemBody)
                itemBody.constraints = RigidbodyConstraints.FreezePosition;

            IsRotating = true;
        }
    }

    public void ChangeItemBehaviour()
    {
        if (itemBody)
        {
            itemBody.isKinematic = true;
            itemBody.constraints = RigidbodyConstraints.None;
        }
    }
}
