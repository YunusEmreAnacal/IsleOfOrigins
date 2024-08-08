using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    private NavMeshAgent agent = null;
    private Animator animator = null;
    [SerializeField] private Transform target;
    public float range;
    public Transform centrePoint;

    private Zombie_Data zd;

    private bool inTrigger = false;

    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        zd = GetComponent<Zombie_Data>();
        timer = wanderTimer;
    }

    void Update()
    {
        if (inTrigger)
        {
            MoveToTarget();
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
                animator.SetBool("isWalking", agent.speed > 0.5f); // Walk animation if moving
            }

            
        }

        
    }

    

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
    private void MoveToTarget()
    {
        agent.SetDestination(target.position);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= agent.stoppingDistance)
        {
            Debug.Log(distanceToTarget);
            RotateToTarget();
            animator.SetBool("isAttacking", true);
            //agent.isStopped = true;
        }
        else
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f); 
            //agent.isStopped = false; 
        }

       
    }

   

    private void RotateToTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = false;
            
        }
    }
}
