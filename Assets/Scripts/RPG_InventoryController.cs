using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class RPG_InventoryController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform placeholder;
    public GameObject itemPrefab;
    public GameData.Item currentItem;
    public List<RPGItem> items;
    public Button useButton;
    public void Initate(AvatarController.Stats stats)
    {
        var groupByID = stats.items.GroupBy(x=> x.id);

        foreach(var group in groupByID)
        {
            GameObject item = Instantiate(itemPrefab , placeholder);
            Debug.Log(group.Key);
            GameData.Item currentItem = GameData.instance.globalItem.Find(x => x.id == group.Key);
            item.GetComponent<RPGItem>().Initiate(stats,currentItem);
            items.Add(item.GetComponent<RPGItem>());
        }

    }

    private void Update()
    {
        if(currentItem == null)
        {
            useButton.interactable = false;
        }
    }
    public void ChooseObject(GameData.Item choosenItem)
    {
        currentItem = choosenItem;
    }
    public void UseObject()
    {
        if (currentItem == null)
            return;
        FindObjectOfType<TurnBasedRPG>().SetItem(currentItem);
        FindObjectOfType<TurnBasedRPG>().SetAction(TurnBasedRPG.ActionType.USEITEM);
    }
    public void UpdateItem(GameData.Item item)
    {
        items.Find(x => x.item.id == item.id).UpdateAmmount();   
    }
}
