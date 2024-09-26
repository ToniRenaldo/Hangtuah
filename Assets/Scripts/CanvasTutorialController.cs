using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTutorialController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool debug;
    public string TutorialKey;
    private void Start()
    {
        if (debug)
            return;
        if (PlayerPrefs.HasKey(TutorialKey))
        {
            if(PlayerPrefs.GetInt(TutorialKey) == 1)
            {
                CloseTutorial();
            }
        }
    }

    [ContextMenu("Remove Key")]
    public void ResetKey()
    {
        PlayerPrefs.DeleteKey(TutorialKey); 
    }

    public void CloseTutorial()
    {
        if (!debug)
        {
            PlayerPrefs.SetInt(TutorialKey, 1);
        }
        gameObject.SetActive(false);
    }
}
