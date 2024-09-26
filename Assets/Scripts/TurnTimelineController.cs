using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnTimelineController : MonoBehaviour
{
    public GameObject avatarTimeLinePrefab;
    public Transform timelineTransform;
    public List<AvatarTL> avatarTimeline = new List<AvatarTL>();
    public int activeAvatarAllowed;
    public Queue<AvatarTL> avatarTimelineQueue = new Queue<AvatarTL>();
    public bool isInitiated;
    public void Initiate()
    {
        Setup();
        isInitiated = true;
        SeeQueue();
    }

    public IEnumerator IE_NextTurn()
    {
        foreach(Transform t in timelineTransform)
        {
            Destroy(t.gameObject);
        }
        Setup();
        SeeQueue();
        yield return null;
    }
    
    public void Setup()
    {
        bool isLeft = TurnBasedRPG.instance.isLeftPlaying;
        int rightTeamTurn = TurnBasedRPG.instance.rightTeamTurn;

        int leftTeamTurn = TurnBasedRPG.instance.leftTeamTurn;
        for (int i = 0; i < activeAvatarAllowed; i++)
        {
            if (isLeft)
            {
                if(i == 0)
                {
                    rightTeamTurn = rightTeamTurn + 1 == TurnBasedRPG.instance.rightTeam.Count ? 0 : rightTeamTurn + 1;
                }
                Debug.Log("Setup Left");
                GameObject atl = Instantiate(avatarTimeLinePrefab, timelineTransform);
                atl.GetComponent<AvatarTL>().Initialize(
                    TurnBasedRPG.instance.leftTeam[leftTeamTurn].
                    GetComponent<AvatarController>());
                avatarTimelineQueue.Enqueue(atl.GetComponent<AvatarTL>());

                leftTeamTurn = leftTeamTurn + 1 == TurnBasedRPG.instance.leftTeam.Count ? 0 : leftTeamTurn + 1;

            }
            else
            {
                Debug.Log("Setup right");
                if (i == 0)
                {
                    leftTeamTurn = leftTeamTurn + 1 == TurnBasedRPG.instance.leftTeam.Count ? 0 : leftTeamTurn + 1;
                }
                GameObject atl = Instantiate(avatarTimeLinePrefab, timelineTransform);
                atl.GetComponent<AvatarTL>().Initialize(
                    TurnBasedRPG.instance.rightTeam[rightTeamTurn].
                    GetComponent<AvatarController>());
                avatarTimelineQueue.Enqueue(atl.GetComponent<AvatarTL>());

                rightTeamTurn = rightTeamTurn + 1 == TurnBasedRPG.instance.rightTeam.Count ? 0 : rightTeamTurn + 1;

            }
            isLeft = !isLeft;
        }
    }

    public void SeeQueue()
    {
        avatarTimeline.Clear();
        for(int i = 0; i < activeAvatarAllowed;i++)
        {
            avatarTimeline.Add(avatarTimelineQueue.ElementAt(i));
        }
    }
    public void DeleteFirst()
    {
        Destroy(avatarTimelineQueue.Dequeue().gameObject);
    }
    public void AddNewItem()
    {
        bool isLeft = TurnBasedRPG.instance.isLeftPlaying;
        int rightTeamTurn = TurnBasedRPG.instance.rightTeamTurn;
        int leftTeamTurn = TurnBasedRPG.instance.leftTeamTurn;

        for (int i = 0; i < activeAvatarAllowed; i++)
        {
            if(i == activeAvatarAllowed - 1)
            {
                if (isLeft)
                {
                    GameObject atl = Instantiate(avatarTimeLinePrefab, timelineTransform);
                    atl.GetComponent<AvatarTL>().Initialize(
                        TurnBasedRPG.instance.leftTeam[leftTeamTurn].
                        GetComponent<AvatarController>());
                    avatarTimelineQueue.Enqueue(atl.GetComponent<AvatarTL>());

                    leftTeamTurn = leftTeamTurn + 1 == TurnBasedRPG.instance.leftTeam.Count ? 0 : leftTeamTurn + 1;

                }
                else
                {
                    GameObject atl = Instantiate(avatarTimeLinePrefab, timelineTransform);
                    atl.GetComponent<AvatarTL>().Initialize(
                        TurnBasedRPG.instance.rightTeam[rightTeamTurn].
                        GetComponent<AvatarController>());
                    avatarTimelineQueue.Enqueue(atl.GetComponent<AvatarTL>());

                    rightTeamTurn = rightTeamTurn + 1 == TurnBasedRPG.instance.rightTeam.Count ? 0 : rightTeamTurn + 1;

                }
            }
            else
            {
                if (isLeft)
                {
                    leftTeamTurn = leftTeamTurn + 1 == TurnBasedRPG.instance.leftTeam.Count ? 0 : leftTeamTurn + 1;
                }
                else
                {
                    rightTeamTurn = rightTeamTurn + 1 == TurnBasedRPG.instance.rightTeam.Count ? 0 : rightTeamTurn + 1;
                }
                
            }
            
            isLeft = !isLeft;
        }
    }
}
