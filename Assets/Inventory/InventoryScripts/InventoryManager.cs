using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Inventory myBag;
    public GameObject slotGrid;
    public GameObject TableGrid;
    public GameObject CraftGrid;
    public Slot slotPrefab;
    public Text itemInformation;
    public GameObject EmptySlot;
    public List<GameObject> slots = new List<GameObject>();
    
    //open/close bag UI
    public GameObject bagPack;
    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }


    private void OnEnable()
    {
        RefreshItem();
        instance.itemInformation.text = "";
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (bagPack.activeSelf)
            {
                bagPack.SetActive(false);
                PlayerSkill.Instance.currentStatus = PlayerSkill.PlayerStatus.Idle;
                Cursor.lockState = CursorLockMode.Locked; 
                Cursor.visible = false;
            }
            else
            {
                bagPack.SetActive(true);
                PlayerSkill.Instance.currentStatus = PlayerSkill.PlayerStatus.OpenBag;
                Cursor.lockState = CursorLockMode.None;  
                Cursor.visible = true;  
                
            }
        }
    }

    public static void UpdateItemInfo(string itemDescription)
    {
        instance.itemInformation.text = itemDescription;
    }
    public static void SetWarningMessage()
    {
        instance.itemInformation.text = "Cannt Use Craft ";
    }
    public void SelectSlotByID(int SLotID)
    {
        for (var i = 0; i < 6; i++)
        {
            if(SLotID!= i)instance.slots[i].GetComponent<Slot>().slotHilight.SetActive(false);
        }
        slots[SLotID].GetComponent<Slot>().SetHilightSlot();
    }
    /*public string GetCraftNameBySlotID(int slotID)
    {
        if (slots[slotID].GetComponent<Slot>().slotItem != null)
        {
            return slots[slotID].GetComponent<Slot>().slotItem.itemName;
        }
        return "No item";

    }
    public int GetCraftNumBySlotID(int slotID)
    {
        if (slots[slotID].GetComponent<Slot>().slotItem != null)
        {
            return slots[slotID].GetComponent<Slot>().slotItem.itemHold;
        }
        return -1;
    }
    public void DestroyBagObject(int slotID)
    {
        slots[slotID].GetComponent<Slot>().slotItem = null;
        myBag.itemList[slotID] = null;
        Debug.Log("Have Destroy the resource");

       
    }*/
    
    public void RefreshItem()
    {
        for (int i = 0; i < instance.TableGrid.transform.childCount; i++)
        {
            //ѭ��ɾ�� slotgrid�µ��Ӽ�����
            if (instance.TableGrid.transform.childCount == 0)
            {
                break;
            }
            Destroy(instance.TableGrid.transform.GetChild(i).gameObject);
            instance.slots.Clear();
        }

        for (int i=0; i <instance.slotGrid.transform.childCount; i++)
        {
            //ѭ��ɾ�� slotgrid�µ��Ӽ�����
            if(instance.slotGrid.transform.childCount == 0)
            {
                break;
            }
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            instance.slots.Clear();
        }
        for (int i = 0; i < instance.CraftGrid.transform.childCount; i++)
        {
            //ѭ��ɾ�� slotgrid�µ��Ӽ�����
            if (instance.CraftGrid.transform.childCount == 0)
            {
                break;
            }
            Destroy(instance.CraftGrid.transform.GetChild(i).gameObject);
            instance.slots.Clear();
        }
        //�������ɶ�Ӧmybag�������Ʒ�� slot
        for (int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            if (i < 6)
            {
                //CreaterNewItem(instance.myBag.itemList[i]);
                instance.slots.Add(Instantiate(instance.EmptySlot));
                instance.slots[i].transform.SetParent(instance.TableGrid.transform);
                instance.slots[i].GetComponent<Slot>().slotID = i;
                instance.slots[i].GetComponent<Slot>().SetupSlot(instance.myBag.itemList[i]);
            }
            if (i >= 6 && i < 18)
            {
                instance.slots.Add(Instantiate(instance.EmptySlot));
                instance.slots[i].transform.SetParent(instance.slotGrid.transform);
                instance.slots[i].GetComponent<Slot>().slotID = i;
                instance.slots[i].GetComponent<Slot>().SetupSlot(instance.myBag.itemList[i]);

            }
            if (18 <= i && i < 21)
            {
                instance.slots.Add(Instantiate(instance.EmptySlot));
                instance.slots[i].transform.SetParent(instance.CraftGrid.transform);
                instance.slots[i].GetComponent<Slot>().slotID = i;
                instance.slots[i].GetComponent<Slot>().SetupSlot(instance.myBag.itemList[i]);
            }
        }



    }

    public void AddItem(int slotId,Item item)
    {
        //先判断是否背包有相同物体
        if (myBag.itemList.Contains(item))
        {
            item.itemHold++;
            
        }
        else
        {
            myBag.itemList[slotId] = item;
           
        }
        RefreshItem();
        SelectSlotByID(PickTableContentManager.Instance.currentIndex);
    }

    public void ConsumedItem(int slotId,int amount)
    {
        Item bag = myBag.itemList[slotId];
        int tempAmount = bag.itemHold - amount;
        if (tempAmount <= 0)
        {
            bag.itemHold = 1;
            myBag.itemList[slotId]=null;
            //to punch type
            PlayerToolController.Instance.currentToolType = PlayerToolController.PlayerTool.Punch;
        }
        else
        {
            bag.itemHold -= tempAmount;
        }
        RefreshItem();
        
    }

    public Item GetCurrentItemData(int slotId)
    {
        return myBag.itemList[slotId];
    }
    
    public void DropItem(int slotId)
    {
        Item bag = myBag.itemList[slotId];
        int tempAmount = bag.itemHold - 1;
        if (tempAmount <= 0)
        {
            bag.itemHold = 1;
            myBag.itemList.RemoveAt(slotId);
        }
        else
        {
            bag.itemHold -= tempAmount;
        }
        RefreshItem();
    }

    public void HilightNewItem(Item item)
    {
        FindSlotByItem(item).CraftEffect();
    }
    public int GetLastNotNullIndex()
    {
          for (int i = 0; i < myBag.itemList.Count - 1;i++)
          {
            if (myBag.itemList[i] == null) return i;  
           
          }
          return myBag.itemList.Count - 1;
    }
    public static Slot FindSlotByItem(Item item)
    {
        foreach (var slot in instance.slots)
        {
            Slot slotComponent = slot.GetComponent<Slot>();
            if (slotComponent != null && slotComponent.slotItem == item)
            {
                return slotComponent;
            }
        }
        return null;
    }
    







}
