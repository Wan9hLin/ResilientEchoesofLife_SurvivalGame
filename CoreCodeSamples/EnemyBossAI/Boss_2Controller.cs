using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss_2Controller : MonoBehaviour
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

    private float normalSpeed = 6.5f;
    public float wanderSpeed = 1.0f;  

    private bool isDead = false;
    private bool isProvoked = false;  

    public float attackStopDistance = 2.0f;
    public GameObject bloodFXPrefab;
    public delegate void AnimalDeathHandler();
    public event AnimalDeathHandler OnDeath;

    private AttackColliderScript attackColliderScript;
    private AttackColliderScript attackColliderScript_2;
    private SwordWave swordWave;
    private WhipFX whipFX;

    private bool isChaseCoroutineRunning = false; 
    private bool isAttackCoroutineRunning = false;
    private bool isFireBreathCoroutineRunning = false;
    private bool isWhipAttackCoroutineRunning = false;
    

    private int meleeAttackCount = 0;
    private int maxMeleeAttackCount = 2; 
    private int fireBreathCount = 0;
    private int maxFireBreathCount = 2;  
    private int maxWhipAttackCount = 2;
    private int whipAttackCount = 0;
    public Transform fireBreathPoint;  
    public GameObject firePrefab; 

    public float whipAttackDistance = 10.0f; // Distance for whip attack

    public Slider healthSlider;


    private enum BossState
    {
        Wander,
        Chase,
        MeleeAttack,
        FireBreath,
        WhipAttack,
        Cooldown,
    }

    private BossState state = BossState.Wander;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("isMove", false);

        currentHealth = maxHealth;
        agent.speed = wanderSpeed;
        StartCoroutine(Wander());

        player = GameObject.FindWithTag("Player").transform;

        // Initialize attack colliders and effects
        attackColliderScript = transform.Find("AttackCollider").GetComponent<AttackColliderScript>();
        attackColliderScript_2 = transform.Find("AttackCollider_2").GetComponent<AttackColliderScript>();
        swordWave = transform.Find("AttackCollider").GetComponent<SwordWave>();
        whipFX = transform.Find("AttackCollider").GetComponent<WhipFX>();
        UpdateHealthUI();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        UpdateHealthUI();

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (state)
        {
            case BossState.Wander:
                if (!isWandering) StartCoroutine(Wander());
                else if (isProvoked) state = BossState.Chase;
                break;

            case BossState.Chase:
                if (!isChaseCoroutineRunning) StartCoroutine(ChasePlayer());
                else if (distanceToPlayer <= attackDistance) state = BossState.MeleeAttack;
                else if (distanceToPlayer > safeDistance) state = BossState.Wander;
                break;

            case BossState.MeleeAttack:
                if (!isAttackCoroutineRunning) StartCoroutine(MeleeAttackPlayer());
                else if (distanceToPlayer > attackDistance) state = BossState.Chase;
                else if (meleeAttackCount >= maxMeleeAttackCount)
                {
                    meleeAttackCount = 0;
                    state = BossState.FireBreath;
                }
                break;

            case BossState.FireBreath:
                if (!isFireBreathCoroutineRunning) StartCoroutine(FireBreathAttackPlayer());
                else if (distanceToPlayer > attackDistance) state = BossState.Chase;
                else if (fireBreathCount >= maxFireBreathCount)
                {
                    fireBreathCount = 0;
                    state = BossState.WhipAttack;
                }
                break;

            case BossState.WhipAttack:
                if (!isWhipAttackCoroutineRunning) StartCoroutine(WhipAttackPlayer());
                else if (distanceToPlayer > attackDistance) state = BossState.Chase;
                else if (whipAttackCount >= maxWhipAttackCount)
                {
                    whipAttackCount = 0;
                    state = BossState.MeleeAttack;
                }
                break;
        }
    }

    // Coroutine to make the boss wander to random points
    IEnumerator Wander()
    {
        if (isDead)
        {
            yield break;
        }

        isWandering = true;
        agent.speed = wanderSpeed;

        int index = Random.Range(0, wanderPoints.Length);
        wanderDestination = wanderPoints[index].position;

        agent.SetDestination(wanderDestination);
        animator.SetBool("isWalk", true);

        // Wait until the boss reaches the destination
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        animator.SetBool("isWalk", false);
        isWandering = false;
    }

    // Coroutine to chase the player
    IEnumerator ChasePlayer()
    {
        isChaseCoroutineRunning = true;
        agent.speed = normalSpeed;
        animator.SetBool("isMove", true);
        animator.SetBool("isWalk", false);

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        agent.SetDestination(player.position);
        yield return null;

        //  animator.SetBool("isMove", false);
        isChaseCoroutineRunning = false;
    }

    // Coroutine to perform melee attack
    IEnumerator MeleeAttackPlayer()
    {
        isAttackCoroutineRunning = true;
        agent.speed = normalSpeed;

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
      
        meleeAttackCount++;
        yield return new WaitForSeconds(1.0f);

        yield return new WaitForSeconds(2.0f);
        isAttackCoroutineRunning = false;
    }

    // Coroutine to perform fire breath attack
    IEnumerator FireBreathAttackPlayer()
    {
        isFireBreathCoroutineRunning = true;
        agent.speed = normalSpeed;

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

        animator.SetTrigger("FireBreath");
        fireBreathCount++;
        yield return new WaitForSeconds(1.0f);

        meleeAttackCount = 0;   // Reset melee attack count after fire breath

        yield return new WaitForSeconds(2.0f); 
        isFireBreathCoroutineRunning = false;
    }

    // Coroutine to perform whip attack
    IEnumerator WhipAttackPlayer()
    {
        isWhipAttackCoroutineRunning = true;
        agent.speed = normalSpeed;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (Vector3.Distance(player.position, transform.position) <= whipAttackDistance)  
        {
            agent.isStopped = true; // Stop moving during whip attack
        }
        else
        {
            agent.isStopped = false;
        }

        animator.SetTrigger("WhipAttack");
        whipAttackCount++;
        yield return new WaitForSeconds(1.0f);

        yield return new WaitForSeconds(2.0f); 

        isWhipAttackCoroutineRunning = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) 
        {
            return;
        }
        UpdateHealthUI();
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

    public void SetTriggerActive_2()
    {
        attackColliderScript_2.SetTriggerActive();
    }

    public void SetTriggerDeActive_2()
    {
        attackColliderScript_2.SetTriggerDeActive();
    }

    public void SummonSwordWave()
    {
        swordWave.SummonSwordWave();
    }
    public void FireBreath()
    {
       
        Instantiate(firePrefab, fireBreathPoint.position, fireBreathPoint.rotation);
    }


    public void SpawnFireEffect()
    {
        whipFX.SpawnFireEffect();
    }
    private void UpdateHealthUI()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        healthSlider.value = healthPercentage;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            TakeDamage(other.GetComponent<WeaponDamage>().weapon.damage);
        }
    }

    public void SwordFXAudio()
    {
        BossAudioManager.instance.PlaySFX("SwrodSFX");
    }

    public void PenhuoFXAudio()
    {
        BossAudioManager.instance.PlaySFX("PenhuoFX");
    }
}
