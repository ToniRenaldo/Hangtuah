using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    // Start is called before the first frame update


    [Header("UI")]
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private Transform weaponTabContent;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemTabContent;
    [SerializeField] private GameObject armorPrefab;
    [SerializeField] private Transform armorContainer;
    [SerializeField] private TMP_Text gold;
    public void OpenInventory()
    {
        foreach(Transform t in armorContainer)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in itemTabContent)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in weaponTabContent)
        {
            Destroy(t.gameObject);
        }

        foreach(var panel in GlobalInventory.instance.weapons)
        {
            GameObject item = Instantiate(weaponPrefab, weaponTabContent);
            RPGItem rpgItem = item.GetComponent<RPGItem>();
            rpgItem.InventorySetup(panel,true, panel.owner);
        }

        foreach (var panel in GlobalInventory.instance.items)
        {
            GameObject item = Instantiate(itemPrefab, itemTabContent);
            RPGItem rpgItem = item.GetComponent<RPGItem>();
            rpgItem.InventorySetup(panel);
        }

        foreach (var panel in GlobalInventory.instance.armors)
        {
            GameObject item = Instantiate(armorPrefab, armorContainer);
            RPGItem rpgItem = item.GetComponent<RPGItem>();
            rpgItem.InventorySetup(panel , true , panel.owner);
        }
        gold.text = GlobalInventory.instance.gold.ToString();

        gameObject.SetActive(true);

    }
    public void SaveInventory()
    {
        SaveFileController.instance.Save();
    }
}
