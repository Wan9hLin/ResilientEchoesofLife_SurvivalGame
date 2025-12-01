using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnDrag : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public Transform originalParent;

    public Inventory myBag;

    public int currentItemID;

    public bool isAlreadyActive;

    public bool isAdd;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        currentItemID = originalParent.GetComponent<Slot>().slotID;
        isAlreadyActive = originalParent.GetComponent<Slot>().slotHilight.activeSelf;


        if (currentItemID == 20) isAdd = true;
        //Debug.Log(isAlreadyActive);
        transform.SetParent(transform.parent.parent);
        transform.position = eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
      /*  Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);*/

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            //not craft position ,to drag and switch
            if (!isAdd) {

                if (eventData.pointerCurrentRaycast.gameObject.name == "ItemImage")
                {
                    var parent = eventData.pointerCurrentRaycast.gameObject.transform.parent;
                    transform.SetParent(parent.parent);
                    transform.position = parent.parent.position;


                    //itemList的物品存储位置改变
                    var temp = myBag.itemList[currentItemID];
                    //

                    var newSlotId = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID;

                    myBag.itemList[currentItemID] = myBag.itemList[newSlotId];
                    myBag.itemList[newSlotId] = temp;

                    parent.position = originalParent.position;
                    parent.SetParent(originalParent);

                    var slot = parent.gameObject.GetComponentInParent<Slot>();
                    if (slot.slotHilight.activeSelf)
                    {
                        PlayerToolController.Instance.currentToolType = myBag.itemList[currentItemID].type;
                        PlayerToolController.Instance.currentItemData = myBag.itemList[currentItemID];
                        PlayerToolController.Instance.EquipOnTool();
                        Debug.Log("drag_current_item:" + myBag.itemList[currentItemID].itemName);
                    }
                    else if (!isAlreadyActive)
                    {
                        if (PickTableContentManager.Instance.currentIndex == newSlotId)
                        {
                            PlayerToolController.Instance.currentToolType = myBag.itemList[newSlotId].type;
                            PlayerToolController.Instance.currentItemData = myBag.itemList[newSlotId];
                            PlayerToolController.Instance.EquipOnTool();
                            Debug.Log("drag_current_item:" + myBag.itemList[newSlotId].itemName);
                        }

                    }

                    InventoryManager.instance.RefreshItem();
                    //highlight
                    InventoryManager.instance.SelectSlotByID(PickTableContentManager.Instance.currentIndex);
                    GetComponent<CanvasGroup>().blocksRaycasts = true;
                    return;
                }
                if (eventData.pointerCurrentRaycast.gameObject.name == "slot(Clone)")
                {
                    transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform);
                    transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;
                    myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID] = myBag.itemList[currentItemID];

                    if (eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID != currentItemID)
                    {
                        myBag.itemList[currentItemID] = null;
                    }
                    if (!isAlreadyActive)
                    {
                        if (PickTableContentManager.Instance.currentIndex == eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID)
                        {
                            PlayerToolController.Instance.currentToolType = myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID].type;
                            PlayerToolController.Instance.currentItemData = myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID];
                            PlayerToolController.Instance.EquipOnTool();
                            Debug.Log("drag_current_item:" + myBag.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID].itemName);
                        }
                    }
                    InventoryManager.instance.RefreshItem();
                    //highlight
                    InventoryManager.instance.SelectSlotByID(PickTableContentManager.Instance.currentIndex);
                    GetComponent<CanvasGroup>().blocksRaycasts = true;
                    return;
                }

            }else
            {
                //判断这个item是否在背包内，如果在增加数值，拖拽结束对象必须为null，将物体放置过去
                if(myBag.itemList.Contains(originalParent.GetComponent<Slot>().slotItem)){
                    Debug.Log(true);
                }else { }
            }

        }
        //其他任何位置都归为
        if(eventData.pointerCurrentRaycast.gameObject == null)
        {

        }

        transform.SetParent(originalParent);
        transform.position = originalParent.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        InventoryManager.instance.RefreshItem();
        //highlight
        InventoryManager.instance.SelectSlotByID(PickTableContentManager.Instance.currentIndex);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
