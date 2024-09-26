using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class SwapWeaponController : MonoBehaviour
{
    public TMP_Text weaponTMP;
    public void SetWeaponName(string weaponName)
    {
        GetComponent<Animator>().SetTrigger("Swap");
        weaponTMP.text = weaponName;
    }
}
