using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldController : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text goldCount;
    private void FixedUpdate()
    {
        if (GlobalInventory.instance != null)
            goldCount.text = GlobalInventory.instance.gold.ToString();
            
    }
}
