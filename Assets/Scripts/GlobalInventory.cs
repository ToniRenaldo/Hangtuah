using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GlobalInventory : MonoBehaviour
{
    public List<GameData.Item> items;
    public List<GameData.Weapon> weapons;
    public List<GameData.Armor> armors;
    [SerializeField] private int _gold;
    public int gold { 
        get { return _gold; }
        set { _gold = Mathf.Clamp(value, 0, 999999); }
    }
    [Header("Debug")]
    public int debugAddGold;
    public static GlobalInventory instance;

    private void Start()
    {
        weapons.ForEach(x => x.owner = FindObjectsOfType<AvatarController>().ToList().Find(y => y.choosenAvatar == x.avatar));
        foreach(var weapon in weapons)
        {
            if(weapon.owner != null)
            {
                if(weapon.owner.stats.weapon1 == null)
                {
                    weapon.owner.stats.weapon1 = weapon;
                }
                else if (weapon.owner.stats.weapon2 == null)
                {
                    weapon.owner.stats.weapon2 = weapon;
                }
            }
        }

        armors.ForEach(x => x.owner = FindObjectsOfType<AvatarController>().ToList().Find(y => y.choosenAvatar == x.avatar));

        foreach(var armor in armors)
        {
            armor.owner.stats.armor = armor;
        }

    }
    private void Awake()
    {
        instance = this;
     

    }

    [ContextMenu("Debug - Add Gold")]
    public void DebugAddGold()
    {
        AddGold(debugAddGold);
    }
    public void AddGold(int ammount)
    {
        gold += ammount;
    }
    public void AddItem(RPGItem item)
    {
        if(item.itemType == RPGItem.Type.Weapon)
        {
            weapons.Add(new GameData.Weapon() { id = item.commonItem.id ,avatar = AvatarController.AVATAR.None});
        }
        else if (item.itemType == RPGItem.Type.Armor)
        {
            armors.Add(new GameData.Armor() { id = item.commonItem.id , avatar = AvatarController.AVATAR.None });
        }
        else if (item.itemType == RPGItem.Type.Item)
        {
            items.Add(new GameData.Item() { id = item.commonItem.id });
        }

    }

}
