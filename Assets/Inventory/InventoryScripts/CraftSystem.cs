using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftSystem : MonoBehaviour
{
    public Inventory myBag;
    public Inventory CraftBag;
    private int Craf1Num, Craf2Num;

    public List<Item> craftResourceList;

    // Start is called before the first frame update
    void Start()
    {
        Craf1Num = 18;
        Craf2Num = 19;

    }

    // Update is called once per frame
    void Update()
    {



        //Debug.Log(Craft1);

    }

    public void Craft()
    {
        if (myBag.itemList[Craf1Num] != null && myBag.itemList[Craf2Num] != null)
        {
            // 合成需要两个格子
            if (myBag.itemList[Craf1Num].itemHold > 0 || myBag.itemList[Craf2Num].itemHold > 0)
            {
                switch (myBag.itemList[Craf1Num].itemName)
                {
                    case "Stone":
                        switch (myBag.itemList[Craf2Num].itemName)
                        {
                            case "Stick":
                                CraftStoneStick();
                                break;
                            default:
                                InventoryManager.SetWarningMessage();
                                break;
                        }
                        break;
                    case "Stick":
                        switch (myBag.itemList[Craf2Num].itemName)
                        {
                            case "Stone":
                                CraftStickStone();
                                break;
                            case "Rock":
                                CraftStickRock();
                                break;
                            case "Iron":
                                CraftStickIron();
                                break;

                            default:
                                InventoryManager.SetWarningMessage();
                                break;
                        }
                        break;
                    case "Rock":
                        switch (myBag.itemList[Craf2Num].itemName)
                        {
                            case "Stick":
                                CraftRockStick();
                                break;
                            default:
                                InventoryManager.SetWarningMessage();
                                break;
                        }
                        break;
                    default:
                        InventoryManager.SetWarningMessage();
                        break;
                    case "Iron":
                        switch (myBag.itemList[Craf2Num].itemName)
                        {
                            case "Stick":
                                CraftIronStick();
                                break;
                            default:
                                InventoryManager.SetWarningMessage();
                                break;
                        }
                        break;
                    
                }
            }
            else
            {
                InventoryManager.SetWarningMessage();
            }
        }
        else if (myBag.itemList[Craf1Num] != null || myBag.itemList[Craf2Num] != null)
        {
            // 合成需要一个格子
            if (myBag.itemList[Craf1Num] != null)
            {
                switch (myBag.itemList[Craf1Num].itemName)
                {
                    case "Log":
                        CraftLog();
                        break;
                    default:
                        InventoryManager.SetWarningMessage();
                        break;
                }
            }
            else if (myBag.itemList[Craf2Num] != null)
            {
                switch (myBag.itemList[Craf2Num].itemName)
                {
                    case "Log":
                        CraftLog();
                        break;
                    default:
                        InventoryManager.SetWarningMessage();
                        break;
                }
            }
            else
            {
                InventoryManager.SetWarningMessage();
            }
        }
        else
        {
            InventoryManager.SetWarningMessage();
        }
    }

    private void CraftStoneStick()
    {
        if (myBag.itemList[Craf1Num].itemHold < 2 || myBag.itemList[Craf2Num].itemHold < 1)
        {
            InventoryManager.SetWarningMessage();
        }
        else
        {
            // 执行合成逻辑
            myBag.itemList[Craf1Num].itemHold -= 2;
            myBag.itemList[Craf2Num].itemHold -= 1;
            int slotid = InventoryManager.instance.GetLastNotNullIndex();
            InventoryManager.instance.AddItem(InventoryManager.instance.GetLastNotNullIndex(), craftResourceList[5]);
            InventoryManager.instance.RefreshItem();
            InventoryManager.instance.HilightNewItem(craftResourceList[5]);

            Debug.Log("Succeed");
        }
    }

    private void CraftStickStone()
    {
        // 交换位置
        var temp = myBag.itemList[Craf1Num];
        myBag.itemList[Craf1Num] = myBag.itemList[Craf2Num];
        myBag.itemList[Craf2Num] = temp;

        if (myBag.itemList[Craf1Num].itemHold >= 2 && myBag.itemList[Craf2Num].itemHold >= 1)
        {
            // 执行合成逻辑
            myBag.itemList[Craf1Num].itemHold -= 2;
            myBag.itemList[Craf2Num].itemHold -= 1;
            int slotid = InventoryManager.instance.GetLastNotNullIndex();
            InventoryManager.instance.AddItem(InventoryManager.instance.GetLastNotNullIndex(), craftResourceList[5]);
            temp = myBag.itemList[Craf1Num];
            myBag.itemList[Craf1Num] = myBag.itemList[Craf2Num];
            myBag.itemList[Craf2Num] = temp;
            InventoryManager.instance.RefreshItem();
            InventoryManager.instance.HilightNewItem(craftResourceList[5]);

            Debug.Log("Succeed");
        }
    }

    private void CraftRockStick()
    {
        if (myBag.itemList[Craf1Num].itemHold < 3 || myBag.itemList[Craf2Num].itemHold < 1)
        {
            InventoryManager.SetWarningMessage();
        }
        else
        {
            // 执行合成逻辑
            myBag.itemList[Craf1Num].itemHold -= 3;
            myBag.itemList[Craf2Num].itemHold -= 1;
            int slotid = InventoryManager.instance.GetLastNotNullIndex();
            InventoryManager.instance.AddItem(InventoryManager.instance.GetLastNotNullIndex(), craftResourceList[6]);
            InventoryManager.instance.RefreshItem();
            InventoryManager.instance.HilightNewItem(craftResourceList[6]);

            Debug.Log("Succeed");
        }
    }
    private void CraftStickRock()
    {
        // 交换位置
        var temp = myBag.itemList[Craf1Num];
        myBag.itemList[Craf1Num] = myBag.itemList[Craf2Num];
        myBag.itemList[Craf2Num] = temp;
        if (myBag.itemList[Craf1Num].itemHold < 3 || myBag.itemList[Craf2Num].itemHold < 1)
        {
            InventoryManager.SetWarningMessage();
        }
        else
        {
            // 执行合成逻辑
            myBag.itemList[Craf1Num].itemHold -= 3;
            myBag.itemList[Craf2Num].itemHold -= 1;
            int slotid = InventoryManager.instance.GetLastNotNullIndex();
            InventoryManager.instance.AddItem(InventoryManager.instance.GetLastNotNullIndex(), craftResourceList[6]);
            InventoryManager.instance.RefreshItem();
            InventoryManager.instance.HilightNewItem(craftResourceList[6]);

            Debug.Log("Succeed");
        }
    }
    private void CraftIronStick()
    {
        if (myBag.itemList[Craf1Num].itemHold < 3 || myBag.itemList[Craf2Num].itemHold < 1)
        {
            InventoryManager.SetWarningMessage();
        }
        else
        {
            // 执行合成逻辑
            myBag.itemList[Craf1Num].itemHold -= 3;
            myBag.itemList[Craf2Num].itemHold -= 1;
            int slotid = InventoryManager.instance.GetLastNotNullIndex();
            InventoryManager.instance.AddItem(InventoryManager.instance.GetLastNotNullIndex(), craftResourceList[6]);
            InventoryManager.instance.RefreshItem();
            InventoryManager.instance.HilightNewItem(craftResourceList[6]);

            Debug.Log("Succeed");
        }
    }

    private void CraftStickIron()
    {
        var temp = myBag.itemList[Craf1Num];
        myBag.itemList[Craf1Num] = myBag.itemList[Craf2Num];
        myBag.itemList[Craf2Num] = temp;
        if (myBag.itemList[Craf1Num].itemHold < 3 || myBag.itemList[Craf2Num].itemHold < 1)
        {
            InventoryManager.SetWarningMessage();
        }
        else
        {
            // 执行合成逻辑
            myBag.itemList[Craf1Num].itemHold -= 3;
            myBag.itemList[Craf2Num].itemHold -= 1;
            int slotid = InventoryManager.instance.GetLastNotNullIndex();
            InventoryManager.instance.AddItem(InventoryManager.instance.GetLastNotNullIndex(), craftResourceList[6]);
            InventoryManager.instance.RefreshItem();
            InventoryManager.instance.HilightNewItem(craftResourceList[6]);

            Debug.Log("Succeed");
        }
    }
    private void CraftLog()
    {
        if (myBag.itemList[Craf1Num] != null)
        {
            if (myBag.itemList[Craf1Num].itemHold < 2)
            {
                InventoryManager.SetWarningMessage();
            }
            else
            {
                // 执行合成逻辑
                if (myBag.itemList[Craf1Num].itemHold > 2)
                {
                    myBag.itemList[Craf1Num].itemHold -= 2;
                    Debug.Log("Create Wood Succedd");
                    int slotid = InventoryManager.instance.GetLastNotNullIndex();
                    InventoryManager.instance.AddItem(InventoryManager.instance.GetLastNotNullIndex(), craftResourceList[7]);
                    InventoryManager.instance.RefreshItem();
                    InventoryManager.instance.HilightNewItem(craftResourceList[7]);

                }

                if (myBag.itemList[Craf1Num].itemHold == 2)
                {
                    myBag.itemList[Craf1Num].itemHold -= 1;
                    myBag.itemList[Craf1Num] = null;
                    int slotid = InventoryManager.instance.GetLastNotNullIndex();
                    InventoryManager.instance.AddItem(InventoryManager.instance.GetLastNotNullIndex(), craftResourceList[7]);
                    InventoryManager.instance.RefreshItem();
                    InventoryManager.instance.HilightNewItem(craftResourceList[7]);

                }
            }
        }
        else if (myBag.itemList[Craf2Num] != null)
        {
            if (myBag.itemList[Craf2Num].itemHold < 2)
            {
                InventoryManager.SetWarningMessage();
            }
            else
            {
                // 执行合成逻辑
                if (myBag.itemList[Craf2Num].itemHold > 2)
                {
                    myBag.itemList[Craf2Num].itemHold -= 2;
                    int slotid = InventoryManager.instance.GetLastNotNullIndex();
                    InventoryManager.instance.AddItem(InventoryManager.instance.GetLastNotNullIndex(), craftResourceList[7]);
                    InventoryManager.instance.RefreshItem();
                    InventoryManager.instance.HilightNewItem(craftResourceList[7]);

                }

                if (myBag.itemList[Craf2Num].itemHold == 2)
                {
                    myBag.itemList[Craf2Num].itemHold -= 1;
                    myBag.itemList[Craf2Num] = null;
                    int slotid = InventoryManager.instance.GetLastNotNullIndex();
                    InventoryManager.instance.AddItem(InventoryManager.instance.GetLastNotNullIndex(), craftResourceList[7]);
                    InventoryManager.instance.RefreshItem();
                    InventoryManager.instance.HilightNewItem(craftResourceList[7]);
                }
            }
        }
        else
        {
            InventoryManager.SetWarningMessage();
        }
    }


}

