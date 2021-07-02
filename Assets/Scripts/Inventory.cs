using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;
    public static Inventory instance;
    InventoryUI inventoryUI;

    [Header("Items")]
    public GameObject crate;
    public GameObject wood;
    public GameObject goldBar;

    [Header("Item Pickups From Dropping")]
    public GameObject axe_Pickup;
    public GameObject staff_Pickup;
    public GameObject sword_Pickup;
    public GameObject mana_Pickup;
    public GameObject health_Pickup;
    private void Awake()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found");
            return;
        }

        instance = this;
    }

   

    public int space;
    public List<Item> items = new List<Item>();
    public bool Add(Item item)
    {
        if (items.Count >= space)
        {
            Debug.Log("No room");
            return false;
                
        }
        items.Add(item);
        inventoryUI.UpdateUI();
        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);
    }
}
