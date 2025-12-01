using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int slotID; // �ո�ID ������ƷID  mybag index
    public Item slotItem;
    public Image slotImage;
    public Text slotNum;
    public string slotInfo;
    public GameObject itemInSlot;
    public GameObject slotHilight;
    public Item punch;

    public GameObject CraftEffectPrefab;


    public void ItemOnClick()
    {
        InventoryManager.UpdateItemInfo(slotInfo);
    }

    public void SetupSlot(Item item)
    {
        if(item == null)
        {
            itemInSlot.SetActive(false);
            return;
        }

        slotItem = item;
        slotImage.sprite = item.itemImage;
        slotNum.text = item.itemHold.ToString();
        slotInfo = item.itemInfo;
    }
    public void SetHilightSlot()
    {
        
        //Debug.Log("@@@"+slotID);
        if (slotHilight.activeSelf)
        {
            slotHilight.SetActive(false);
            //return to punch
            PlayerToolController.Instance.currentToolType = PlayerToolController.PlayerTool.Punch;
            PlayerToolController.Instance.currentItemData = punch;
            PlayerToolController.Instance.EquipOnTool();
        }else
        {
            slotHilight.SetActive(true);
            if (slotItem)
            {   
                PlayerToolController.Instance.currentToolType = slotItem.type;
                PlayerToolController.Instance.currentItemData = slotItem;
                PlayerToolController.Instance.EquipOnTool();
            }
           
        }
    }
    public void CraftEffect()
    {
        CraftEffectPrefab.SetActive(true);
        StartCoroutine(HideCraftEffect());
    }
    private IEnumerator HideCraftEffect()
    {
        // 等待一定时间
        yield return new WaitForSeconds(1f);

        // 隐藏特效
        CraftEffectPrefab.SetActive(false);
    }


}
