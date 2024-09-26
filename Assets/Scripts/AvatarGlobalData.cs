using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AvatarController;

public class AvatarGlobalData : MonoBehaviour
{
    // Start is called before the first frame update
    public List<AvatarList> avatarList = new List<AvatarList>();
    public static AvatarGlobalData instance;
    private void Awake()
    {
        instance = this;
    }

    public Sprite GetSprite(AVATAR avatar)
    {
        return avatarList.Find(x=>x.type == avatar).avatarSprite;
    }
}
