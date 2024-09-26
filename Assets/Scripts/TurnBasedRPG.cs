using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TurnBasedRPG : MonoBehaviour
{
    // Start is called before the first frame update
    public enum ActionType
    {
        IDLE,
        ATTACK1,
        ATTACK2,
        DEFEND,
        USEITEM
    }
    [Header("Debug")]
    public List<AvatarStats> setupRightTeam;
    public List<AvatarStats> setupLeftTeam;
    public static TurnBasedRPG instance;
    [Header("UI")]

    public List<Transform> rightSpot;
    public List<Transform> leftSpot;
    public GameObject avatarControllerPrefab;

    public List<GameObject> rightTeam = new List<GameObject>();
    public List<GameObject> leftTeam = new List<GameObject>();

    public List<GameObject> playerOrder= new List<GameObject>();
    public int leftTeamTurn;
    public int rightTeamTurn;
    public bool isLeftPlaying;

    [Header("Current Activity")]
    public GameObject currentAvatarTurn;
    public ActionType actionType;
    public GameObject actionTarget;
    public GameData.Item choosenItem;
    public RPG_InventoryController inventoryController;
    [Header("UIs")]
    public GameObject attackCanvas;
    public GameObject targetCanvas;
    public GameObject itemCanvas;
    public GameObject mainCanvas;
    public Transform playerStatsPanel;
    public Transform enemyStatsPanel;
    public List<RPGPlayerStats> playerStatsUI;
    public GameObject playerStatsPrefab;
    public GameObject playerStatsMiniPrefab;

    [Header("Target UI")]
    public Transform leftTeamPanel;
    public Transform rightTeamPanel;
    public GameObject targetPrefab;
    [Header("Camera")]
    public Transform mainCamera;
    public Transform leftTeamCamPos;
    public Transform rightTeamCamPos;

    [Header("Result")]
    public List<ResultCanvasController.Reward> rewardList;
    public List<ResultCanvasController.Reward> punishmentList;

    public ResultCanvasController resultController;
    private void Awake()
    {
        instance = this;
    }




    [ContextMenu("Start Battle")]

    // Fucntion ini berguna untuk memulai Battle RPG
    public void StartBattle()
    {
        
        Application.targetFrameRate = 60;
        InitiateBattle(setupRightTeam,setupLeftTeam);
    }


    public async void InitiateBattle(List<AvatarStats> rightTeamStats , List<AvatarStats> leftTeamStats)
    {
        // Spawing Avatar
        inventoryController = GetComponent<RPG_InventoryController>();


        // Setup statisitik player team kanan (team musuh)

        for (int i = 0; i < rightTeamStats.Count; i++)
        {
            // Spawn Avatar Musuh pada posisi yang sudah ditentukan
            GameObject avatar = Instantiate(avatarControllerPrefab , rightSpot[i],true);
            avatar.transform.localPosition = Vector3.zero;
            avatar.transform.localRotation = Quaternion.identity;

            // Set AvatarController
            // Avatar controller berguna untuk menyimpan statistik avatar dan berguna mengatur behaviour avatar
            // seperti set animasi, damage, dan hit
            AvatarController ava = rightTeamStats[i].avatar;
            
            // Set avatar karakter
            avatar.GetComponent<AvatarController>().SetAvatar(ava.choosenAvatar , false);

            // Set statistik avatar (Attack , HP, Shield)
            avatar.GetComponent<AvatarController>().SetStats(ava, ava.stats.weapon1, ava.stats.weapon2, ava.stats.armor, ava.stats.items);
            rightTeam.Add(avatar);
            GameObject stats = Instantiate(playerStatsMiniPrefab , enemyStatsPanel);

            // Setup stat darah dan armor diatas kepala karakter
            stats.GetComponent<RPGPlayerStats>().Initiate(avatar.GetComponent<AvatarController>());

            playerStatsUI.Add(stats.GetComponent<RPGPlayerStats>());
            stats.GetComponent<ObjectFollower>().target3DObject = avatar.GetComponent<AvatarController>().statPlace;

            // Set Bot Behaviour
            avatar.AddComponent<BotController>();
            avatar.GetComponent<BotController>().SetType( rightTeamStats[i].avatar.GetComponent<BotController>().type);

        }

        // Setup avatar player
        for (int i = 0; i < leftTeamStats.Count; i++)
        {
            //Spawn avatar
            GameObject avatar = Instantiate(avatarControllerPrefab, leftSpot[i]);
            avatar.transform.localPosition = Vector3.zero;
            avatar.transform.localRotation = Quaternion.identity;

            // Set karakter avatar
            avatar.GetComponent<AvatarController>().SetAvatar(leftTeamStats[i].avatar.choosenAvatar, leftTeamStats[i].isPlayer);

            AvatarController ava = leftTeamStats[i].avatar;
            avatar.GetComponent<AvatarController>().SetAvatar(ava.choosenAvatar,true);

            // Set Statistik avatar
            avatar.GetComponent<AvatarController>().SetStats(ava, ava.stats.weapon1, ava.stats.weapon2, ava.stats.armor, ava.stats.items);
            leftTeam.Add(avatar);
            GameObject stats = Instantiate(playerStatsPrefab, playerStatsPanel);

            // Setup statistik (Attack, HP, Shield) pada pojok kiri bawah
            stats.GetComponent<RPGPlayerStats>().Initiate(avatar.GetComponent<AvatarController>());
            playerStatsUI.Add(stats.GetComponent<RPGPlayerStats>()); 


        }

        //Setup Inventory

        SetupInventory();

        // Setup Team

        // Setup turn
        leftTeamTurn = Random.Range(0,leftTeam.Count);
        rightTeamTurn = Random.Range(0, rightTeam.Count);
        isLeftPlaying = Random.Range(0, 2) == 0;
        SetupAttackChoice();
        // Start Coroutine

        if(FadeCanvasController.instance != null)
        {
            await FadeCanvasController.instance.FadeIn();
        }
        StartCoroutine(IE_Gameplay());

    }
    public void ChangeWeapon()
    {
        AvatarController p1 = currentAvatarTurn.GetComponent<AvatarController>();
        if (GameData.instance.GetWeapon(p1.stats.weapon2) == null)  
        {
            return;
        }
        p1.ChangeWeapon(p1.activeWeapon == p1.stats.weapon1? p1.stats.weapon2:p1.stats.weapon1) ;
        FindObjectOfType<SwapWeaponController>().SetWeaponName(p1.activeWeapon.objectName);

    }

    public void SetupInventory()
    {
        leftTeam.ForEach(x => x.GetComponent<AvatarController>().stats.items = leftTeam[0].GetComponent<AvatarController>().stats.items);
        inventoryController.Initate(leftTeam[0].GetComponent<AvatarController>().stats);
    }
    public void CloseAllCanvas(bool mainCanvasTurnOn =true)
    {
        mainCanvas.gameObject.SetActive(mainCanvasTurnOn);
        targetCanvas.gameObject.SetActive(false);
        attackCanvas.gameObject.SetActive(false);
        itemCanvas.gameObject.SetActive(false);
    }
    public void SetupAttackChoice()
    {
        foreach(var player in leftTeam)
        {
            GameObject playerButton = Instantiate(targetPrefab, leftTeamPanel);
            playerButton.GetComponent<ActionTargetButton>().targetAvatar = player.GetComponent<AvatarController>();
            playerButton.GetComponent<ActionTargetButton>().targetName.text = player.GetComponent<AvatarController>().stats.avatarName;
            playerButton.GetComponent<Button>().onClick.AddListener(()=>SetTarget(player));
            playerButton.GetComponent<ActionTargetButton>().avatarImage.sprite = player.GetComponent<AvatarController>().GetAvatar();

        }
        foreach (var player in rightTeam)
        {
            GameObject playerButton = Instantiate(targetPrefab, rightTeamPanel);
            playerButton.GetComponent<ActionTargetButton>().targetAvatar = player.GetComponent<AvatarController>();
            playerButton.GetComponent<ActionTargetButton>().targetName.text = player.GetComponent<AvatarController>().stats.avatarName;
            List<AvatarController.AvatarList> avatarList = player.GetComponent<AvatarController>().avatarList;
            playerButton.GetComponent<ActionTargetButton>().avatarImage.sprite = player.GetComponent<AvatarController>().GetAvatar();

            playerButton.GetComponent<Button>().onClick.AddListener(() => SetTarget(player));


        }

    }
    public IEnumerator IE_Gameplay()
    {
        CloseAllCanvas(false);
        yield return IE_PlayNotification("Pertarungan Dimulai!",2);

        // While selamanya hingga dihentikan jika semua musuh sudah mati
        while (true)
        {


            // Increment Turn dari karakter. Jika berganti maka tambahakan increment agar menjadi giliran
            // karakter selanjutnya
            if (isLeftPlaying)
            {
                leftTeamTurn = leftTeamTurn + 1 == leftTeam.Count ? leftTeamTurn = 0 : leftTeamTurn + 1;
            }
            else
            {
                rightTeamTurn = rightTeamTurn + 1 == rightTeam.Count ? rightTeamTurn = 0 : rightTeamTurn + 1;
            }

            // Set Timeline pada sebelah kiri layar
            if(GetComponent<TurnTimelineController>().isInitiated == false)
            {
                GetComponent<TurnTimelineController>().Initiate();

            }
            else
            {
                StartCoroutine(GetComponent<TurnTimelineController>().IE_NextTurn());

            }


            Transform currentCamPos = isLeftPlaying ? leftTeamCamPos : rightTeamCamPos;

            // Set posisi kamera sesuai posisi team yang bermain
            mainCamera.SetPositionAndRotation(currentCamPos.position, currentCamPos.rotation);

            currentAvatarTurn = isLeftPlaying ? leftTeam[leftTeamTurn] : rightTeam[rightTeamTurn];

            AvatarController p1 = currentAvatarTurn.GetComponent<AvatarController>();

            // Set giliran karakter / Avatar Controller selanjutnya
            p1.SetTurn(true);

            Debug.Log(p1.avatarName + " Turns ! ");

            // Keluarkan notifikasi memberitahukan giliran karakter yang bermain
            yield return IE_PlayNotification($"Giliran {p1.stats.avatarName}", 0.5f);

            // Set button swap weapon
            FindObjectOfType<SwapWeaponController>(true).SetWeaponName(p1.activeWeapon.objectName);



            attackCanvas.gameObject.SetActive(false);
            targetCanvas.SetActive(false);
            leftTeamPanel.gameObject.SetActive(false);
            rightTeamPanel.gameObject.SetActive(false);

            mainCanvas.GetComponent<CanvasGroup>().interactable = isLeftPlaying;
            mainCanvas.SetActive(isLeftPlaying);
            actionType = ActionType.IDLE;
            actionTarget = null;
            choosenItem = null;

            if (!isLeftPlaying)
            {
                // Jika giliran bot, maka karakter akan berfikir terlebih dahulu apa action yang ingin dia lakukan
                // Action yang tersedia adalah Attack, Guard, Heal, Increase Damage
                // Untuk sekarang logika thinking nya hanya random
                p1.GetComponent<BotController>().Thinking();
            }

            // Menunggu actionType yang dipilih oleh karakter tidak sama dengan Idle
            yield return new WaitUntil(() => actionType != ActionType.IDLE);

            AvatarController p2 = null;

            // Jika karakter memilih untuk attack maka...(1)
            if (actionType !=  ActionType.DEFEND && actionType != ActionType.USEITEM)
            {
                // (1)...Menunggu action target (action target adalah target siapa action tersebut ditujukan)
                yield return new WaitUntil(() => actionTarget != null);
                CloseAllCanvas(false);
                p2 = actionTarget.GetComponent<AvatarController>();
            }


            yield return new WaitForSeconds(0.5f);
            
            GameObject deathPlayer = null;

            // jika pemain memilih attack
            if(actionType == ActionType.ATTACK1)
            {
                if(isLeftPlaying && GameData.instance.debug)
                {

                }
                if(p1.activeWeapon.type != GameData.WeaponType.PANAH)
                {
                    yield return p1.IE_MoveToAttackPos(p2.attackPlace);
                }

                // Berikan damage pada P2, dengan parameter jumlah damage yang bisa diberikan P1
                deathPlayer = p2.GiveDamage(p1.Attack1());
                yield return new WaitForSeconds(2);
                yield return p1.IE_ResetPos();
            }
            // Jika pemain memilih defend
            else if (actionType == ActionType.DEFEND)
            {
                Debug.Log("Guards Up");

                // Memnaggil action defend pada Avatar 
                p1.Defend();
                yield return p1.IE_ResetPos();

            }

            // Jika pemain memilih use item
            else if(actionType == ActionType.USEITEM)
            {
                yield return new WaitUntil(() => choosenItem != null);
                
                p1.UseItem(choosenItem);
                yield return p1.IE_ResetPos();

            }

            // Update statistik player (HP, dan Shield)
            playerStatsUI.Find(x => x.currentAc == p1).UpdateStats();
            if(p2 != null)
            playerStatsUI.Find(x => x.currentAc == p2).UpdateStats();


            // Jika ada karakter yang darahnya sudah <=0 maka set kematian karakter tersebut
            if (deathPlayer != null)
            {
                SetDeath(deathPlayer);
            }



            // Check apakah sudah memenuhi kondisi kemenangan ? 
            if (isWin())
            {
                // Tampilkan panel menang
                ShowWin();
                yield break;
            }
            // Stop giliran pemain p1
            p1.SetTurn(false);
            
            CloseAllCanvas(false);

            yield return new WaitForSeconds(0.5f);

            // Ganti giliran
            isLeftPlaying = !isLeftPlaying;


            yield return null;

        }


    }
    public void SetDeath(GameObject avatar)
    {
        if (isLeftPlaying)
        {
            rightTeam.Remove(avatar);
            rightTeamTurn--;
        }
        else
        {
            leftTeam.Remove(avatar);
            leftTeamTurn--;

        }
    }
    public bool isWin()
    {
        bool won = true;
        if (isLeftPlaying)
        {
            foreach(var i in rightTeam)
            {
                if (i.GetComponent<AvatarController>().stats.currentHP != 0)
                {
                    won = false;
                    break;
                }
            }
            
        }
        else
        {
            foreach (var i in leftTeam)
            {
                if (i.GetComponent<AvatarController>().stats.currentHP != 0)
                {
                    won = false;
                    break;
                }
            }
        }
        return won;
    }

    public void ShowWin()
    {
        if (isLeftPlaying)
        {
            Debug.Log("Left Team Win");

            foreach (var item in rewardList)
            {
                resultController.AddReward(item.type, item.value);
            }

            resultController.ShowResult(true);
        }
        else
        {
            Debug.Log("Right Team Win");
            foreach (var item in punishmentList)
            {
                resultController.AddReward(item.type, item.value);
            }
            resultController.ShowResult(false);

        }

        foreach(var ava in AvatarDatas.instance.avatars)
        {
            if (leftTeam.Find(x => x.GetComponent<AvatarController>().choosenAvatar == ava.choosenAvatar) == null)
                continue;
            ava.stats.currentHP = leftTeam.Find(x=>x.GetComponent<AvatarController>().choosenAvatar == ava.choosenAvatar).GetComponent<AvatarController>().stats.currentHP;
            if(ava.stats.currentHP <= 10)
            {
                ava.stats.currentHP = 10;
            }
        }
    }

    public void OpenTargetPanel()
    {
        targetCanvas.gameObject.SetActive(true);
        Transform targetTransform = isLeftPlaying? rightTeamPanel:leftTeamPanel;
        targetTransform.gameObject.SetActive(true);
        foreach(Transform player in targetTransform)
        {
            ActionTargetButton target = player.GetComponent<ActionTargetButton>();
            if (target != null)
            {
                if (target.targetAvatar.stats.currentHP <= 0)
                    target.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void SetAction(ActionType action)
    {
        actionType = action;

    }
    public void SetAction(int action)
    {
        actionType = (ActionType) action;   
    }
    public void SetTarget(GameObject target)
    {
        actionTarget = target;
        SetAction(ActionType.ATTACK1);

    }
    ActionType tempType;
    public void SetAttackType(int type )
    {
        tempType = (ActionType) type;
    }
    public void SetItem(GameData.Item item)
    {
        choosenItem = item;
    }

    [Header("Notification")]
    public GameObject notificationPanel;
    public TMP_Text notificationText;

    public IEnumerator IE_PlayNotification(string content, float time)
    {
        notificationText.text = content;
        CanvasGroup cg = notificationPanel.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        notificationPanel.gameObject.SetActive(true);
        while(cg.alpha < 1)
        {
            cg.alpha += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1;

        yield return new WaitForSeconds(time);

        while (cg.alpha > 0)
        {
            cg.alpha -= Time.deltaTime;
            yield return null;
        }
        notificationPanel.gameObject.SetActive(false);



    }

}
