using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public GameObject parent;
    public Image iconImage;
    public Sprite shopIcon;
    public Sprite questIcon;
    public Sprite sideQuestIcon;

    public Sprite mapIcon;
    public Sprite enemyIcon;

    public void Setup()
    {
        gameObject.SetActive(parent.activeInHierarchy);
        if (parent.GetComponent<ShopController>() != null)
        {
            iconImage.sprite = shopIcon;
        }
        else if (parent.GetComponentInChildren<NelayanController>() != null)
        {
            iconImage.sprite = mapIcon;
        }
        else if(parent.GetComponent<QuestGiver>() != null)
        {
            if(parent.GetComponent<QuestGiver>().quest != null)
            {
                if(parent.GetComponent<QuestGiver>().quest.isDone == false)
                {
                    iconImage.sprite = parent.GetComponent<QuestGiver>().quest.mainQuest? questIcon:sideQuestIcon;
                }
                else
                {
                    gameObject.SetActive(false);

                }
            }
            else
            {
                gameObject.SetActive(false);

            }
        }
        else if (parent.GetComponentInChildren<QuestGiver>() != null)
        {
            var questGiver = parent.GetComponentInChildren<QuestGiver>();
            if (questGiver.quest != null)
            {
                if (questGiver.quest.isDone == false)
                {
                    iconImage.sprite = questGiver.quest.mainQuest ? questIcon : sideQuestIcon;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);

            }
        }
        else if (parent.GetComponent<QuestUpdater>() != null)
        {
            var qu = parent.GetComponent<QuestUpdater>();
            var quest = SaveFileController.instance.quests.Find(x => x.questId == qu.targetQuestId);
            if (quest != null && quest.isDone == false && qu.updated == false)
            {
                iconImage.sprite = quest.mainQuest ? questIcon : sideQuestIcon;
            }
            else
            {
                gameObject.SetActive(false);
            }

        }
        
        else
        {
            gameObject.SetActive(false);
        }
    }
}
