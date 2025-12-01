using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // Define Boss's states
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
        ChangeState(BossState.FireBall);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        FlyAround();
        randomnum = Random.Range(1, 4);
    }

    private void Update()
    {
        // Handle different states and their behavior
        switch (currentState)
        {
            case BossState.FireBall:
                // Handle Fireball attacks
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
                // Handle Firebreathing attack
                FireThrower.SetActive(true);
                FireThrower.transform.LookAt(playerTransform);
                break;

            case BossState.Poison:
                // Handle Poison attacks
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
    }

    // Make the boss fly around the player
    private void FlyAround()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0; 
        transform.rotation = Quaternion.LookRotation(direction);
    }

    // Throw the specified attack skill (fireball or poison)
    private void ThrowSkills(GameObject skillObject)
    {
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
