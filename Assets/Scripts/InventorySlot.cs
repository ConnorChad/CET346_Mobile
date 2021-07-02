using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    UIManager uiManager;
    PlayerController player;
    Inventory inv;
    public Image icon;
    Item item;
    public GameObject removeButtonObj;
    public Button removeButton;
    bool toggleBool;
    bool toggleBool2;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        inv = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;

        if (item.state == Item.ItemType.Axe)
        {
            removeButtonObj.SetActive(true);
        }
        else if (item.state == Item.ItemType.Sword)
        {
            removeButtonObj.SetActive(true);
        }
        else if (item.state == Item.ItemType.Staff)
        {
            removeButtonObj.SetActive(true);
        }
        else if (item.state == Item.ItemType.HealthPotion)
        {
            removeButtonObj.SetActive(true);
        }
        else if (item.state == Item.ItemType.ManaPotion)
        {
            removeButtonObj.SetActive(true);
        }
        
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButtonObj.SetActive(false);
    }

    public void DropItem()
    {

        if (item.state == Item.ItemType.Axe)
        {
            var tempColour = icon.color;
            tempColour.a = 1f;
            icon.color = tempColour;
            player.toolEquipped = PlayerController.ToolEqupped.none;
            player.axe.SetActive(false);
            Instantiate(inv.axe_Pickup, player.throwPoint.position, player.throwPoint.rotation);
            Inventory.instance.Remove(item);

            ClearSlot();
        }
        else if (item.state == Item.ItemType.Sword)
        {
            var tempColour = icon.color;
            tempColour.a = 1f;
            icon.color = tempColour;
            player.toolEquipped = PlayerController.ToolEqupped.none;
            player.sword.SetActive(false);
            Instantiate(inv.sword_Pickup, player.throwPoint.position, player.throwPoint.rotation);
            Inventory.instance.Remove(item);
            ClearSlot();
        }
        else if (item.state == Item.ItemType.Staff)
        {
            var tempColour = icon.color;
            tempColour.a = 1f;
            icon.color = tempColour;
            player.toolEquipped = PlayerController.ToolEqupped.none;
            player.staff.SetActive(false);
            Instantiate(inv.staff_Pickup, player.throwPoint.position, player.throwPoint.rotation);
            Inventory.instance.Remove(item);
            ClearSlot();
        }
        else if (item.state == Item.ItemType.HealthPotion)
        {
            var tempColour = icon.color;
            tempColour.a = 1f;
            icon.color = tempColour;
            Instantiate(inv.health_Pickup, player.throwPoint.position, player.throwPoint.rotation);
            Inventory.instance.Remove(item);
            ClearSlot();
        }
        else if (item.state == Item.ItemType.ManaPotion)
        {
            var tempColour = icon.color;
            tempColour.a = 1f;
            icon.color = tempColour;
            Instantiate(inv.mana_Pickup, player.throwPoint.position, player.throwPoint.rotation);
            Inventory.instance.Remove(item);
            ClearSlot();
        }
        
    }

    public void UseItem()
    {
        if (item != null)
        {
            if (item.state == Item.ItemType.Crate)
            {
                Instantiate(inv.crate, player.throwPoint.position, player.throwPoint.rotation);
                Inventory.instance.Remove(item);
                ClearSlot();
            }

            if (item.state == Item.ItemType.Wood)
            {
                Instantiate(inv.wood, player.throwPoint.position, player.throwPoint.rotation);
                Inventory.instance.Remove(item);
                ClearSlot();
            }

            if (item.state == Item.ItemType.GoldBar)
            {
                Instantiate(inv.goldBar, player.throwPoint.position, player.throwPoint.rotation);
                Inventory.instance.Remove(item);
                ClearSlot();

            }

            if (item.state == Item.ItemType.HealthPotion)
            {
                if (player.health != player.maxHealth)
                {
                    player.health = 100;
                    Inventory.instance.Remove(item);
                    ClearSlot();
                }
                
            }

            if (item.state == Item.ItemType.ManaPotion)
            {
                if (player.mana != player.maxMana)
                {
                    player.mana = 100;
                    Inventory.instance.Remove(item);
                    ClearSlot();
                }
                
            }

          
            if (item.state == Item.ItemType.Axe)
            {
                if (player.toolEquipped == PlayerController.ToolEqupped.none)
                {
                    player.toolEquipped = PlayerController.ToolEqupped.axe;
                    var tempColour = icon.color;
                    tempColour.a = 0.5f;
                    icon.color = tempColour;
                    player.axe.SetActive(true);
                }
                else if (player.toolEquipped == PlayerController.ToolEqupped.axe)
                {
                    player.toolEquipped = PlayerController.ToolEqupped.none;
                    var tempColour = icon.color;
                    tempColour.a = 1f;
                    icon.color = tempColour;
                    player.axe.SetActive(false);
                }

            }

            if (item.state == Item.ItemType.Sword)
            {
                if (player.toolEquipped == PlayerController.ToolEqupped.none)
                {
                    player.toolEquipped = PlayerController.ToolEqupped.sword;
                    var tempColour = icon.color;
                    tempColour.a = 0.5f;
                    icon.color = tempColour;
                    player.sword.SetActive(true);
                    uiManager.attackButton.GetComponent<Image>().sprite = uiManager.meleeSprite;
              
                }
                else if (player.toolEquipped == PlayerController.ToolEqupped.sword)
                {
                    player.toolEquipped = PlayerController.ToolEqupped.none;
                    var tempColour = icon.color;
                    tempColour.a = 1f;
                    icon.color = tempColour;
                    player.sword.SetActive(false);
                }
            }

            if (item.state == Item.ItemType.Staff)
            {
                if (player.toolEquipped == PlayerController.ToolEqupped.none)
                {
                    player.toolEquipped = PlayerController.ToolEqupped.staff;
                    var tempColour = icon.color;
                    tempColour.a = 0.5f;
                    icon.color = tempColour;
                    player.staff.SetActive(true);
                    player.animator.SetBool("staff", true);
                    uiManager.attackButton.GetComponent<Image>().sprite = uiManager.rangedSprite;
                }
                else if (player.toolEquipped == PlayerController.ToolEqupped.staff)
                {
                    player.toolEquipped = PlayerController.ToolEqupped.none;
                    var tempColour = icon.color;
                    tempColour.a = 1f;
                    icon.color = tempColour;
                    player.staff.SetActive(false);
                    player.animator.SetBool("staff", false);
                }
            }



            if (item.state == Item.ItemType.Key)
            {
                if (player.toolEquipped == PlayerController.ToolEqupped.none)
                {
                    player.toolEquipped = PlayerController.ToolEqupped.key;
                    var tempColour = icon.color;
                    tempColour.a = 0.5f;
                    icon.color = tempColour;
                }
                else if (player.toolEquipped == PlayerController.ToolEqupped.key)
                {
                    player.toolEquipped = PlayerController.ToolEqupped.none;
                    var tempColour = icon.color;
                    tempColour.a = 1f;
                    icon.color = tempColour;
                }
            }
        }
    }
}
