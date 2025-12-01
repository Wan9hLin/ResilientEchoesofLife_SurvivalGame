using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    public float speed = 10f;  // 骨矛的速度

    public Transform Head;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction)
    {
        rb.velocity = direction * speed;
        Vector3 directionToPlayer = direction;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        Head.rotation = targetRotation;
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //BossController boss = transform.parent.GetComponent<BossController>();
            //boss.TakeDamage(10);  // 假设对Boss造成10点伤害
            Debug.Log("Player Hurt!");
        }
        else if (!collision.gameObject.CompareTag("Spear"))
        {
            StickIntoGround();
        }
    }

    private void StickIntoGround()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        // 骨矛插入地面的逻辑
        // ...
    }
}
