using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarmthBar : MonoBehaviour
{
    public Slider warmthSlider;
    public TextMeshProUGUI warmthText;

    private int maxWarmth;
    public static WarmthBar Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        maxWarmth = (int)PlayerStatsManager.Instance.maxWarmth;
        warmthSlider.maxValue = maxWarmth;
        UpdateWarmthBar(maxWarmth);
    }

    public void UpdateWarmthBar(float currentWarmth)
    {
        warmthSlider.value = currentWarmth;
        warmthText.text = currentWarmth + " / " + maxWarmth;
    }

    public void ResetWarmth()
    {   
        Debug.Log("hp"+maxWarmth);
        UpdateWarmthBar(maxWarmth);
    }
}