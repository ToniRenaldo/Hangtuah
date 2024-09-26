using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource bgmAudioSource;
    [Header("ButtonBGM")]
    public GameObject buttonBGM;
    public GameObject imageMuteBGM;
    public GameObject imageUnMuteBGM;
    public Slider bgmSlider;


    [Header("Clips")]
    public AudioClip battleClip;
    public AudioClip roamingClip;


    public AudioClip pulau1AC;
    public AudioClip pulau2AC;
    public AudioClip pulau3AC;

    public float defaultVolume;

    [Header("SFX")]

    public AudioSource sfxAudioSource;
    [Header("Button SFx")]
    public GameObject buttonSfx;
    public GameObject imageMuteSfx;
    public GameObject imageUnMuteSfx;
    public Slider sfxSlider;

    private void Start()
    {
        bgmSlider.value = bgmAudioSource.volume;
        sfxSlider.value = sfxAudioSource.volume;

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }


    public void SetBGMVolume(float value)
    {
        bgmAudioSource.volume = value;
    }
    public void SetSFXVolume(float value)
    {
        sfxAudioSource.volume = value;
    }

    public void ChangeBGMToPulau2()
    {
        roamingClip = pulau2AC;
        SetRoamingBGM();

    }
    public void ChangeBGMToPulau3()
    {
        roamingClip = pulau3AC;
        SetRoamingBGM();

    }

    public void ToggleBGM()
    {
        if(bgmAudioSource.volume  != 0)
        {
            bgmAudioSource.volume = 0;
            
            imageMuteBGM.SetActive(true);
            imageUnMuteBGM.SetActive(false);
        }
        else
        {
            bgmAudioSource.volume = defaultVolume;
            imageMuteBGM.SetActive(false);
            imageUnMuteBGM.SetActive(true);
        }
    }

    public void ToggleSFX()
    {
        if (sfxAudioSource.volume != 0)
        {
            sfxAudioSource.volume = 0;
            imageMuteSfx.SetActive(true);
            imageUnMuteSfx.SetActive(false);
        }
        else
        {
            sfxAudioSource.volume = defaultVolume;
            imageMuteSfx.SetActive(false);
            imageUnMuteSfx.SetActive(true);
        }
    }
    public void SetBattleBGM()
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = battleClip;
        bgmAudioSource.Play();
    }
    public void SetRoamingBGM()
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = roamingClip;
        bgmAudioSource.Play();
    }

}
