
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86;

public class BattleController : MonoBehaviour
{
    public GameObject BattlePrefab;
    public List<GameObject> bot;
    public GameObject mainCamera;
    public static BattleController instance;
    [Header("Result")]
    public List<ResultCanvasController.Reward> rewardList;
    public List<ResultCanvasController.Reward> punishmentList;
    public GameObject activeBattle;
    public System.Action<bool> CurrentCallback;
    [Header("Icon")]
    [SerializeField] private Sprite goldIcon;
    AudioClip previousAudioClip;
    [ContextMenu("Test Start Battle")]
    public void TestBattle()
    {
        StartBattle();

    }

    private void Awake()
    {
        instance = this;
    }
    public void StartBattle(bool random = true, List<ResultCanvasController.Reward> rewards = null, List<ResultCanvasController.Reward> punishments = null, List<AvatarController> enemyAvatars = null, System.Action<bool> callback = null, int startingRandomIndex = 0, int endRandomIndex = 0 , int level = 1, int count = 1, List<AvatarController> playerTeam = null)
    {
        GameObject battle = Instantiate(BattlePrefab);
        activeBattle = battle;
        battle.transform.position = new Vector3(0, 400, 0);

        List<GameData.Weapon> weapons = GameData.instance.globalWeapon.FindAll(x => x.level == level);
        List<GameData.Armor> armors = GameData.instance.globalArmor.FindAll(x => x.level == level);


        battle.GetComponent<TurnBasedRPG>().setupLeftTeam.Clear();
        battle.GetComponent<TurnBasedRPG>().setupRightTeam.Clear();


        int enemyCount = random ? count : enemyAvatars.Count ;

        for(int i = 0; i< enemyCount; i++)
        {
            GameObject obj = new GameObject((random ? "Bot " + (i+1) : enemyAvatars[i].stats.avatarName));
            bot.Add(obj);
            AvatarController ac = obj.AddComponent<AvatarController>();
            ac.choosenAvatar =  random ? (AvatarController.AVATAR)Random.Range(startingRandomIndex, endRandomIndex) : enemyAvatars[i].choosenAvatar;
            ac.stats.avatarName = random? "Musuh " + (i+1) : enemyAvatars[i].stats.avatarName;
            ac.stats.weapon1 = random ? weapons[Random.Range(0, weapons.Count)] : 
                GameData.instance.GetWeapon(enemyAvatars[i].stats.weapon1);

            ac.stats.defaultHP = random? Random.Range(5, 15) * level : enemyAvatars[i].stats.defaultHP;
            ac.stats.currentHP = ac.stats.defaultHP;
            ac.stats.defaultAP = random? Random.Range(10, 20) * level : enemyAvatars[i].stats.defaultAP;
            ac.stats.currentAP = ac.stats.defaultAP;


            battle.GetComponent<TurnBasedRPG>().setupRightTeam.Add(new AvatarStats() { avatar = ac });

            BotController bc = obj.AddComponent<BotController>();
            bc.type = (BotController.BotType)Random.Range(0, 5);
        }
        int goldReward = Random.Range(30, 70);
        int goldPunishment = Random.Range(0,-20);

        rewardList.Clear();
        punishmentList.Clear();

        if (random)
        {
            rewardList.Add(new ResultCanvasController.Reward("Gold", goldReward));
            punishmentList.Add(new ResultCanvasController.Reward("Gold", goldPunishment));
        }
        else
        {
            if(rewards != null)
                rewardList.AddRange(rewards);
            if(punishments != null)
                punishmentList.AddRange(punishments);
        }
        

        battle.GetComponent<TurnBasedRPG>().rewardList.Clear();
        battle.GetComponent<TurnBasedRPG>().rewardList.AddRange(rewardList);

        battle.GetComponent<TurnBasedRPG>().punishmentList.Clear();
        battle.GetComponent<TurnBasedRPG>().punishmentList.AddRange(punishmentList);
        //Setup Reward

        var avas = FindObjectOfType<AvatarDatas>().avatars;
        if (playerTeam != null)
        {
            avas = playerTeam;
        }

        foreach (var ava in avas)
        {
            AvatarStats newStat = new AvatarStats() { avatar = ava };
            newStat.avatar.stats.items = GlobalInventory.instance.items;
            battle.GetComponent<TurnBasedRPG>().setupLeftTeam.Add(newStat);
            
        }

        //Fade Here

        // Turning Off Camera
        mainCamera.gameObject.SetActive(false);
        CurrentCallback = callback;
        battle.GetComponent<TurnBasedRPG>().StartBattle();
        previousAudioClip = FindObjectOfType<AudioManager>().bgmAudioSource.clip;
        FindObjectOfType<AudioManager>().SetBattleBGM();
    }

    public async void CloseBattle(bool win)
    {
        await FadeCanvasController.instance.FadeOut();
        foreach (var item in rewardList)
        {
            if (item.type == "Gold")
            {
                GlobalInventory.instance.gold += item.value;
            }
        }
        mainCamera.gameObject.SetActive(true);

        foreach(var t in bot)
        {
            Destroy(t.gameObject);
        }
        if(CurrentCallback != null)
            CurrentCallback.Invoke(win);
        CurrentCallback = null;

        Destroy(activeBattle);
        await FadeCanvasController.instance.FadeIn();
        if(previousAudioClip == FindObjectOfType<AudioManager>().roamingClip)
        {
            FindObjectOfType<AudioManager>().SetRoamingBGM();
        }
    }
}
