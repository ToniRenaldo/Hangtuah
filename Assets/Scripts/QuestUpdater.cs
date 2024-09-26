using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUpdater : MonoBehaviour
{
    public string targetQuestId;
    public int progress;
    public bool updated;
    public void AddProgress()
    {
        updated = true;
        SaveFileController.instance.AddQuestProgress(progress, SaveFileController.instance.quests.Find(x=>x.questId == targetQuestId));
    }
}
