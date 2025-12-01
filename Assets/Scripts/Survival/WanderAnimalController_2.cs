using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderAnimalController_2 : MonoBehaviour
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
    private int currentHealth;

    private bool isWandering = false;
    private Vector3 wanderDestination;
    public Transform[] wanderPoints;  // 新增一个存储漫步点的数组

    private float normalSpeed = 3.5f;
    public float wanderSpeed = 1.0f;  // 新增一个用于设置漫步速度的变量

    private bool isDead = false;

    public GameObject meatPrefabName;  // 肉 Prefab 的名称
    public int meatCount = 3;  // 动物死亡时掉落的肉的数量
    public float meatSpawnRadius = 1f; // 肉生成的半径
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
        wanderPoints = WanderPointManager_3.Instance.wanderPoints;
        StartCoroutine(Wander());


        // 找到标签为"Player"的游戏对象，并获取其Transform组件
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {

        if (isDead)  // 如果动物已经死亡，直接返回，不执行后续的代码
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

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

        if (isDead)  // 如果动物已经死亡，直接返回，不执行后续的代码
        {
            yield break;
        }

        agent.speed = normalSpeed;
        StopCoroutine(Wander());
        isResting = true;
        animator.SetBool("isMove", true);
        animator.SetBool("isWalk", false);

        // Run for a while
        Vector3 dirToPlayer = (transform.position - player.position).normalized;
        Vector3 randomDirection = transform.position + dirToPlayer * agent.speed * runDuration;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, agent.speed * runDuration, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
        yield return new WaitForSeconds(runDuration);

        // Slowdown
        float originalSpeed = agent.speed;
        for (float t = 0; t < slowdownDuration; t += Time.deltaTime)
        {
            agent.speed = Mathf.Lerp(originalSpeed, 0, t / slowdownDuration);
            yield return null;
        }

        // Rest and play animation
        animator.SetBool("isResting", true);
        yield return new WaitForSeconds(restDuration);
        animator.SetBool("isResting", false);

        agent.speed = normalSpeed;  // 将速度设置回normalSpeed
        isResting = false;

        StartCoroutine(Wander());
    }



    IEnumerator Wander()
    {

        if (isDead)  // 如果动物已经死亡，直接返回，不执行后续的代码
        {
            yield break;
        }

        isWandering = true;
        agent.speed = wanderSpeed;

        while (isWandering)
        {
            // 在开始新的漫步前，确保动物已准备好再次逃跑
            if (isResting)
            {
                isResting = false;
            }

            // 随机选择一个漫步点作为目的地
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
        if (isDead)  // 如果动物已经死亡，直接返回，不执行后续的代码
        {
            return;
        }
        currentHealth -= damage;
        Instantiate(bloodFXPrefab, transform.position, Quaternion.identity);
        AudioManager.instance.PlaySFX("CutMeat");
        animator.SetTrigger("Hit");  // 播放被攻击动画

        if (currentHealth <= 0)
        {
            Die();  // 如果动物的生命值降到0或以下，动物就死亡
        }
    }

    void Die()
    {
        isDead = true;  // 当动物死亡时，将isDead设置为true
        OnDeath?.Invoke();
        StopAllCoroutines();  // 停止所有的协程
        animator.SetTrigger("Dead");  // 播放死亡动画
        GetComponent<NavMeshAgent>().enabled = false;  // 禁用动物的NavMeshAgent，让动物停止移动

        FindObjectOfType<AnimalSpawnerScript>().OnAnimalDied();

     
        for (int i = 0; i < meatCount; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-meatSpawnRadius, meatSpawnRadius), 0, Random.Range(-meatSpawnRadius, meatSpawnRadius));
            Vector3 spawnPosition = transform.position + randomOffset;
            Instantiate(meatPrefabName, spawnPosition, Quaternion.identity);
        }

        // 在2秒后调用DestroyAnimal函数来销毁动物的游戏对象
        Invoke("DestroyAnimal", 4.0f);
    }

    void DestroyAnimal()
    {
        Destroy(gameObject);  // 销毁动物的游戏对象
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            TakeDamage(other.GetComponent<WeaponDamage>().weapon.damage);

        }
    }
}
