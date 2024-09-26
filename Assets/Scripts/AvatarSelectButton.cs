using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelectButton : MonoBehaviour
{
    public Image avatar;
    public TMP_Text avatarName;
    public TMP_Text avatarHp;
    public Image avatarHpFill;

    public void SetupData(Sprite ava ,string avaName, int avaHp, int avaMaxHp)
    {
        avatar.sprite = ava;
        avatarName.text = avaName;
        avatarHp.text = $"HP : {avaHp}/{avaMaxHp}";
        avatarHpFill.fillAmount = (float)avaHp/avaMaxHp;
    }
}
