using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenerAnimalController_2 : MonoBehaviour
{
    public Transform player;
    public float alertDistance = 10.0f;
    public float safeDistance = 20.0f;
    public float wanderRadius = 10.0f;

    public float runDuration = 5.0f;
    public float slowdownDuration = 3.0f;
    public float restDuration = 5.0f;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isResting = false;

    public int maxHealth = 3;
    public int currentHealth;

    private bool isWandering = false;
    private Vector3 wanderDestination;
    public Transform[] wanderPoints;  

    private float normalSpeed = 3.5f;
    public float wanderSpeed = 1.0f;  

    private bool isDead = false;

    public GameObject meatPrefabName;  
    public int meatCount = 3;  
    public float meatSpawnRadius = 1f; 

    public GameObject bloodFXPrefab;

    public delegate void AnimalDeathHandler();
    public event AnimalDeathHandler OnDeath;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        animator.SetBool("isMove", false);
        animator.SetBool("isResting", false);

        currentHealth = maxHealth;
        agent.speed = wanderSpeed;
        wanderPoints = WanderPointsManager.Instance.wanderPoints;
        StartCoroutine(Wander());
        

        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {

        if (isDead)  // Skip updates if the animal is dead
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // Start running if the player is too close
        if (distanceToPlayer < alertDistance && !isResting)
        {
            isWandering = false;
            StopCoroutine(Wander());
            StartCoroutine(RunAndRest());
        }

        else if (distanceToPlayer > safeDistance)
        {
            animator.SetBool("isMove", false);
            StartCoroutine(Wander());
        }

    }


    IEnumerator RunAndRest()
    {

        if (isDead) 
        {
            yield break;
        }

        agent.speed = normalSpeed;
        StopCoroutine(Wander());
        isResting = true;
        animator.SetBool("isMove", true);
        animator.SetBool("isWalk", false);

        // Run away from the player
        Vector3 dirToPlayer = (transform.position - player.position).normalized;
        Vector3 randomDirection = transform.position + dirToPlayer * agent.speed * runDuration;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, agent.speed * runDuration, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
        yield return new WaitForSeconds(runDuration);

        // Gradually slow down
        float originalSpeed = agent.speed;
        for (float t = 0; t < slowdownDuration; t += Time.deltaTime)
        {
            agent.speed = Mathf.Lerp(originalSpeed, 0, t / slowdownDuration);
            yield return null;
        }

        // Enter resting phase
        animator.SetBool("isResting", true);
        yield return new WaitForSeconds(restDuration);
        animator.SetBool("isResting", false);

        agent.speed = normalSpeed; 
        isResting = false;

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
            if (isResting)
            {
                isResting = false;
            }

            // Select a random wander point
            int index = Random.Range(0, wanderPoints.Length);
            wanderDestination = wanderPoints[index].position;

            agent.SetDestination(wanderDestination);
            animator.SetBool("isWalk", true);

            // Wait until the animal reaches its destination
            while (agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            animator.SetBool("isWalk", false);

            // Wait for a bit before moving to the next destination
            yield return new WaitForSeconds(1f);
        }

        agent.speed = normalSpeed;
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
        OnDeath?.Invoke();
        StopAllCoroutines();  
        animator.SetTrigger("Dead");  
        GetComponent<NavMeshAgent>().enabled = false;  // Disable movement

        // Notify spawner of death
        FindObjectOfType<AnimalSpawnerScript>().OnAnimalDied();

        
        for (int i = 0; i < meatCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-meatSpawnRadius, meatSpawnRadius), 0, Random.Range(-meatSpawnRadius, meatSpawnRadius));
            Vector3 spawnPosition = transform.position + randomOffset;
            Instantiate(meatPrefabName, spawnPosition, Quaternion.identity);
        }

        Invoke("DestroyAnimal", 4.0f);
    }


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
}