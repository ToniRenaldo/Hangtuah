using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarTL : MonoBehaviour
{
    public Image queueImage;
    public Image profilePicture;
    public Sprite onQueueSprite;
    public Sprite activeSprite;
    public AvatarController avatarController;
    public TMP_Text playerName;
    public string avaName;
    public void Initialize(AvatarController ac)
    {
        avatarController = ac;
        playerName.text = ac.stats.avatarName;
        avaName = ac.stats.avatarName;  
        name = ac.stats.avatarName;

        if (profilePicture != null)
        {
            profilePicture.sprite = ac.avatarList.Find(x => x.type == ac.choosenAvatar).avatarSprite;
        }
    }
    public void ActiveTurn(bool flag)
    {
        queueImage.sprite = flag ? activeSprite : onQueueSprite;
    }
}
