using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // 定义Boss的状态枚举
    public enum BossState
    {
        FireBall,
        FireThrower,
        Poison
    }

    public static BossController instance = null;

    public GameObject fireballPrefab;
    public GameObject PoisonPrefab;
    public GameObject FireThrower;

    private Transform playerTransform;
    public Transform firepoint;
    public Animator anim;

    private BossState currentState;
    public int attackcount;
    public int randomnum;
    public bool isFiring = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 初始化Boss的状态
        ChangeState(BossState.FireBall);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        FlyAround();
        randomnum = Random.Range(1, 4);
    }

    private void Update()
    {


        switch (currentState)
        {
            case BossState.FireBall:
                if (attackcount < randomnum && !isFiring)
                {
                    ThrowSkills(fireballPrefab);
                    anim.SetTrigger("Fireball");
                    attackcount++;
                    isFiring = true;
                    StartCoroutine(AttackDelay());
                }
                else if (attackcount == randomnum)
                {
                    ChangeState(BossState.FireBall);
                    randomnum = Random.Range(1, 4);
                    attackcount = 0;
                }



                break;

            case BossState.FireThrower:





                FireThrower.SetActive(true);
                FireThrower.transform.LookAt(playerTransform);

                //anim.SetBool("FireThrower",true);


                break;

            case BossState.Poison:

                if (attackcount < randomnum && !isFiring)
                {
                    ThrowSkills(PoisonPrefab);
                    anim.SetTrigger("Lightning");
                    attackcount++;
                    isFiring = true;
                    StartCoroutine(AttackDelay());
                }
                else if (attackcount == randomnum)
                {
                    ChangeState(BossState.FireBall);
                    randomnum = Random.Range(1, 4);
                    attackcount = 0;
                }
                break;
        }
    }



    public void ChangeState(BossState newState)
    {
        currentState = newState;
        // 在这里你可以根据不同的状态切换Boss的动画、外观等
    }

    private void FlyAround()
    {
        // Boss朝向玩家的方向向量
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0; // 可选：将方向向量的y分量设为0，保持Boss在水平方向上朝向玩家

        // 使用LookAt方法使Boss朝向玩家
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void ThrowSkills(GameObject skillObject)
    {


        /*
                // 获取朝向玩家的方向向量
                Vector3 directionToPlayer = playerTransform.position+new Vector3(0,1f,0) - firepoint.position;

                // 实例化火球预制体，并设置朝向为Boss到玩家的方向向量
                GameObject fireballInstance = Instantiate(skillObject, firepoint.position, Quaternion.LookRotation(directionToPlayer));
                fireballInstance.transform.LookAt(playerTransform);

                Debug.DrawRay(firepoint.position, directionToPlayer, Color.blue, 100.0f);

                // 发射火球，你可以使用Rigidbody或其他方法来使火球移动
                Rigidbody fireballRb = fireballInstance.GetComponent<Rigidbody>();
                if (fireballRb != null)
                {
                    StartCoroutine(AttackDelay());
                    fireballRb.AddForce(directionToPlayer.normalized * 300, ForceMode.Impulse);

                }*/


        Vector3 directionToPlayer = playerTransform.position + new Vector3(0, 1f, 0) - firepoint.position;
        GameObject fireballInstance = Instantiate(skillObject, firepoint.position, Quaternion.identity);
        fireballInstance.GetComponent<RemoteAttack>().target = playerTransform;

        Debug.DrawRay(firepoint.position, directionToPlayer, Color.blue, 100.0f);


    }

    private IEnumerator AttackDelay()
    {
        // Delay the attack for 5 seconds
        yield return new WaitForSeconds(5f);

        isFiring = false;
    }




}
