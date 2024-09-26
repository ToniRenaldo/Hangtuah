using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelector : MonoBehaviour
{
    public static AvatarSelector instance;
    public GameObject canvas;
    public Transform avatarsContainer;
    public GameObject buttonPrefab;
    private void Awake()
    {
        instance = this;
    }

    public Action<AvatarController> CurrentCallback;
    public void ChooseAvatar(Action<AvatarController> callback)
    {
        CurrentCallback = callback;
        Debug.Log("Choosing Avatar");
        foreach (Transform t in avatarsContainer)
        {
            Destroy(t.gameObject);
        }
        var avas = AvatarDatas.instance.avatars;

        foreach(var ava in avas)
        {
            GameObject but = Instantiate(buttonPrefab, avatarsContainer);
            but.GetComponent<AvatarSelectButton>().SetupData(AvatarGlobalData.instance.GetSprite(ava.choosenAvatar), ava.stats.avatarName, ava.stats.currentHP, ava.stats.defaultHP);
            but.GetComponent<Button>().onClick.AddListener(() => callback.Invoke(ava));
            but.GetComponent<Button>().onClick.AddListener(() => canvas.gameObject.SetActive(false));

        }

        canvas.gameObject.SetActive(true);
    }
}
