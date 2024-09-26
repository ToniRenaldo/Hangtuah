using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BotController : MonoBehaviour
{
    public enum BotType
    {
        Random = 0,
        Healer = 1,
        Berserk = 2,
        Agresive = 3,
        Defender =4
    }

    public int thinkingTime = 1000;

    [System.Serializable]
    public class Chance
    {
        public Chance(string action , int chance)
        {
            this.action = action;
            this.chance = chance;
        }
        public string action;
        public int chance;
    }

    public BotType type;
    public bool signatureMoveDone;
    public List<Chance> chances = new List<Chance>();

    public void SetType(BotType type)
    {

        chances.Add(new Chance("HEAL", 10));
        chances.Add(new Chance("ATTACK1", 30));
        chances.Add(new Chance("DMG_INC", 10));
        chances.Add(new Chance("GUARD", 10));

        this.type = type;
        if (type == BotType.Healer)
        {
            chances.Find(x =>x.action == "HEAL").chance = 20;
        }
        else if (type == BotType.Berserk)
        {
            chances.Find(x => x.action == "DMG_INC").chance = 20;
        }
        else if (type == BotType.Agresive)
        {
            chances.Find(x => x.action == "ATTACK1").chance = 50;
        }else if( type == BotType.Defender)
        {
            chances.Find(x => x.action == "GUARD").chance = 20;

        }





    }
    public async void Thinking()
    {
        // Action
        int totalChances = 0;
        foreach (var chance in chances)
        {
            totalChances += chance.chance;
        }

        int randomNumber = Random.Range(0, totalChances);

        // Choose object based on chances
        int cumulativeChances = 0;
        Chance choosenAction = null;
        foreach (var c in chances)
        {
            cumulativeChances += c.chance;
            if (randomNumber < cumulativeChances)
            {
                choosenAction = c;
                break;
            }
        }

        Debug.Log("Bot Choose : " + choosenAction.action);
        await Task.Delay(thinkingTime);;

        if (choosenAction.action == "HEAL")
        {
            TurnBasedRPG.instance.SetAction(TurnBasedRPG.ActionType.USEITEM);

            if (Random.Range(0, 2) == 0){
                TurnBasedRPG.instance.SetItem(GameData.instance.globalItem.Find(x => x.id == "smallHP"));
            }
            else
            {
                TurnBasedRPG.instance.SetItem(GameData.instance.globalItem.Find(x => x.id == "bigHP"));
            }
        }

        else if (choosenAction.action == "ATTACK1")
        {
            TurnBasedRPG.instance.SetAction(TurnBasedRPG.ActionType.ATTACK1);
            List<GameObject> targets = TurnBasedRPG.instance.leftTeam;
            GameObject target = targets[Random.Range(0, targets.Count)];
            TurnBasedRPG.instance.SetTarget(target);
        }
        else if (choosenAction.action == "ATTACK2")
        {
            TurnBasedRPG.instance.SetAction(TurnBasedRPG.ActionType.ATTACK2);
            List<GameObject> targets = TurnBasedRPG.instance.leftTeam;
            GameObject target = targets[UnityEngine.Random.Range(0, targets.Count)];
            TurnBasedRPG.instance.SetTarget(target);
        }
        else if (choosenAction.action == "DMG_INC")
        {
            TurnBasedRPG.instance.SetAction(TurnBasedRPG.ActionType.USEITEM);
            if (Random.Range(0, 2) == 0)
            {
                TurnBasedRPG.instance.SetItem(GameData.instance.globalItem.Find(x => x.id == "smallDmg"));
            }
            else
            {
                TurnBasedRPG.instance.SetItem(GameData.instance.globalItem.Find(x => x.id == "bigDmg"));
            }
        }
        else if (choosenAction.action == "GUARD")
        {
            TurnBasedRPG.instance.SetAction(TurnBasedRPG.ActionType.DEFEND);
        }

    }
}
