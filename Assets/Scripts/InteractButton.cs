using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Button interactButton;
    public UnityAction currentCallback;
    public void ActivateButton(UnityAction callback)
    {
        interactButton.gameObject.SetActive(true);
        currentCallback = callback;
        interactButton.onClick.AddListener(currentCallback);
    }
    public void DeactivateButton()
    {
        interactButton.gameObject.SetActive(false);
        interactButton.onClick.RemoveAllListeners();
    }
}
