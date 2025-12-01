using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TextMeshProUGUI healthText; 

    private int maxHealth;
    public static HealthBar Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        maxHealth = (int)PlayerStatsManager.Instance.maxHp;
        healthSlider.maxValue = maxHealth;
        UpdateHealthBar(maxHealth);
    }

    public void UpdateHealthBar(float currentValue)
    {
        healthSlider.value = currentValue;
        healthText.text = currentValue + " / " + maxHealth; 
    }
    public void ResetHp()
    {
        Debug.Log("hp"+maxHealth);
        UpdateHealthBar(maxHealth);
    }
    

}



