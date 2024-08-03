using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SheepHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;
    private NavMeshAgent agent;
    public Rigidbody rb;
    public float knockbackForce = 10f;
    public GameObject mealPrefab;

    public Renderer sheepRenderer;
    public Color hitColor = Color.red;
    public float colorChangeDuration = 0.2f;
    private Color originalColor;

    private Vector3 lastAttackerPosition;
    private bool isDead = false; // Koyunun ölü olup olmadığını kontrol etmek için

    public delegate void DeathEventHandler(GameObject deadSheep);
    public event DeathEventHandler OnDeathEvent;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        originalColor = sheepRenderer.material.color;
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
            FleeFromAttacker();
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
        sheepRenderer.material.color = hitColor;
        yield return new WaitForSeconds(colorChangeDuration);

        // Meal objelerinin spawn edilmesi
        int numberOfMeals = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < numberOfMeals; i++)
        {
            Vector3 spawnPosition = transform.position + UnityEngine.Random.insideUnitSphere * 1.5f;
            spawnPosition.y = transform.position.y;
            Instantiate(mealPrefab, spawnPosition, Quaternion.identity);
        }

        // Ölüm olayını tetikle
        OnDeathEvent?.Invoke(gameObject);

        // Koyunu yok et
        Destroy(gameObject);
    }

    private void FleeFromAttacker()
    {
        if (isDead) return; // Koyun ölü ise kaçma işlemi yapma

        Vector3 fleeDirection = (transform.position - lastAttackerPosition).normalized * 5f;
        agent.SetDestination(transform.position + fleeDirection);
    }

    private IEnumerator ChangeColor()
    {
        sheepRenderer.material.color = hitColor;
        yield return new WaitForSeconds(colorChangeDuration);
        sheepRenderer.material.color = originalColor;
    }
}


