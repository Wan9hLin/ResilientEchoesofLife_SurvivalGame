using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthController : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public GameObject bosstimeline2;
    public GameObject bossfighttrigger;

    public Slider healthSlider;

    public static BossHealthController instance = null;

    private void Awake()
    {
        instance = this;
    }

   
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }


    void Update()
    {
        // If the boss's health falls below 50%, trigger the timeline for phase 2
        if (currentHealth < 50)
        {
            bosstimeline2.SetActive(true);
            bossfighttrigger.SetActive(false);
        }

        UpdateHealthUI();
    }

    // Updates the health UI to reflect the current health percentage
    private void UpdateHealthUI()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        healthSlider.value = healthPercentage;
    }

    // Handles taking damage and updating health
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        BossController.instance.anim.SetTrigger("GetHit");

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

    }
}
