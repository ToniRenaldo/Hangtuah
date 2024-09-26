using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCanvasController : MonoBehaviour
{
    // Start is called before the first frame update[Header("UI")]
    [SerializeField] private GameObject questPrefab;
    [SerializeField] private Transform mainQuestContainer;
    [SerializeField] private Transform sideQuestContainer;
    public GameObject canvas;
    public void OpenQuestPanel()
    {
        foreach (Transform t in mainQuestContainer)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in sideQuestContainer)
        {
            Destroy(t.gameObject);
        }
        List<SaveFileController.Quest> quests = SaveFileController.instance.quests;

        foreach(var quest in quests)
        {
            Transform targetPanel;
            if (quest.mainQuest)
            {
                targetPanel = mainQuestContainer;
            }
            else
            {
                targetPanel = sideQuestContainer;
            }

            GameObject questPanel = Instantiate(questPrefab, targetPanel);
            questPanel.GetComponent<QuestItem>().SetupQuest(quest);
     
        }
        gameObject.SetActive(true);

    }
}
