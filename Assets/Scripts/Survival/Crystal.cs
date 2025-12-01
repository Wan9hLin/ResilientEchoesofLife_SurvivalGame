using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    [SerializeField] private GameObject fxSpark; // 挖矿效果
    [SerializeField] private GameObject fxDestruction; // 矿石被挖完的效果
    [SerializeField] private GameObject minePrefab; // 矿石掉落的物品

    private int health = 3; // 矿石需要被挖掘的次数

    private void OnTriggerEnter(Collider other)
    {
        // 只有使用"Hammer"武器才可以挖矿
        if (other.name == "Axe")
        {
            
            MineOre(other);
        }
    }

    private void MineOre(Collider hammer)
    {
        health -= 1;


        AudioManager.instance.PlaySFX("Mining");

        // 挖矿特效出现在锤子碰撞的位置
        Vector3 hitPoint = hammer.ClosestPoint(transform.position);
        Instantiate(fxSpark, hitPoint, Quaternion.identity);

        if (health <= 0)
        {
            // 矿石被挖掘完，播放销毁特效
            Instantiate(fxDestruction, transform.position, Quaternion.identity);

            // 掉落物品
            Instantiate(minePrefab, transform.position, Quaternion.identity);

            // 销毁矿石
            Destroy(gameObject);
        }
    }
}
