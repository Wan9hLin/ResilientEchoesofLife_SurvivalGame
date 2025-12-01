using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickTableContentManager : MonoBehaviour
{
    public Inventory myBag;

    private int ItemNum;

    public int currentIndex=0;

    public static PickTableContentManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {   
        if(myBag.itemList[0]==null)return;
        InventoryManager.instance.SelectSlotByID(0);
        PlayerToolController.Instance.currentToolType = myBag.itemList[0].type;
        PlayerToolController.Instance.currentItemData = myBag.itemList[0];
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            InventoryManager.instance.SelectSlotByID(0);
            currentIndex = 0;
            if (myBag.itemList[0] != null)
            {
                Debug.Log(myBag.itemList[0].itemName);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InventoryManager.instance.SelectSlotByID(1);
            currentIndex = 1;
            if (myBag.itemList[1] != null)
            {
                Debug.Log(myBag.itemList[1].itemName);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            InventoryManager.instance.SelectSlotByID(2);
            currentIndex = 2;
            if (myBag.itemList[2] != null)
            {
                Debug.Log(myBag.itemList[2].itemName);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            InventoryManager.instance.SelectSlotByID(3);
            currentIndex = 3;
            if (myBag.itemList[3] != null)
            {

                Debug.Log(myBag.itemList[3].itemName);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            InventoryManager.instance.SelectSlotByID(4);
            currentIndex = 4;
            if (myBag.itemList[4] != null)
            {
     
                Debug.Log(myBag.itemList[4].itemName);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            InventoryManager.instance.SelectSlotByID(5);
            currentIndex = 5;
            if (myBag.itemList[5] != null)
            {
        
                Debug.Log(myBag.itemList[5].itemName);
            }
        }


    }


}
