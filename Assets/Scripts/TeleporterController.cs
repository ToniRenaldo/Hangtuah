using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TeleporterController : MonoBehaviour
{
    [System.Serializable]
    public class TeleportPoint
    {
        public string id;
        public Transform point;
    }

    public static TeleporterController instance;

    public List<TeleportPoint> points = new List<TeleportPoint>();


    private void Awake()
    {
        instance = this;
    }


    public async void TeleportPlayer(string position)
    {
        Transform player = FindObjectOfType<LocalPlayer>(true).transform;
        Transform point = points.Find(x => x.id == position).point;
        await FadeCanvasController.instance.FadeOut();
        player.transform.SetPositionAndRotation(point.position, point.rotation);
        await Task.Delay(1000);

        await FadeCanvasController.instance.FadeIn();

    }
}
