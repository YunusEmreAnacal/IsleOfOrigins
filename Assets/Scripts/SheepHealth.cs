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
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Knockback(attackerPosition);
            StartCoroutine(ChangeColor());
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        agent.isStopped = true;
        // Yavaşça yıkılma animasyonunun bitmesini bekle, ardından Meal objesini oluştur ve koyunu yok et
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(2f); // Animasyon süresine göre ayarlayın
        Instantiate(mealPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void Knockback(Vector3 attackerPosition)
    {
        agent.enabled = false;
        Vector3 knockbackDirection = (transform.position - attackerPosition).normalized * knockbackForce;
        rb.AddForce(knockbackDirection, ForceMode.Impulse);
        Invoke("EnableNavMeshAgent", 1f);
    }

    private void EnableNavMeshAgent()
    {
        agent.enabled = true;
    }

    private IEnumerator ChangeColor()
    {
        sheepRenderer.material.color = hitColor;
        yield return new WaitForSeconds(colorChangeDuration);
        sheepRenderer.material.color = originalColor;
    }
}
