using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAdjuster : MonoBehaviour
{
    [System.Serializable]
    public class WeaponPos
    {
        public GameData.WeaponType type;
        public Transform template;        
    }

    public List<WeaponPos> weaponList;
    public GameObject activeWeapon;
    public void SetupWeapon(GameData.Weapon prefab)
    {
        if(activeWeapon != null)
            Destroy(activeWeapon);  
        Animator ac= GetComponent<AvatarController>().animator;
        Transform hand = ac.GetBoneTransform(HumanBodyBones.RightHand);
        GameObject weapon = Instantiate(prefab.weaponPrefab, hand);
        activeWeapon = weapon;
        Transform template = weaponList.Find(x => x.type == prefab.type).template ;
        weapon.transform.SetLocalPositionAndRotation(template.localPosition, template.localRotation);      
    }
}
