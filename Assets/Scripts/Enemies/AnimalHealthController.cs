using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalHealthController : MonoBehaviour
{
    public int maxHealth = 1;
    public int currentHealth = 1;
    public bool isDead = false;
    private NavMeshAgent navAgent;
    private Animator animator;
    private Vector3 fixedPosition;
    public GameObject bloodFXPrefab;
    //public GameObject meatPrefabName;  // 肉 Prefab 的名称
    public int meatCount = 3;  // 动物死亡时掉落的肉的数量
    public float meatSpawnRadius = 1f; // 肉生成的半径
    private AttackColliderScript attackColliderScript;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        attackColliderScript = transform.Find("AttackCollider").GetComponent<AttackColliderScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            transform.position = fixedPosition;
        }
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //animator.SetTrigger("Hit");  // 播放被攻击动画
        Instantiate(bloodFXPrefab, transform.position, Quaternion.identity);
        AudioManager.instance.PlaySFX("CutMeat");


        if (currentHealth <= 0 && !isDead)
        {
            Die();  // 如果动物的生命值降到0或以下，动物就死亡
            isDead = true;
        }
    }



    void Die()
    {

        fixedPosition = transform.position;
        animator.SetTrigger("Die");  // 播放死亡动画
                                     //GetComponent<NavMeshAgent>().enabled = false;  // 禁用动物的NavMeshAgent，让动物停止移动
                                     //GetComponent<BehaviorTree>().enabled = false;


      // for (int i = 0; i < meatCount; i++)
      // {
      //     Vector3 randomOffset = new Vector3(Random.Range(-meatSpawnRadius, meatSpawnRadius), 0, Random.Range(-meatSpawnRadius, meatSpawnRadius));
      //     Vector3 spawnPosition = transform.position + randomOffset;
      //     Instantiate(meatPrefabName, spawnPosition, Quaternion.identity);
      // }

        // 在4秒后调用DestroyAnimal函数来销毁动物的游戏对象
        Invoke("DestroyAnimal", 4.0f);
    }


    void DestroyAnimal()
    {
        Destroy(gameObject);  // 销毁动物的游戏对象
    }

  public void SetTriggerActive()
  {
      attackColliderScript.SetTriggerActive();
  }
 
  public void SetTriggerDeActive()
  {
      attackColliderScript.SetTriggerDeActive();
  }
}
