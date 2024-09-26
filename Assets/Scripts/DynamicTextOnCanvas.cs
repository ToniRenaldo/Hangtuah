using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTextOnCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject damagePrefab;
    public Transform canvas;
    public void ShowValue(Transform target , int value)
    {
        GameObject text = Instantiate(damagePrefab, canvas);
        text.GetComponent<ObjectFollower>().target3DObject = target;
        DamageController dmgController = FindObjectOfType<DamageController>();
        text.GetComponent<DynamicText>().Initialise(value.ToString(), dmgController.floatingNumber);
    }
}
