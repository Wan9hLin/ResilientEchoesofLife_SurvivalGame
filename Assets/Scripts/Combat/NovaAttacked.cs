using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaAttacked : MonoBehaviour
{
    private Collider trigger;

    // Start is called before the first frame update
    void Start()
    {
        trigger = GetComponent<Collider>();  // 获取当前游戏对象上的 Collider 组件
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            other.GetComponent<PlayerStatsManager>().TakeDamage(2);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
