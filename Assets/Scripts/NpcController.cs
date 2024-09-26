using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NpcController : MonoBehaviour
{
    // Start is called before the first frame update
    public string npcName;
    public bool isAggresive;
    public List<string> dialogues;
    public List<string> dialogueAfter;

    public UnityEvent OnDialogueDone;
    public UnityEvent OnDialogueAfterDone;

    public List<AudioClip> narasi;

    [Header("Canvas")]
    public GameObject dialogueCanvas;
    public TMP_Text dialogueTMP;
    public TMP_Text dialogueName;
    public Button canvasButton;

    public Animator avaAnimator;
    public Transform cameraPosition;
    public bool interacted { get; set; }
    public int dialogueCounter = 0;
    public bool typing;
    private List<string> activeDialogue;
    public float dialogueSpeed;
    public float defaultCamDistance;
    [Header("Audio Source")]
    public AudioClip huhClip;
    private void Start()
    {
        if(canvasButton!=null)
        canvasButton.onClick.AddListener(NextDialgoue);

        defaultCamDistance = FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance ;

    }
    Coroutine CR_Dialgoue;

    public void SetAggresive(bool flag)
    {
        isAggresive = flag; 
    }
    public void NextDialgoue()
    {
        

        if(CR_Dialgoue != null)
        {
            StopCoroutine(CR_Dialgoue);
            dialogueTMP.text = activeDialogue[dialogueCounter];
            CR_Dialgoue = null;
        }
        else
        {
            if (narasi.Count != 0 && narasi[dialogueCounter] != null)
            {
                GetComponentInParent<AudioSource>().Stop();
            }

            if (dialogueCounter + 1 == activeDialogue.Count)
            {
               
                FindObjectOfType<CinemachineVirtualCamera>().Follow = FindObjectOfType<ThirdPersonController>().GetComponent<CameraPosition>().cameraPos;
                FindObjectOfType<UICanvasControllerInput>(true).gameObject.SetActive(true);
                dialogueCanvas.SetActive(false);
                if (interacted)
                {
                    LocalPlayer.instance.mainAvatar.SetActive(true);

                    OnDialogueAfterDone.Invoke();
                    FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = defaultCamDistance;

                }
                else
                {
                    LocalPlayer.instance.mainAvatar.SetActive(true);
                    OnDialogueDone.Invoke();
                    FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = defaultCamDistance;
                }
                interacted = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                DeactivateInteractButton();
                return;
            }
            dialogueCounter++;
            

            dialogueTMP.text = "";
            CR_Dialgoue = StartCoroutine(IE_Type());
            if (narasi.Count != 0 && narasi[dialogueCounter] != null)
            {
                GetComponentInParent<AudioSource>().PlayOneShot(narasi[dialogueCounter]);
            }
        }

    }

    public IEnumerator IE_Type()
    {
        int textLength = activeDialogue[dialogueCounter].Length;
        int counter = 0;
        while(counter != textLength)
        {
            dialogueTMP.text += activeDialogue[dialogueCounter][counter];
            yield return new WaitForSeconds(dialogueSpeed);
            counter++;
        }
        CR_Dialgoue = null;
        yield return null;
    }

    public void ShowDialgoue()
    {
        if (cameraPosition != null)
        {
            FindObjectOfType<CinemachineVirtualCamera>().Follow = cameraPosition;
            defaultCamDistance = FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance;
            FindObjectOfType<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = 3;
        }

        FindObjectOfType<UICanvasControllerInput>(true).gameObject.SetActive(false);
        LocalPlayer.instance.mainAvatar.SetActive(false);

        activeDialogue = interacted ?  dialogueAfter: dialogues;  
        dialogueCounter = 0;
        dialogueName.text = npcName;
        dialogueTMP.text = "";

        if(npcName != "")
        {
            FindObjectOfType<AudioManager>().sfxAudioSource.PlayOneShot(huhClip);
        }

        CR_Dialgoue = StartCoroutine(IE_Type());
        if (narasi.Count != 0 && narasi[dialogueCounter] != null)
        {
            GetComponentInParent<AudioSource>().PlayOneShot(narasi[dialogueCounter]);
        }
        //dialogueTMP.text = activeDialogue[0];
        dialogueCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void InitateInteractButton()
    {
        if (isAggresive)
        {
            ShowDialgoue();
        }
        else
        {
            FindObjectOfType<InteractButton>(true).ActivateButton(ShowDialgoue);
        }
    }
    public void DeactivateInteractButton()
    {
        FindObjectOfType<InteractButton>(true).DeactivateButton();
    }

    public void DestroyInstance()
    {
        Destroy(gameObject);
    }
}
