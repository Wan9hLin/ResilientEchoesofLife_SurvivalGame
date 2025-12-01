using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HungerBar : MonoBehaviour
{
    public Slider hungerSlider;
    public TextMeshProUGUI hungerText;

    private int maxHunger;
    public static HungerBar Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        maxHunger = (int)PlayerStatsManager.Instance.maxHunger;
        hungerSlider.maxValue = maxHunger;
        UpdateHungerBar(maxHunger);
    }

    public void UpdateHungerBar(float currentValue)
    {
        hungerSlider.value = currentValue;
        hungerText.text = currentValue + " / " + maxHunger;
    }
    
    public void ResetHurger()
    {
        Debug.Log("hp"+maxHunger);
        UpdateHungerBar(maxHunger);
    }
   
}