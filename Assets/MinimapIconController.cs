using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinimapIconController : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject camPulau1;
    public GameObject camPulau2;
    public GameObject canvasMap;
    [SerializeField] GameObject pulau1;
    [SerializeField] GameObject pulau2;

    public List<MinimapIcon> iconList;
    void Start()
    {
        iconList = FindObjectsOfType<MinimapIcon>(true).ToList();

        iconList.ForEach(x => x.parent = x.transform.parent.gameObject);

        iconList.ForEach(x=>x.transform.SetParent(transform,true));
        iconList.ForEach(x => x.transform.rotation = Quaternion.Euler(new Vector3(90,0,0)));
        iconList.ForEach(x => x.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f));
    }

    public void OpenMap()
    {
        iconList = FindObjectsOfType<MinimapIcon>(true).ToList();
        var player = FindObjectOfType<ThirdPersonController>().transform;
        var playerIcon = FindObjectOfType<MinimapIconPlayer>(true);
        playerIcon.gameObject.SetActive(true);

        camPulau1.SetActive(pulau1.activeSelf == true && pulau2.activeSelf == false);
        camPulau2.SetActive(pulau2.activeSelf == true);

        iconList.RemoveAll(x => x.parent == null);

        iconList.ForEach(x => x.Setup());

        canvasMap.SetActive(true);

    }

    public void CloseMap()
    {
        canvasMap.SetActive(false);
        camPulau1.SetActive(false);
        camPulau2.SetActive(false);
        iconList.ForEach(x => x.gameObject.SetActive(false));
        FindObjectOfType<MinimapIconPlayer>().gameObject.SetActive(false);

    }

}
