using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class NelayanController : MonoBehaviour
{
    public int targetGold;
    public List<string> oldDialogue;
    public List<string> newDialogue;
    public UnityEvent OnDialogueDoneOld;
    public UnityEvent OnDialogueDoneAfterOld;

    public UnityEvent OnDialogueDone;
    public bool targetAchieved;

    public GameObject pulau1;
    public Transform TravelPointPulau1;
    public GameObject pulau2;
    public Transform TravelPointPulau2;

    [Header("Pulau Nikah")]
    public GameObject pulauNikah;
    public Transform TravelPointPulauNikah;

   
    private void Start()
    {
        oldDialogue.AddRange( GetComponent<NpcController>().dialogues);
        OnDialogueDoneAfterOld = GetComponent<NpcController>().OnDialogueAfterDone;
        OnDialogueDoneOld = GetComponent<NpcController>().OnDialogueDone;
    }
    private void Update()
    {
        if (!targetAchieved)
        {
            if(GlobalInventory.instance.gold >= targetGold)
            {
                GetComponent<NpcController>().dialogues = newDialogue;
                GetComponent<NpcController>().dialogueAfter = newDialogue;

                GetComponent<NpcController>().OnDialogueDone = OnDialogueDone;
                GetComponent<NpcController>().OnDialogueAfterDone = OnDialogueDone;
                targetAchieved = true;
            }
        }
        else
        {
            if (GlobalInventory.instance.gold < targetGold)
            {
                GetComponent<NpcController>().dialogues = oldDialogue;
                GetComponent<NpcController>().dialogueAfter = oldDialogue;
                GetComponent<NpcController>().OnDialogueDone = OnDialogueDoneOld;
                GetComponent<NpcController>().OnDialogueAfterDone = OnDialogueDoneAfterOld;

                targetAchieved = false;
            }
        }
   
    }


    [ContextMenu("Travel Pulau Nikah")]
    public void TravelToPulauNikah()
    {
        GlobalInventory.instance.gold -= targetGold;
        if (SaveFileController.instance.quests.Find(x => x.questId == "dermaga0")?.isDone == false)
        {
            SaveFileController.instance.AddQuestProgress(1, SaveFileController.instance.quests.Find(x => x.questId == "dermaga0"));
        }
        OnDialogueDone.RemoveAllListeners();
        OnDialogueDone.AddListener(TravelToPulau2);
        TransitionTravelToPulauNikah();
    }


    public void TravelToPulau2()
    {
        GlobalInventory.instance.gold -= targetGold;
        if(SaveFileController.instance.quests.Find(x=>x.questId == "dermaga1")?.isDone == false)
        {
            SaveFileController.instance.AddQuestProgress(1, SaveFileController.instance.quests.Find(x => x.questId == "dermaga1"));
        }
        Travel(true);
    }

    public async void TransitionTravelToPulauNikah()
    {
        await Teleport(TravelPointPulauNikah);
        SaveFileController.instance.SendNotification("Anda telah tiba di Pulau Majapahit");

    }

    public async Task Teleport(Transform tpPoint) 
    {
        FindObjectOfType<InteractButton>(true).gameObject.SetActive(false);
        FindObjectOfType<ThirdPersonController>().allowedToMove = false;
        await FindObjectOfType<FadeCanvasController>().FadeOut();

        pulauNikah.SetActive(tpPoint == TravelPointPulauNikah);
        pulau1.SetActive(tpPoint == TravelPointPulauNikah || tpPoint == TravelPointPulau1);
        pulau2.SetActive(tpPoint == TravelPointPulau2);

        var player = FindObjectOfType<ThirdPersonController>();
        player.transform.SetPositionAndRotation(tpPoint.position, tpPoint.rotation);
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x=>x.GetComponent<NavMeshAgent>().enabled = false); 
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation));
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.GetComponent<NavMeshAgent>().enabled = true);

        await Task.Delay(2000);
        await FindObjectOfType<FadeCanvasController>().FadeIn();
        FindObjectOfType<InteractButton>(true).gameObject.SetActive(true);
        FindObjectOfType<ThirdPersonController>().allowedToMove = true;
        FindObjectOfType<CinemachineVirtualCamera>().Follow = FindObjectOfType<ThirdPersonController>().GetComponent<CameraPosition>().cameraPos;
        FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = 8;

        SaveFileController.instance.Save();
    }

    public async Task TeleportImmidiate(Transform tpPoint)
    {
        FindObjectOfType<InteractButton>(true).gameObject.SetActive(false);
        FindObjectOfType<ThirdPersonController>().allowedToMove = false;
        await FadeCanvasController.instance.FadeOut();



        var player = FindObjectOfType<ThirdPersonController>();
        player.transform.SetPositionAndRotation(tpPoint.position, tpPoint.rotation);
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.GetComponent<NavMeshAgent>().enabled = false);
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation));
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.GetComponent<NavMeshAgent>().enabled = true);

        await Task.Delay(2000);
        await FadeCanvasController.instance.FadeIn();
        FindObjectOfType<InteractButton>(true).gameObject.SetActive(true);
        FindObjectOfType<ThirdPersonController>().allowedToMove = true;
    }
    public async void Travel(bool flagPulau2)
    {
        FindObjectOfType<InteractButton>().gameObject.SetActive(false);
        Transform tpPoint = flagPulau2 ? TravelPointPulau2 : TravelPointPulau1;
        await Teleport(tpPoint);
        SaveFileController.instance.SendNotification("Anda telah tiba area baru");
    }

}
