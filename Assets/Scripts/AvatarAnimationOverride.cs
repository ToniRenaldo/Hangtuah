using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimationOverride : MonoBehaviour
{
    [System.Serializable]
    public class AnimationType
    {
        public GameData.WeaponType type;
        public RuntimeAnimatorController acOverride;
        
    }
    public List<AnimationType> animations;
    public RuntimeAnimatorController GetAnimator(GameData.WeaponType type)
    {
        return animations.Find(x=>x.type == type).acOverride;
    }
}
