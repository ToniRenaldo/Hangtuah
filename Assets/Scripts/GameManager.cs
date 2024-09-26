using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int framerate;
    void Start()
    {
        Application.targetFrameRate = framerate;    
    }

    #region Inventory

    public void OpenInventory()
    {
        FindObjectOfType<InventoryController>(true).OpenInventory();
    }
    public void OpenQuests()
    {
        FindObjectOfType<QuestCanvasController>(true).OpenQuestPanel();

    }
    #endregion

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void BeriKeris()
    {
        var globInv = FindObjectOfType<GlobalInventory>();
        if (globInv.weapons.Find(x => x.id == "keris5") == null)
        {
            var keris = new GameData.Weapon() { id = "keris5", owner = null , avatar = AvatarController.AVATAR.None};
            globInv.weapons.Add(keris);
        }
    }
    #region Battle


    #endregion
}
