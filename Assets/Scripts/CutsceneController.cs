using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool playOnAwake;

    void Start()
    {
        if (playOnAwake)
        {
            GetComponent<NpcController>().ShowDialgoue();
        }
    }

}
