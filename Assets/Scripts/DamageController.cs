using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    // Start is called before the first frame update
    public DynamicTextData floatingNumber;

    public int debugDamage;
    public Vector3 debugPos;
    [ContextMenu("Debug Show Damage")]
    void Debug_ShowDamage()
    {
        ShowDamage(debugDamage);
    }
    public void ShowDamage(int damage)
    {
        DynamicTextData data = floatingNumber;
        Vector3 dmgPos = debugPos;
        Vector3 destination = dmgPos;
        destination.x += (Random.value - 0.5f) / 3f;
        destination.y += Random.value;
        destination.z += (Random.value - 0.5f) / 3f;

        DynamicTextManager.CreateText(dmgPos, damage.ToString(), data);
    }
}
