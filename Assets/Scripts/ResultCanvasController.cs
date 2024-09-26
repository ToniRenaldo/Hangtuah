using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultCanvasController : MonoBehaviour
{
    public GameObject canvasResult;
    public GameObject rewardPrefab;
    public Transform rewardPlaceholder;
    public TMP_Text resultStatus;
    [System.Serializable]
    public class Reward
    {
        public Reward(string type , int value)
        {
            this.type = type;
            this.value = value; 
        }
        public string type;
        [JsonIgnore]
        public Sprite icon;
        public int value;
    }

    public List<Reward> listRewards;
    public List<Reward> transferedReward;
    bool win;
    public void AddReward(string type , int value)
    {
        GameObject reward = Instantiate(rewardPrefab, rewardPlaceholder);
        reward.GetComponentInChildren < TMP_Text>().text = $"{value} {type}";
        reward.GetComponentInChildren<Image>().sprite = listRewards.Find(x => x.type == type).icon;
        transferedReward.Add(new Reward(type, value));
    }
    
    public void ShowResult(bool success)
    {
        win = success;
        if (success)
        {
            resultStatus.text = "Berhasil!";
        }
        else
        {
            resultStatus.text = "Gagal!";
        }
        canvasResult.SetActive(true);

    }

    public void CloseCanvas()
    {
        foreach(Transform t in rewardPlaceholder)
        {
            Destroy(t.gameObject);
        }
        transferedReward.Clear();
        if (FindObjectOfType<BattleController>() != null)
        {
            FindObjectOfType<BattleController>().CloseBattle(win);
        }
        canvasResult.gameObject.SetActive(false);

       
    }

    public void AddToInventory()
    {

    }
}
