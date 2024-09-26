using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class SaveFileController : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Serializable]
    public class Progress
    {
        public bool prologue;
        public bool tutorial;
        public bool pengamokAwal;
        public bool prologue2;

    }
    [System.Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    public class Quest
    {
        public string questId;
        public string title;
        public string description;
        public int target;
        public int progress;
        public bool isDone;
        public bool mainQuest;
        [JsonIgnore]
        public GameObject questOwner;
        public List<ResultCanvasController.Reward> rewards;
        public List<ProgressAction> progressActions;
        [System.Serializable]
        public class ProgressAction
        {
            public int progress;
            public UnityEvent OnProgressUpdated;
        }

    }
    public List<Quest> quests;
    public static SaveFileController instance; 
    public Transform newQuestCanvas;
    public GameObject questNotificationPrefab;

    [Header("Debug")]
    [SerializeField] public List<Quest> debugQuests;
    [SerializeField] int debugIndex;
    [SerializeField] int addedProgress;

    private void Awake()
    {
        instance = this;
        Load();
    }

    private void Start()
    {

        LoadPosition();

    }

    [ContextMenu("Debug Add New Quest")]
    public void DebugAddQuest()
    {
        foreach(var quest in debugQuests)
        {
            AddQuest(quest);
        }
    }


    [ContextMenu("Reset Save")]
    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
    }

    public void LoadPosition()
    {
        var player = FindObjectOfType<ThirdPersonController>();

        if (!PlayerPrefs.HasKey("PlayerPosition"))
        {
            Vector3 pos = player.transform.position;
            SerializableVector3 position = new SerializableVector3(pos);
            string posJson = JsonConvert.SerializeObject(position);

            PlayerPrefs.SetString("PlayerPosition", posJson);
        }
        else
        {

            Vector3 pos = JsonUtility.FromJson<SerializableVector3>(PlayerPrefs.GetString("PlayerPosition")).ToVector3();

            player.transform.position = pos;

            var distancePulau1 = Vector3.Distance(player.transform.position, FindObjectOfType<NelayanController>(true).TravelPointPulau1.position);
            var distancePulau2 = Vector3.Distance(player.transform.position, FindObjectOfType<NelayanController>(true).TravelPointPulau2.position);
            var distancePulauNikah = Vector3.Distance(player.transform.position, FindObjectOfType<NelayanController>(true).TravelPointPulauNikah.position);

            if (distancePulau1 < distancePulau2)
            {
                FindObjectOfType<NelayanController>(true).pulau1.SetActive(true);
                if (distancePulauNikah < distancePulau1)
                {
                    FindObjectOfType<NelayanController>(true).pulauNikah.SetActive(true);
                }
            }
            else
            {
                FindObjectOfType<NelayanController>(true).pulau2.SetActive(true);
                FindObjectOfType<NelayanController>(true).pulau1.SetActive(false);
                FindObjectOfType<NelayanController>(true).pulauNikah.SetActive(false);

            }


            FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.GetComponent<NavMeshAgent>().enabled = false);
            FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation));
            FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.GetComponent<NavMeshAgent>().enabled = true);

        }

    }

    [Header("Notification Quest")]
    public GameObject regularIconQuest;
    public GameObject animationIconQuest;
    public void AddQuest(Quest newQuest)
    {
        SendNotification("Misi baru ditambahkan");
        quests.Add(newQuest);

        if (newQuest.mainQuest)
        {
            regularIconQuest.SetActive(false );
            animationIconQuest.SetActive(true);
        }
        Save();
    }

    public async void SendNotification(string message)
    {
        Debug.Log("Pushing Notification");
        GameObject notification = Instantiate(questNotificationPrefab, newQuestCanvas);
        notification.GetComponentInChildren<TMP_Text>().text = message;
        notification.SetActive(true);
        await Task.Delay(3000);
        Destroy(notification);

    }


    [ContextMenu("Add Progress On Debug Quest")]
    public void DebugAddProgress()
    {
        AddQuestProgress(addedProgress , quests[debugIndex]);
    }
    public void AddQuestProgress(int progress, Quest quest)
    {

        quest.progress += progress;
        quest.progress = Mathf.Clamp(quest.progress, 0, quest.target);
        quest.progressActions.Find(x=>x.progress == quest.progress)?.OnProgressUpdated.Invoke();
        if (quest.progress == quest.target)
        {
            if(!quest.isDone)
            QuestDone(quest);
        }
        else
        {
            if(quest.progress != 0)
            SendNotification("Progres misi ditambahkan");
        }
        Save();
    }

    public List<QuestGiver> questGivers;

    public void Load()
    {
        var globalInv = FindObjectOfType<GlobalInventory>();
        if (PlayerPrefs.HasKey("PlayerPosition"))
        {
            GameObject.Find("Prologue 1").SetActive(false);
        }
        if (PlayerPrefs.HasKey("SaveQuest"))
        {
            Debug.Log(PlayerPrefs.GetString("SaveQuest"));
            quests = JsonConvert.DeserializeObject<List<Quest>>(PlayerPrefs.GetString("SaveQuest"));
            questGivers = FindObjectsOfType<QuestGiver>(true).ToList();
      

            foreach (var quest in quests)
            {
                questGivers = questGivers.FindAll(x => x.quest != null);
                questGivers = questGivers.FindAll(x => x.quest.questId != "");
                var q = questGivers.Find(x=>x.quest.questId == quest.questId);
                Debug.Log("Setting Up " + quest.questId);
                q.SetupQuest();
                Debug.Log("Setting Up " + quest.questId + " - Done");

            }

          
        }
        if (PlayerPrefs.HasKey("SaveItems"))
        {
            globalInv.items = JsonConvert.DeserializeObject<List<GameData.Item>>(PlayerPrefs.GetString("SaveItems"));
        }
        if (PlayerPrefs.HasKey("SaveWeapons"))
        {
            globalInv.weapons = JsonConvert.DeserializeObject<List<GameData.Weapon>>(PlayerPrefs.GetString("SaveWeapons"));
            foreach (var weapon in globalInv.weapons) 
            { 
         
                    
            }
        }

        if (PlayerPrefs.HasKey("SaveArmor"))
        {
            globalInv.armors = JsonConvert.DeserializeObject<List<GameData.Armor>>(PlayerPrefs.GetString("SaveArmor"));
        }
        if (PlayerPrefs.HasKey("SaveGold"))
        {
            globalInv.gold = PlayerPrefs.GetInt("SaveGold");
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {


        var json = JsonConvert.SerializeObject(quests);
        PlayerPrefs.SetString("SaveQuest", json);

        foreach (var item in GlobalInventory.instance.weapons)
        {
            if (item.owner == null)
            {
                item.avatar = AvatarController.AVATAR.None;
                continue;

            }
            item.avatar = item.owner.choosenAvatar;
        }
        foreach (var item in GlobalInventory.instance.armors)
        {
            if (item.owner == null)
            {
                item.avatar = AvatarController.AVATAR.None;
                continue;

            }
            item.avatar = item.owner.choosenAvatar;
        }


        var armors = JsonConvert.SerializeObject(GlobalInventory.instance.armors);

        PlayerPrefs.SetString("SaveArmor", armors);



      

        var weapons = JsonConvert.SerializeObject(GlobalInventory.instance.weapons);
        Debug.Log(weapons);

        PlayerPrefs.SetString("SaveWeapons", weapons);

        var items = JsonConvert.SerializeObject(GlobalInventory.instance.items);

        PlayerPrefs.SetString("SaveItems", items);

        PlayerPrefs.SetInt("SaveGold", GlobalInventory.instance.gold);

        var player = FindObjectOfType<ThirdPersonController>();

        Vector3 pos = player.transform.position;
        SerializableVector3 position = new SerializableVector3(pos);
        string posJson = JsonConvert.SerializeObject(position);

        PlayerPrefs.SetString("PlayerPosition", posJson);

    }
    public void QuestDone(Quest correspondingQuest)
    {
        SendNotification("Misi Selesai");
        Quest quest = quests.Find(x => x.questId == correspondingQuest.questId);
        quest.isDone = true;
        foreach (var reward in correspondingQuest.rewards)
        {
            if(reward.type == "Gold")
            {
                GlobalInventory.instance.gold += reward.value;
            }
        }

        Save();

    }
}
