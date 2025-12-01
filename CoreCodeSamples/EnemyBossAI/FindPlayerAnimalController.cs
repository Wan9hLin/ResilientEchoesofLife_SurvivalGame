using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FindPlayerAnimalController : MonoBehaviour
{
    public Transform player;
    public float attackDistance = 10.0f;
    private NavMeshAgent agent;
    private Animator animator;

    public int maxHealth = 3;
    private int currentHealth;

    private float normalSpeed = 3.5f;
    private bool isDead = false;
    public float attackStopDistance = 2.0f;

    private AttackColliderScript attackColliderScript;
    public GameObject meatPrefabName;  
    public int meatCount = 3;  
    public float meatSpawnRadius = 1f; 

    public GameObject bloodFXPrefab;
    private AudioSource audioSource;
    private PlayerStatsManager playerStatsManager;

    
    private bool Isoutdoor = true;
    private bool isTeleported = false;
    public Transform spawnPoint;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("isMove", false);
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        agent.speed = normalSpeed;

        player = GameObject.FindWithTag("Player").transform;
        attackColliderScript = transform.Find("AttackCollider").GetComponent<AttackColliderScript>();
        
        StartCoroutine(ChasePlayer());
        playerStatsManager = player.GetComponent<PlayerStatsManager>();
        playerStatsManager.OnHealthChanged += OnPlayerHealthChanged;

        PlayerStatsManager.Instance.OnPlayerOutdoorStatusChanged += IsPlayerOutdoor;
        if(audioSource)audioSource.Play();
    }

    void Update()
    {
        if (Isoutdoor)
        {
            if (isDead)
            {
                return;
            }

            float distanceToPlayer = Vector3.Distance(player.position, transform.position);

            // Transition between chase and attack based on distance to player
            if (distanceToPlayer <= attackDistance)
            {
                StopCoroutine(ChasePlayer());
                StartCoroutine(AttackPlayer());
            }
            else if (distanceToPlayer > attackDistance)
            {
                StopCoroutine(AttackPlayer());
                StartCoroutine(ChasePlayer());
            }
        }
        else
        {
            if (!isTeleported) // Teleport if the player is indoors
            {
                TeleportToSpawnPoint();
            }
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
            // Player has died, stop all attacks and chases
            StopAllCoroutines();
            animator.SetBool("isMove", false);
            agent.isStopped = true;
        }
        else
        {
            // Player has respawned, start chasing and attacking again
            StartCoroutine(ChasePlayer());
        }
    }

    // Event triggered when the player's outdoor status changes
    public void IsPlayerOutdoor(bool isOutdoor)
    {
        Debug.Log("animal get event player status change to"+isOutdoor);
        Isoutdoor = isOutdoor;

    }

    // Chase the player continuously until stopped or killed
    IEnumerator ChasePlayer()
    {
        agent.speed = normalSpeed;
        animator.SetBool("isMove", true);

        while (true)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            agent.SetDestination(player.position);
            yield return null;
        }
    }

    // Attack the player when within attack range
    IEnumerator AttackPlayer()
    {
        agent.speed = normalSpeed;

        // Face the player and stop moving
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Stop moving when close enough to attack
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

        // Continue chasing or attacking based on distance
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
    }

    void Die()
    {
        isDead = true;
        StopAllCoroutines();
        animator.SetTrigger("Dead");
        GetComponent<NavMeshAgent>().enabled = false;

        FindObjectOfType<AnimalSpawnerScript>().OnAnimalDied();

      
        for (int i = 0; i < meatCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-meatSpawnRadius, meatSpawnRadius), 0, Random.Range(-meatSpawnRadius, meatSpawnRadius));
            Vector3 spawnPosition = transform.position + randomOffset;
            Instantiate(meatPrefabName, spawnPosition, Quaternion.identity);
        }

        Invoke("DestroyAnimal", 4.0f);
    }

    // Handle weapon collision 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            TakeDamage(other.GetComponent<WeaponDamage>().weapon.damage);

        }
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

    // Teleport the predator to the spawn point if the player is indoors
    private void TeleportToSpawnPoint()
    {
        StopAllCoroutines();
        agent.SetDestination(spawnPoint.position);
        animator.SetBool("isMove", true);

        isTeleported = true;
        Destroy(gameObject, 10f); 
    }
}
