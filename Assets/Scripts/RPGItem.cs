using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static AvatarController;

public class RPGItem : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Type
    {
        Item,
        Weapon,
        Armor
    }
    public AvatarController.Stats stats;
    public GameData.Item item;
    public GameData.Weapon weapon;
    public GameData.Armor armor;
    public GameData.CommonItem commonItem;
    public int ammount;
    public bool isWeapon;
    public Type itemType;
    [Header("UI")]
    public Image icon;
    public Image avatar;
    public TMP_Text avatarName;
    public TMP_Text itemName;
    public TMP_Text itemEffect;
    public TMP_Text itemAmmount;
    public Button buyButton;
    public Button useButton;
    [Header("Inventory")]
    public bool isInventory;
    public GameData.CommonItem choosenItem;

    [Header("Weapon")]
    public AvatarController owner;
    public Image ownerImage;
    
    public void Initiate(AvatarController.Stats stats, GameData.Item item)
    {
        this.stats = stats;
        this.item = item;


        var groupByID = stats.items.GroupBy(x => x.id);

        ammount = stats.items.FindAll(x => x.id == item.id).Count;

        itemName.text = item.objectName;

        string effect = "";
        if(item.effect == GameData.ItemEffect.IncreaseHealth)
        {
            effect += "+ " + item.value + "HP";
        }else if(item.effect == GameData.ItemEffect.IncreaseDamage)
        {
            effect += "+ " + item.value + "DMG";
        }
        itemEffect.text = effect;
        itemAmmount.text = "x" + ammount;
        icon.sprite = item.imageSprite;

        GetComponent<Button>().onClick.AddListener(ChooseItem);
    }
    public void UpdateAmmount()
    {
        ammount = stats.items.FindAll(x => x.id == item.id).Count;
        itemAmmount.text = "x" + ammount;
        if (ammount == 0)
            Destroy(gameObject); 
    }

    public void ChooseItem()
    {
        FindObjectOfType<RPG_InventoryController>().ChooseObject(this.item);
    }
     public void UseItem()
    {
        stats.items.Remove(item);
        UpdateAmmount();
    }

    public void InventorySetup(GameData.Weapon obj , bool isInventory = false, AvatarController owner = null)
    {

        this.weapon = GameData.instance.GetWeapon(obj);

        if (isInventory)
        {
            useButton.onClick.AddListener(() => choosenItem = this.weapon);
            useButton.onClick.AddListener(() => AvatarSelector.instance.ChooseAvatar(AssignItem));
        }

        if(owner == null)
        {
            avatar.gameObject.SetActive(false);

            useButton.gameObject.SetActive(isInventory);
        }
        else
        {
            avatar.sprite = AvatarGlobalData.instance.GetSprite(owner.choosenAvatar);
            avatar.gameObject.GetComponentInChildren<TMP_Text>(true).text = owner.stats.avatarName;

            avatar.gameObject.SetActive(true);
            useButton.gameObject.SetActive(false);
        }

        

        itemName.text = weapon.objectName;  
        itemEffect.text = weapon.damage + "DMG";
        if(weapon.imageSprite != null)
        {
            icon.sprite = weapon.imageSprite;
        }
        itemAmmount.text = weapon.price.ToString();
        itemType = Type.Weapon;
        commonItem = weapon;
    }
    public void InventorySetup(GameData.Item obj, bool store = false)
    {
        item = GameData.instance.GetItem(obj);

        if (store == false)
        {
            useButton.onClick.AddListener(() => choosenItem = item);
            useButton.onClick.AddListener(() => AvatarSelector.instance.ChooseAvatar(AssignItem));
        }


        itemType = Type.Item;

        useButton.gameObject.SetActive(!store);


        var groupByID = stats.items.GroupBy(x => x.id);

        ammount = stats.items.FindAll(x => x.id == item.id).Count;

        itemName.text = item.objectName;

        string effect = "";
        if (item.effect == GameData.ItemEffect.IncreaseHealth)
        {
            effect += "+ " + item.value + "HP";
        }
        else if (item.effect == GameData.ItemEffect.IncreaseDamage)
        {
            effect += "+ " + item.value + "DMG";
        }
        itemEffect.text = effect;
        itemAmmount.text = store? item.price.ToString() : "x" + ammount;
        icon.sprite = item.imageSprite;
        commonItem = item;

    }

    public void InventorySetup(GameData.Armor obj, bool isInventory = false, AvatarController owner = null)
    {
        this.armor = GameData.instance.GetArmor(obj);
        Debug.Log("Setting Up Data");
        if (isInventory)
        {
            Debug.Log("Button Clicked");
            useButton.onClick.AddListener(() => choosenItem = this.armor);
            useButton.onClick.AddListener(() => AvatarSelector.instance.ChooseAvatar(AssignItem));
        }

        if (owner == null)
        {
            avatar.gameObject.SetActive(false);
            useButton.gameObject.SetActive(isInventory);
        }
        else
        {
            avatar.sprite = AvatarGlobalData.instance.GetSprite(owner.choosenAvatar);
            Debug.Log("Owner : " + owner.stats.avatarName);
            avatar.gameObject.GetComponentInChildren<TMP_Text>(true).text = owner.stats.avatarName;
            avatar.gameObject.SetActive(true);
            useButton.gameObject.SetActive(false);
        }

        itemName.text = armor.objectName;
        itemEffect.text = armor.defendChance + "DEF";
        if (armor.imageSprite != null)
        {
            icon.sprite = armor.imageSprite;
        }
        itemAmmount.text = armor.price.ToString();
        itemType = Type.Armor;
        commonItem = armor;


    }

    public void AssignItem(AvatarController ava)
    {
 

        GameData.Item item = GameData.instance.globalItem.Find(x => x.id == choosenItem.id);
        if (item != null)
        {
            if(item.effect == GameData.ItemEffect.IncreaseHealth)
            {
                ava.stats.currentHP = Mathf.Clamp(ava.stats.currentHP + item.value, 0, ava.stats.defaultHP);
                var removeItem = GlobalInventory.instance.items.Find(x => x.id == item.id);
                GlobalInventory.instance.items.Remove(removeItem);
                if(FindObjectOfType<InventoryController>() != null)
                {
                    FindObjectOfType<InventoryController>().OpenInventory();
                }
            }
            return;
        }
        GameData.Armor armor = GameData.instance.globalArmor.Find(x => x.id == choosenItem.id);

        if(armor != null)
        {
            if (GlobalInventory.instance.armors.Find(x => x.owner == ava) != null)
                GlobalInventory.instance.armors.Find(x => x.owner == ava).owner = null;
            GlobalInventory.instance.armors.Find(x => x.id == armor.id && x.owner == null).owner = ava;
            ava.stats.armor = armor;
            if(FindObjectOfType<InventoryController>() != null)
            {
                FindObjectOfType<InventoryController>().OpenInventory();
            }
            return;
        }
        GameData.Weapon weapon = GameData.instance.globalWeapon.Find(x => x.id == choosenItem.id);

        if (weapon != null)
        {
            if(GlobalInventory.instance.weapons.Find(x => x.owner == ava) != null)
                GlobalInventory.instance.weapons.Find(x => x.owner == ava).owner = null;
            GlobalInventory.instance.weapons.Find(x => (x.id == weapon.id) && x.owner==null).owner = ava;
            ava.stats.weapon2 = weapon;
            if(FindObjectOfType<InventoryController>() != null)
            {
                FindObjectOfType<InventoryController>().OpenInventory();
            }
            return;
        }



    }
}
