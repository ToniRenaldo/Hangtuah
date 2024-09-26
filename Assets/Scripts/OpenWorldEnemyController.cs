using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class OpenWorldEnemyController : MonoBehaviour
{
    public bool random;
    public bool countRandom;
    public int enemyCount;
    public List<ResultCanvasController.Reward> reward;
    public List<ResultCanvasController.Reward> punishment;

    public List<AvatarController> enemyAvatars;
    public UnityEvent OnWin;
    public UnityEvent OnLose;

    public int indexRandomStart;
    public int indexRandomEnd;
    public int level;
    public TMP_Text levelTMP;

    public List<GameObject> team;
    [Header("Debug vs HangJebat")]
    public bool versusHangJebat;
    private void Start()
    {
    }
    private void OnEnable()
    {
        for (int i = 0; i < team.Count; i++)
        {
            team[i].gameObject.SetActive(i < enemyCount);
        }
        levelTMP.text = "Lv." + level;

    }
    public async void InitiateBattle()
    {
        if (random)
        {
            if (countRandom) 
            {
                enemyCount = UnityEngine.Random.Range(1, 5);
            }

            await FadeCanvasController.instance.FadeOut();
            BattleController.instance.StartBattle(callback: EndBattle, startingRandomIndex:indexRandomStart,endRandomIndex:indexRandomEnd,level:level , count:enemyCount);
        }
        else
        {
            BattleController.instance.StartBattle(false,reward,punishment,enemyAvatars: enemyAvatars , EndBattle);
        }
    }

    public async void InitiateByOneBattle(string avaName)
    {
        if(GetComponent<QuestGiver> () != null)
        {
            if (GetComponent<QuestGiver>().questSettedUp == false)
                return;
        }
        List<AvatarController> avatars = new List<AvatarController>();
        
        if (Enum.TryParse(avaName, out AvatarController.AVATAR avaType))
        {
            Debug.Log("Parsed enum: " + avaType);
        }
        else
        {
            Debug.LogError("Invalid enum string: " + avaType);
        }

        avatars.Add(FindObjectOfType<AvatarDatas>().avatars.Find(x => x.choosenAvatar == avaType));
        if (versusHangJebat)
        {
            BattleController.instance.StartBattle(random : false,playerTeam: avatars, callback: EndBattle, startingRandomIndex: indexRandomStart, endRandomIndex: indexRandomEnd, level: level, enemyAvatars: enemyAvatars);
            return;
        }
        BattleController.instance.StartBattle(playerTeam: avatars, callback: EndBattle, startingRandomIndex: indexRandomStart, endRandomIndex: indexRandomEnd, level: level, count: enemyCount);
    }

    public void EndBattle(bool win)
    {
        if (win)
        {
            OnWin.Invoke();
        }
        else
        {
            OnLose.Invoke();
        }
    }
}
