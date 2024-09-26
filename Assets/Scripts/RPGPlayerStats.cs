using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RPGPlayerStats : MonoBehaviour
{

    public AvatarController currentAc;
    public AvatarController.Stats currentStat;

    [Header("UI")]
    public Image shieldFill;
    public Image hpFill;
    public Image profilePicture;
    public Sprite choosenSprite;
    public TMP_Text avatarName;
    public void Initiate(AvatarController ac)
    {
        currentAc = ac;
        currentStat = ac.stats;
        shieldFill.fillAmount = (float)currentStat.currentAP / (float)currentStat.defaultAP;
        hpFill.fillAmount = (float)currentStat.currentHP / (float)currentStat.defaultHP;
        if (profilePicture != null)
        {
  
            choosenSprite = ac.avatarList.Find(x => x.type == ac.choosenAvatar).avatarSprite;

            profilePicture.sprite = choosenSprite;
      
        }
        avatarName.text = currentStat.avatarName;
    }

    public void UpdateStats()
    {
        StartCoroutine(IE_UpdateStats());
    }
    public IEnumerator IE_UpdateStats()
    {

        while ((shieldFill.fillAmount != (float)currentStat.currentAP / (float)currentStat.defaultAP)  )       
        {
            shieldFill.fillAmount = Mathf.MoveTowards(shieldFill.fillAmount, (float)currentStat.currentAP / (float)currentStat.defaultAP, 0.05f);
            yield return null;
        }

        while ((hpFill.fillAmount != (float)currentStat.currentHP / (float)currentStat.defaultHP))
        {
            hpFill.fillAmount = Mathf.MoveTowards(hpFill.fillAmount, (float)currentStat.currentHP / (float)currentStat.defaultHP, 0.05f);
            yield return null;
        }
        
    }

}
