using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GenerAnimalController : MonoBehaviour
{
    public Transform player;
    public float alertDistance = 10.0f;
    public float safeDistance = 20.0f;

    public float runDuration = 5.0f;
    public float slowdownDuration = 3.0f;
    public float restDuration = 5.0f;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isResting = false;

    public int maxHealth = 3;  // 动物的最大生命值
    public int currentHealth;  // 动物当前的生命值

    private bool isDead = false;
    private bool isRunningCoroutine = false;
    private bool needToRun = false;  // 添加一个标记

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

        if (distanceToPlayer < alertDistance && !isResting && !isRunningCoroutine)  // 检查 isRunningCoroutine
        {
            needToRun = true;  // 设置标记为 true
        }
        else if (distanceToPlayer > safeDistance)
        {
            animator.SetBool("isMove", false);
            agent.SetDestination(transform.position);
        }

        // 在 Update 函数中检查标记，如果它是 true，则立即开始逃跑
        if (needToRun)
        {
            StartCoroutine(RunAndRest());
            needToRun = false;  // 清除标记
        }


    }

    IEnumerator RunAndRest()
    {
        isRunningCoroutine = true;

        if (isDead)  // 如果动物已经死亡，直接返回，不执行后续的代码
        {
            yield break;
        }

        isResting = true;
        animator.SetBool("isMove", true);

        // Run for a while
        Vector3 dirToPlayer = (transform.position - player.position).normalized;
        Vector3 randomDirection = transform.position + dirToPlayer * agent.speed * runDuration;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, agent.speed * runDuration, NavMesh.AllAreas);
        agent.SetDestination(hit.position);

        float elapsedRunTime = 0;
        while (elapsedRunTime < runDuration)
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            if (distanceToPlayer < alertDistance)  // 如果玩家进入警戒范围
            {
                break;  // 立即中断逃跑，开始新的逃跑
            }
            elapsedRunTime += Time.deltaTime;
            yield return null;
        }

        // 这里添加你的 slowdown 和 rest 的代码
        // ...

        isResting = false;
        isRunningCoroutine = false;  // 将 isRunningCoroutine 设置为 false
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
        // 这里可以添加其他的死亡逻辑，比如播放死亡音效、生成掉落物等

        
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
