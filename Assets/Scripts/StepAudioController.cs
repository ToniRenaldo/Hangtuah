using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepAudioController : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayStep()
    {
        if(GetComponent<AudioSource>() != null)
        GetComponent<AudioSource>().Play();
    }
}
