using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRemain : MonoBehaviour
{
    public float damageInterval = 1f; // 伤害间隔
    public int damageAmount = 2; // 每次伤害数量

    private float timer = 0f;


    void Update()
    {
        // 如果玩家已经进入火焰区域
        if (PlayerStatsManager.Instance.isfiring)
        {
            timer += Time.deltaTime;

            if (timer >= damageInterval)
            {
                // 达到伤害间隔，进行伤害逻辑
                PlayerStatsManager.Instance.isfiring = false;
                timer = 0f; // 重置计时器
            }
        }
    }

    // 当玩家进入触发器区域时开始计时
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !PlayerStatsManager.Instance.isfiring)
        {

            // 玩家第一次进入火焰区域直接扣除血量
            PlayerStatsManager.Instance.TakeDamage(damageAmount);
            PlayerStatsManager.Instance.isfiring = true;
            

            // 玩家进入火焰区域开始计时
            timer = 0f;

        }
    }

    // 当玩家离开触发器区域时停止计时
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 玩家离开火焰区域停止计时
            PlayerStatsManager.Instance.isfiring = false;
            timer = 0f; // 重置计时器
        }
    }
}
