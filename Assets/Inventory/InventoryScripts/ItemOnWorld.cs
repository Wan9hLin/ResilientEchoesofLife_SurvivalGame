using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld : MonoBehaviour
{
    public Item thisItem;
    public Inventory PlayerInventory;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            AddNewItem();
            Destroy(gameObject);
        }
    }
    public void AddNewItem()
    {
        if (!PlayerInventory.itemList.Contains(thisItem))
        {
            //PlayerInventory.itemList.Add(thisItem);
            //InventoryManager.CreaterNewItem(thisItem);
            for(int i = 0;i < PlayerInventory.itemList.Count; i++)
            {
                if(PlayerInventory.itemList[i] == null)
                {
                    PlayerInventory.itemList[i] = thisItem;
                    break; 
                }
            }
        }
        else
        {
            thisItem.itemHold += 1;
        }
        InventoryManager.instance.RefreshItem();
    }
}
