using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName = "Inventory/New Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public int itemHold;
    [TextArea]
    public string itemInfo;
    public int damage;
    public PlayerToolController.PlayerTool type;
    public int addHpAmount;
    public int addHungerAmount;
}
