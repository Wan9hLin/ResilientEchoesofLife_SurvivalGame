using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToolController : MonoBehaviour
{   
    public enum PlayerTool
    {
        Hammer,Shovel,Spear,Resource,Axe,Water,Punch,Food,Shield
    }

    public PlayerTool currentToolType;
    
    public static PlayerToolController Instance;

    public Item currentItemData;

    public GameObject currentWeapon; 
        
    public List<GameObject> tool;


    private void Awake()
    {
        Instance = this;
    }


    // Equips the tool corresponding to the currently selected item
    public void EquipOnTool()
    {
        foreach (var toolObj in tool)
        {
            if (toolObj.name == currentItemData.itemName)
            {
                toolObj.SetActive(true);
                currentWeapon = toolObj;
                
            }
            else
            {
                toolObj.SetActive(false);
            }
        }
    }


    //Returns the type of the currently equipped tool
    public PlayerTool GetPlayerToolType()
    {
        return currentToolType;
    }
    
    //Set box collider to hit / attack animal 
    public void ActiveBoxCollider()
    {
        currentWeapon.GetComponent<BoxCollider>().enabled = true;
        PlayerSkill.Instance.currentStatus = PlayerSkill.PlayerStatus.Attack;
    }

    // Deactivates the hitbox (BoxCollider) for the equipped tool
    public void DeActiveBoxCollider()
    {
        currentWeapon.GetComponent<BoxCollider>().enabled = false;
        PlayerSkill.Instance.currentStatus = PlayerSkill.PlayerStatus.Idle;
    }
}
