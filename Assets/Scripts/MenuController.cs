using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ConfirmationCanvas;
    public Button ContinueButton;
    public Button StartButton;
    public Button YesStartOverButton;
    private void Start()
    {
        StartButton.onClick.AddListener(StartGame);
        YesStartOverButton.onClick.AddListener(ConfirmDeleteSaveFile);

        if (PlayerPrefs.HasKey("PlayerPosition"))
        {
            ContinueButton.interactable = true;
        }
    }
    public void StartGame()
    {
        if (PlayerPrefs.HasKey("PlayerPosition"))
        {
            ConfirmationCanvas.SetActive(true);
        }
        else
        {
            PlayGame();
        }
    }
    public void ConfirmDeleteSaveFile()
    {
        ResetSave();
        PlayGame();
    }

    [ContextMenu("Reset Save")]
    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
    }

    public async void PlayGame()
    {
        await FadeCanvasController.instance.FadeOut();
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
