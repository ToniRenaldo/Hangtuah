using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PulauNikahController : MonoBehaviour
{

    public UnityEvent OnPulauEnabled;
    public UnityEvent OnPulauDisabled;

    private void OnEnable()
    {
        OnPulauEnabled.Invoke();
    }
    private void OnDisable()
    {
        OnPulauDisabled.Invoke();
    }
}
