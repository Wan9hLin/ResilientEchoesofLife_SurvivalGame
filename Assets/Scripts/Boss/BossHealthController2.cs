using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthController2 : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

 

    // 添加UI相关的引用，比如血条Slider或Text等，这里假设使用Slider来表示血量
    public Slider healthSlider;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }



    // Update is called once per frame
    void Update()
    {
        
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // 计算血量百分比，用于更新Slider的值
        float healthPercentage = (float)currentHealth / maxHealth;
        healthSlider.value = healthPercentage;
    }

    public void TakeDamage(int damageAmount)
    {
        // 扣除血量
        currentHealth -= damageAmount;
        //BossController.instance.anim.SetTrigger("GetHit");

        // 确保血量不会小于0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // 更新血量UI
        UpdateHealthUI();




    }
}
