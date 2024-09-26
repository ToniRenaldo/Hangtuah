using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarDatas : MonoBehaviour
{
    public static AvatarDatas instance;
    public List<AvatarController> avatars;
    private void Awake()
    {
        instance = this;    
    }
}
