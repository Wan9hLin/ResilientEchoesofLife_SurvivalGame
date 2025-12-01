 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackAnimalController : MonoBehaviour
{
    public Transform player;
    public float attackDistance = 10.0f;
    public float safeDistance = 20.0f;
    public float wanderRadius = 10.0f;

    private NavMeshAgent agent;
    private Animator animator;

    public int maxHealth = 3;
    private int currentHealth;

    private bool isWandering = false;
    private Vector3 wanderDestination;
    public Transform[] wanderPoints;  

    private float normalSpeed = 3.5f;
    public float wanderSpeed = 1.0f;  

    private bool isDead = false;
    private bool isProvoked = false;  

    public float attackStopDistance = 2.0f;
    public GameObject meatPrefabName;  
    public int meatCount = 3;  
    public float meatSpawnRadius = 1f; 
    public GameObject bloodFXPrefab;
    public delegate void AnimalDeathHandler();
    public event AnimalDeathHandler OnDeath;
    
    private PlayerStatsManager playerStatsManager;
    private AttackColliderScript attackColliderScript;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("isMove", false);

        currentHealth = maxHealth;
        agent.speed = wanderSpeed;
        wanderPoints = WanderPointManager_4.Instance.wanderPoints;
        StartCoroutine(Wander());

        player = GameObject.FindWithTag("Player").transform;
        attackColliderScript = transform.Find("AttackCollider").GetComponent<AttackColliderScript>();
        playerStatsManager = player.GetComponent<PlayerStatsManager>();
        playerStatsManager.OnHealthChanged += OnPlayerHealthChanged;
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (isProvoked && distanceToPlayer <= attackDistance)
        {
            // Attack the player if within attack range
            StopCoroutine(Wander());
            StartCoroutine(AttackPlayer());
        }

        else if (isProvoked && distanceToPlayer > attackDistance && distanceToPlayer < safeDistance)
        {
            // Chase the player if within chase range
            StopCoroutine(Wander());
            StopCoroutine(AttackPlayer());
            StartCoroutine(ChasePlayer());
        }

        else if (distanceToPlayer >= safeDistance)
        {
            // Return to wandering if the player is out of range
            isProvoked = false;
            StopCoroutine(ChasePlayer());
            StopCoroutine(AttackPlayer());
            StartCoroutine(Wander());
        }
    }

    void OnDestroy()
    {
       
        playerStatsManager.OnHealthChanged -= OnPlayerHealthChanged;
    }

    void OnPlayerHealthChanged(float newHealth)
    {
        if (newHealth <= 0)
        {
            // Stop attacking if the player dies
            StopAllCoroutines();
            animator.SetBool("isMove", false);
            agent.isStopped = true;
            isProvoked = false; 
            StartCoroutine(DelayedWander()); 
        }
        else
        {
            // Resume chasing if the player respawns
            if (isProvoked)
            {
                StartCoroutine(ChasePlayer());

            }
        }
    }

    IEnumerator DelayedWander()
    {
        yield return null; // Wait for one frame
        agent.isStopped = false; // Restart the NavMeshAgent
        StartCoroutine(Wander());
    }


    IEnumerator Wander()
    {
        if (isDead)
        {
            yield break;
        }

        isWandering = true;
        agent.speed = wanderSpeed;

        while (isWandering)
        {
            // Select a random wander point
            int index = Random.Range(0, wanderPoints.Length);
            wanderDestination = wanderPoints[index].position;

            agent.SetDestination(wanderDestination);
            animator.SetBool("isWalk", true);

            // Wait until destination is reached
            while (agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            animator.SetBool("isWalk", false);
            yield return new WaitForSeconds(1f);
        }

        agent.speed = normalSpeed;
    }

    IEnumerator ChasePlayer()
    {
        agent.speed = normalSpeed;
        animator.SetBool("isMove", true);
        animator.SetBool("isWalk", false);

        while (isProvoked)
        {
            // Chase the player and look at them
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            agent.SetDestination(player.position);
            yield return null;
        }

        animator.SetBool("isMove", false);
        StartCoroutine(Wander());
    }


    IEnumerator AttackPlayer()
    {
      
        agent.speed = normalSpeed;
        StopCoroutine(Wander());
        StopCoroutine(ChasePlayer());

        // Face the player and attack
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (Vector3.Distance(player.position, transform.position) <= attackStopDistance)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1.0f);

        // Continue attacking or start chasing based on distance
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer > attackDistance)
        {
            StartCoroutine(ChasePlayer());
        }
        else
        {
            StartCoroutine(AttackPlayer());
        }
   
    }


    public void TakeDamage(int damage)
    {
        if (isDead)  
        {
            return;
        }
        currentHealth -= damage;
        Instantiate(bloodFXPrefab, transform.position, Quaternion.identity);
        AudioManager.instance.PlaySFX("CutMeat");
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (!isProvoked)
        {
            isProvoked = true;
            StartCoroutine(ChasePlayer());
        }
    }

    void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
        StopAllCoroutines();
        animator.SetTrigger("Dead");
        GetComponent<NavMeshAgent>().enabled = false;

        // Spawn meat resources
        for (int i = 0; i < meatCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-meatSpawnRadius, meatSpawnRadius), 0, Random.Range(-meatSpawnRadius, meatSpawnRadius));
            Vector3 spawnPosition = transform.position + randomOffset;
            Instantiate(meatPrefabName, spawnPosition, Quaternion.identity);
        }


        FindObjectOfType<AnimalSpawnerScript>().OnAnimalDied();
        Invoke("DestroyAnimal", 4.0f);
    }

    void DestroyAnimal()
    {
        Destroy(gameObject);
    }

    public void SetTriggerActive()
    {
        attackColliderScript.SetTriggerActive();
    }

    public void SetTriggerDeActive()
    {
        attackColliderScript.SetTriggerDeActive();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            TakeDamage(other.GetComponent<WeaponDamage>().weapon.damage);

        }
    }

}

