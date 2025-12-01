using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliderScript : MonoBehaviour
{
    
    private Collider trigger;

   
    private void Start()
    {
        trigger = GetComponent<Collider>();  // 获取当前游戏对象上的 Collider 组件
        trigger.enabled = false;  // 刚开始将触发器设为不激活
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // PlayerAttack playerAttack = other.gameObject.GetComponent<PlayerAttack>();
            //playerAttack.TakeDamage(1);
            other.GetComponent<PlayerStatsManager>().TakeDamage(5);
        }
    }

    public void SetTriggerActive()
    {
        trigger.enabled = true;
    }

    public void SetTriggerDeActive()
    {
        trigger.enabled = false;
    }

}
