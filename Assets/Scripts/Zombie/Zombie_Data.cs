using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie_Data : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;
    private NavMeshAgent agent;
    public Rigidbody rb;

    public Renderer zombieRenderer;
    public Color hitColor = Color.red;
    public float colorChangeDuration = 0.2f;
    private Color originalColor;

    public float attackRange = 10f;
    public float attackDamage = 25f;

    private Vector3 lastAttackerPosition;
    private bool isDead = false; 

    public delegate void DeathEventHandler(GameObject deadZombie);
    public event DeathEventHandler OnDeathEvent;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        originalColor = zombieRenderer.material.color;
    }

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        if (isDead) return; // Koyun ölü ise işlem yapma

        currentHealth -= damage;
        lastAttackerPosition = attackerPosition; // Attacker position bilgisi saklanıyor

        if (currentHealth <= 0)
        {
            Die(); // Ölüm işlemini başlat
        }
        else
        {
            StartCoroutine(ChangeColor());
        }
    }

    private void OnZombieAttack()
    {
        if (isDead) return;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 rayDirection = transform.forward + Vector3.down * 0.3f;

        Debug.DrawRay(rayOrigin, rayDirection * attackRange, Color.red, 2.0f);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, attackRange))
        {
            
            Character charHealth = hit.collider.GetComponent<Character>();
            
            if (charHealth != null)
            {
                Debug.Log("Hit a sheep!");
                charHealth.TakeDamage(attackDamage, transform.position);
            }
            
        }
        else
        {
            Debug.Log("Raycast didn't hit anything.");
        }
    }

    private void Die()
    {
        if (isDead) return; // Koyun zaten ölü ise hiçbir şey yapma

        isDead = true; // Koyunun ölü olduğunu işaretle
        animator.SetTrigger("die");
        agent.isStopped = true;
        agent.enabled = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        // Ölüm olayını tetikle
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        // Ölüm rengi değişimi
        zombieRenderer.material.color = hitColor;
        yield return new WaitForSeconds(2.5f);

        // Ölüm olayını tetikle
        OnDeathEvent?.Invoke(gameObject);

        // Koyunu yok et
        Destroy(gameObject);
    }



    private IEnumerator ChangeColor()
    {
        zombieRenderer.material.color = hitColor;
        yield return new WaitForSeconds(colorChangeDuration);
        zombieRenderer.material.color = originalColor;
    }
}
