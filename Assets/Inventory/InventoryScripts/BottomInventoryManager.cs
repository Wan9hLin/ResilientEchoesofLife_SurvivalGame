using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BottomInventoryManager : MonoBehaviour
{
    static BottomInventoryManager instance;

    public Inventory myBag;
    public GameObject slotGrid;
    public Slot slotPrefab;
    
    public GameObject EmptySlot;
    public List<GameObject> slots = new List<GameObject>();
    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }


    private void OnEnable()
    {
        RefreshItem();
       
    }

    
    /*public static void CreaterNewItem(Item item)
    {
        Slot newItem = Instantiate(instance.slotPrefab, instance.slotGrid.transform.position, Quaternion.identity);
        newItem.gameObject.transform.SetParent(instance.slotGrid.transform);

        newItem.slotItem = item;
        newItem.slotImage.sprite = item.itemImage;
        // 传输东西的数据 ， transfer the main information to here
        newItem.slotNum.text = item.itemHold.ToString();
    }*/

    public static void RefreshItem()
    {
        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            //循环删除 slotgrid下的子集物体
            if (instance.slotGrid.transform.childCount == 0)
            {
                break;
            }
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            instance.slots.Clear();
        }
        //重新生成对应mybag里面的物品的 slot
        for (int i = 0; i < instance.myBag.itemList.Count; i++)
        {
            //CreaterNewItem(instance.myBag.itemList[i]);
            instance.slots.Add(Instantiate(instance.EmptySlot));
            instance.slots[i].transform.SetParent(instance.slotGrid.transform);
            instance.slots[i].GetComponent<Slot>().slotID = i;
            instance.slots[i].GetComponent<Slot>().SetupSlot(instance.myBag.itemList[i]);
        }
    }
}
