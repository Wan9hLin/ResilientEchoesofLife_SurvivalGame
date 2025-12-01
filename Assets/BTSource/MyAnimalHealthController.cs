using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class MyAnimalHealthController : Action
{
    public SharedInt maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;
    private Animator animator;
    private bool destroyScheduled = false;

    public override void OnStart()
    {
        currentHealth = maxHealth.Value;
        animator = GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (currentHealth <= 0)
        {
            Die();
            return TaskStatus.Failure;
        }

        return TaskStatus.Running;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        GetComponent<NavMeshAgent>().enabled = false;

        if (!destroyScheduled)
        {
            destroyScheduled = true;
            
        }
    }

    
}
