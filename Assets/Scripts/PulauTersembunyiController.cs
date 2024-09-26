using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class PulauTersembunyiController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform tpPoint;
    public Transform tpPointDermaga;


    public async void TeleportHere()
    {
        FindObjectOfType<InteractButton>(true).gameObject.SetActive(false);
        FindObjectOfType<ThirdPersonController>().allowedToMove = false;
        await FadeCanvasController.instance.FadeOut();



        var player = FindObjectOfType<ThirdPersonController>();
        player.transform.SetPositionAndRotation(tpPoint.position, tpPoint.rotation);
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.GetComponent<NavMeshAgent>().enabled = false);
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation));
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.GetComponent<NavMeshAgent>().enabled = true);

        await Task.Delay(2000);
        await FadeCanvasController.instance.FadeIn();
        FindObjectOfType<InteractButton>(true).gameObject.SetActive(true);
        FindObjectOfType<ThirdPersonController>().allowedToMove = true;

        SaveFileController.instance.Save();
    }

    public async void TeleportToDermaga()
    {
        FindObjectOfType<InteractButton>(true).gameObject.SetActive(false);
        FindObjectOfType<ThirdPersonController>().allowedToMove = false;
        await FindObjectOfType<FadeCanvasController>().FadeOut();



        var player = FindObjectOfType<ThirdPersonController>();
        player.transform.SetPositionAndRotation(tpPointDermaga.position, tpPointDermaga.rotation);
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.GetComponent<NavMeshAgent>().enabled = false);
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation));
        FindObjectsOfType<CharacterFollower>().ToList().ForEach(x => x.GetComponent<NavMeshAgent>().enabled = true);

        await Task.Delay(2000);
        await FindObjectOfType<FadeCanvasController>().FadeIn();
        FindObjectOfType<InteractButton>(true).gameObject.SetActive(true);
        FindObjectOfType<ThirdPersonController>().allowedToMove = true;

        if(GlobalInventory.instance.armors.Find(x=>x.id =="lamina" ) == null)
        {
            var lamina = new GameData.Armor() { id = "lamina" ,owner = AvatarDatas.instance.avatars.Find(x => x.choosenAvatar == AvatarController.AVATAR.HANGTUAH) };
            GlobalInventory.instance.armors.Add(lamina);
            AvatarDatas.instance.avatars.Find(x => x.choosenAvatar == AvatarController.AVATAR.HANGTUAH).stats.armor.id = lamina.id;
        }
        if(GlobalInventory.instance.armors.Find(x => x.owner == AvatarDatas.instance.avatars.Find(x => x.choosenAvatar == AvatarController.AVATAR.HANGTUAH)) != null)
        GlobalInventory.instance.armors.Find(x => x.owner == AvatarDatas.instance.avatars.Find(x => x.choosenAvatar == AvatarController.AVATAR.HANGTUAH)).owner = null;

        GlobalInventory.instance.armors.Find(x => x.id == "lamina").owner = AvatarDatas.instance.avatars.Find(x => x.choosenAvatar == AvatarController.AVATAR.HANGTUAH);
        AvatarDatas.instance.avatars.Find(x => x.choosenAvatar == AvatarController.AVATAR.HANGTUAH).stats.armor.id = "lamina";

        SaveFileController.instance.Save();
    }


}
