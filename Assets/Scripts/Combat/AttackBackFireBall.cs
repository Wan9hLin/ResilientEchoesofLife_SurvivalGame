using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBackFireBall : MonoBehaviour
{
    private Rigidbody rb;
    private bool isAttackByPlayer=false;
    private Transform bossPosition;
    private int hitTimes=0;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        bossPosition = GameObject.FindGameObjectWithTag("RealBossFirePoint").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttackByPlayer) return;
 
        Vector3 directionToTarget = bossPosition.position - transform.position;
        directionToTarget.Normalize();
        rb.velocity = directionToTarget * 100f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Shield")
        {
            hitTimes++;
            if (hitTimes >=2)
            {   
                //add sound attack back 
                rb = GetComponent<Rigidbody>();
                isAttackByPlayer = true;
                rb.AddForce(Vector3.up*100f,ForceMode.Impulse);
            }
           
        }

        if (other.CompareTag("RealBoss"))
        {
            Debug.Log("damage boss");
            // add sound  hit boss
            other.GetComponent<BossHealthController>().TakeDamage(damage);
            Destroy(gameObject, 1f);
        }
    }
}
