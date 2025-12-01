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
    public Transform[] wanderPoints;  // 新增一个存储漫步点的数组

    private float normalSpeed = 6.5f;
    public float wanderSpeed = 1.0f;  // 新增一个用于设置漫步速度的变量

    private bool isDead = false;
    private bool isProvoked = false;  // 新增一个表示动物是否被激怒的布尔值

    public float attackStopDistance = 2.0f;
    public GameObject bloodFXPrefab;
    public delegate void AnimalDeathHandler();
    public event AnimalDeathHandler OnDeath;


    private AttackColliderScript attackColliderScript;
    private AttackColliderScript attackColliderScript_2;
    private SwordWave swordWave;
    private WhipFX whipFX;


    private bool isChaseCoroutineRunning = false;  // 新增一个变量，用来表示 ChasePlayer 协程是否正在执行
    private bool isAttackCoroutineRunning = false;
    private bool isFireBreathCoroutineRunning = false;
    private bool isWhipAttackCoroutineRunning = false;
    


    private int meleeAttackCount = 0;
    private int maxMeleeAttackCount = 2;  // 近战攻击的最大次数，可以根据需要修改
    private int fireBreathCount = 0;
    private int maxFireBreathCount = 2;  // 吐火攻击的最大次数，可以根据需要修改
    private int maxWhipAttackCount = 2;
    private int whipAttackCount = 0;
    public Transform fireBreathPoint;  // Boss的嘴部
    public GameObject firePrefab;  // 火的Prefab

    public float whipAttackDistance = 10.0f;  // 鞭子攻击的距离

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
       // wanderPoints = WanderPointsManager.Instance.wanderPoints;
        StartCoroutine(Wander());

        player = GameObject.FindWithTag("Player").transform;

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
                if (!isWandering)
                {
                    StartCoroutine(Wander());
                }
                else if (isProvoked)
                {
                    state = BossState.Chase;
                }
                break;
            case BossState.Chase:
                if (!isChaseCoroutineRunning)
                {
                    StartCoroutine(ChasePlayer());
                }
                else if (distanceToPlayer <= attackDistance)
                {
                    state = BossState.MeleeAttack;
                }
                else if (distanceToPlayer > safeDistance)
                {
                    state = BossState.Wander;
                }
                break;
            case BossState.MeleeAttack:
                if (!isAttackCoroutineRunning)
                {
                    StartCoroutine(MeleeAttackPlayer());
                }
                else if (distanceToPlayer > attackDistance)
                {
                    state = BossState.Chase;
                }
                else if (meleeAttackCount >= maxMeleeAttackCount)
                {
                    meleeAttackCount = 0;
                    state = BossState.FireBreath;
                }
                break;
            case BossState.FireBreath:
                if (!isFireBreathCoroutineRunning)
                {
                    StartCoroutine(FireBreathAttackPlayer());
                }
                else if (distanceToPlayer > attackDistance)
                {
                    state = BossState.Chase;
                }
                else if (fireBreathCount >= maxFireBreathCount)
                {
                    fireBreathCount = 0;
                    state = BossState.WhipAttack;
                }
                break;

            case BossState.WhipAttack:
                if (!isWhipAttackCoroutineRunning)
                {
                    StartCoroutine(WhipAttackPlayer());
                }
                else if (distanceToPlayer > attackDistance)
                {
                    state = BossState.Chase;
                }
                else if (whipAttackCount >= maxWhipAttackCount)
                {
                    whipAttackCount = 0;
                    state = BossState.MeleeAttack;
                }
                break;

           

        }
    }

 

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

        while (agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        animator.SetBool("isWalk", false);
        isWandering = false;
    }

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

        yield return new WaitForSeconds(2.0f); // 调整等待时间，让攻击间隔更长
        isAttackCoroutineRunning = false;
    }

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

        meleeAttackCount = 0;  // 吐火攻击后重置近战攻击次数

        yield return new WaitForSeconds(2.0f); // 调整等待时间，让攻击间隔更长
        isFireBreathCoroutineRunning = false;
    }

    IEnumerator WhipAttackPlayer()
    {
        isWhipAttackCoroutineRunning = true;
        agent.speed = normalSpeed;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (Vector3.Distance(player.position, transform.position) <= whipAttackDistance)  // 使用鞭子攻击的距离进行判断
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }

        animator.SetTrigger("WhipAttack");
        whipAttackCount++;
        yield return new WaitForSeconds(1.0f);

        yield return new WaitForSeconds(2.0f); // 调整等待时间，让攻击间隔更长

        isWhipAttackCoroutineRunning = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)  // 如果动物已经死亡，直接返回，不执行后续的代码
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
        // 计算血量百分比，用于更新Slider的值
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
