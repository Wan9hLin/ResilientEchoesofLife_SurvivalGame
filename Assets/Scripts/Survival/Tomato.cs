using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomato : MonoBehaviour
{
   // [SerializeField] private GameObject fxSpark; 
    [SerializeField] private GameObject fxDestruction; 
    [SerializeField] private GameObject totmatoPrefab; 

    private int health = 2; // 被挖掘的次数

    private void OnTriggerEnter(Collider other)
    {
        // 只有使用"Hammer"武器才可以挖矿
        if (other.name == "Punch")
        {
          
            TomatoOre(other);
        }
    }

    private void TomatoOre(Collider hammer)
    {
        health -= 1;

        // 挖矿特效出现在锤子碰撞的位置
        Vector3 hitPoint = hammer.ClosestPoint(transform.position);
       // Instantiate(fxSpark, hitPoint, Quaternion.identity);

        if (health <= 0)
        {
            AudioManager.instance.PlaySFX("TomatoDrop");
            Instantiate(fxDestruction, transform.position, Quaternion.identity);

            int dropCount = Random.Range(1, 4); // 生成一个1到3的随机数（包含1，但不包含4）
            for (int i = 0; i < dropCount; i++)
            {
                Instantiate(totmatoPrefab, transform.position, Quaternion.identity);
            }

            // 销毁矿石
            Destroy(gameObject);
        }
    }
}
