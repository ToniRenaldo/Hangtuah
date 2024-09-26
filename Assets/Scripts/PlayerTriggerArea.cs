using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTriggerArea : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityEvent playerEnterArea;
    public UnityEvent playerExitArea;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerEnterArea.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerExitArea.Invoke();
        }
    }
}
