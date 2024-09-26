using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
    
public class AvatarController : MonoBehaviour
{
    // Start is called before the first frame update

    public enum AnimationType
    {
        IDLE, // done
        HURT, // done
        DEAD, // done
        ATTACK1, // done
        ATTACK2, // done
        DEFEND,
        USEITEM
    }
    [System.Serializable]
    public class ActiveEffect
    {
        public ActiveEffect(GameData.Item item)
        {
            effect = item.effect;
            point = item.value;
            duration = item.duration;
        }
        public GameData.ItemEffect effect;
        public int point;
        public int duration;
    }
    [System.Serializable]
    public class Stats
    {
        public string avatarName;
        public int defaultHP;
        public int currentHP;
        public int defaultAP;
        public int currentAP;
        public GameData.Armor armor = new GameData.Armor();
        public GameData.Weapon weapon1 = new GameData.Weapon();
        public GameData.Weapon weapon2 = new GameData.Weapon();
        public List<ActiveEffect> activeEffect = new List<ActiveEffect>();
        public List<GameData.Item> items = new List<GameData.Item>();
    }
    [System.Serializable]
    public class Animation
    {
        public AnimationType AnimationType;
        public string stateName;
    }

    [System.Serializable]
    public class AvatarList
    {
        public AVATAR type;
        public GameObject avatar;
        public Sprite avatarSprite;
    }
    public enum AVATAR
    {
        HANGLEKIR,
        HANGTUAH,
        HANGJEBAT,
        HANGLEKIU,
        NPC1_1,
        NPC1_2,
        NPC1_3,
        NPC2_1,
        NPC2_2,
        NPC2_3,
        RAJA,
        ROYALTY_1,
        ROYALTY_2,
        BENDAHARA,
        PANGERAN,
        None
    }
    public List<Animation> animationTypes = new List<Animation>();
    public List<AvatarList> avatarList = new List<AvatarList>();

    [Header("UI")]
    public GameObject playerTurnCanvas;
    public GameObject hitArmorParticle;
    public GameObject hitHealthParticle;

    public GameObject attackParticle;
    public GameObject healParticle;
    public GameObject effectParticle;
    public GameObject defendParticle;
    public GameObject deathParticle;
    public GameObject increaseDamageParticle;
    public Transform statPlace;
    public Transform attackPlace;
    
    [Header("SFX")]
    public AudioClip hitSfx;
    public AudioClip attackSfx;
    public AudioClip drinkSfx;
    public AudioClip deathSfx;
    [Header("Data")]
    public AVATAR choosenAvatar;
    public GameObject choosenAvatarObject;
    public Stats stats = new Stats();
    public string avatarName;
    public bool isDefending;
    public UnityEvent<string> OnTurnActive;
    [Header("Weapon")]
    public GameData.Weapon activeWeapon;
    

    [Header("Debug")]
    public AVATAR debugState;
    public bool isPlayer;
    public Animator animator;

    public void SetStats(AvatarController spawnedAvatar, GameData.Weapon weapon1Id, GameData.Weapon weapon2Id, GameData.Armor armorName, List<GameData.Item> items)
    {
        stats.defaultHP = spawnedAvatar.stats.defaultHP;
        stats.defaultAP = spawnedAvatar.stats.defaultAP;
        stats.currentAP = spawnedAvatar.stats.currentAP;
        stats.currentHP = spawnedAvatar.stats.currentHP;

        stats.avatarName = spawnedAvatar.stats.avatarName;
        stats.weapon1 = GameData.instance.globalWeapon.Find(x=>x.id == weapon1Id.id);
        stats.weapon2 = GameData.instance.globalWeapon.Find(x => x.id == weapon2Id.id);
        stats.armor = GameData.instance.GetArmor(armorName);
        if(stats.armor == null)
        {
            stats.armor = new GameData.Armor();
        }
        stats.items = spawnedAvatar.stats.items;

        // Set Item
        ChangeWeapon(stats.weapon1);
    }
    public void SetAvatar(AVATAR avatar , bool isPlayer)
    {
        
        avatarList.ForEach(x => x.avatar.SetActive(x.type == avatar));
        choosenAvatar = avatar;
        choosenAvatarObject = avatarList.Find(x=> x.type == avatar).avatar;
        animator = choosenAvatarObject.GetComponent<Animator>();
        this.isPlayer = isPlayer;
    }
    public Sprite GetAvatar()
    {
        if(avatarList.Find(x=>x.type == choosenAvatar) != null)
        {
            return avatarList.Find(x => x.type == choosenAvatar).avatarSprite;
        }
        else
        {
            return null;
        }
    }
    public void ChangeWeapon(GameData.Weapon weapon)
    {
        activeWeapon = weapon;
        GetComponent<WeaponAdjuster>().SetupWeapon(activeWeapon);
        UpdateAnimController();
    }
    public void UpdateAnimController()
    {
        RuntimeAnimatorController anim = GetComponent<AvatarAnimationOverride>().GetAnimator(activeWeapon.type);
        animator.runtimeAnimatorController = anim;
    }


    Coroutine CR_PlayAnimation;
    public void PlayAnimation(string animTrigger)
    {
        if(CR_PlayAnimation != null)
        {
            StopCoroutine(CR_PlayAnimation);
        }
        CR_PlayAnimation = StartCoroutine(IE_PlayAnimation(animTrigger));
    }
    IEnumerator IE_PlayAnimation(string animTrigger)
    {
        animator.applyRootMotion = true;
        animator.SetTrigger(animTrigger);
        if (animTrigger == "DEAD")
        {
            yield break;
        }
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.applyRootMotion = false;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        yield return IE_ResetPosAndRot(choosenAvatarObject.transform);

        CR_PlayAnimation = null;
    }

    public void SetTurn(bool active)
    {
        if (active)
        {
            isDefending = false;
            OnTurnActive.Invoke(avatarName);

            playerTurnCanvas.gameObject.SetActive(true);
            ChangeEffectState();
        }
        else
        {
            playerTurnCanvas.gameObject.SetActive(false);

        }

    }

    public void ChangeEffectState()
    {
        // Removing Active Effect 
        List<ActiveEffect> removeEffect = new List<ActiveEffect>();
        foreach (var effect in stats.activeEffect)
        {
            effect.duration -= 1;
            if (effect.duration == 0)
            {
                removeEffect.Add(effect);
            }
        }
        foreach (var item in removeEffect)
        {
            if (item.effect == GameData.ItemEffect.IncreaseDamage)
                increaseDamageParticle.SetActive(false);
            stats.activeEffect.Remove(item);
        }
    }

    
    
    public GameObject GiveDamage(int damage)
    {
        //GetComponent<DamageController>().ShowDamage(damage);
        if (FindObjectOfType<DynamicTextOnCanvas>() != null)
        {
            Debug.Log("Recieved Damage : " + damage);
            FindObjectOfType<DynamicTextOnCanvas>().ShowValue(transform , damage);
        }
        if (isDefending)
        {
            if(Random.Range(0,100) >= stats.armor.defendChance)
            {
                //Guard Break
            }
            else
            {
                PlayAnimation("DEFEND");
                Instantiate(defendParticle,transform).SetActive(true);
                return null;
            }
        }
        if (stats.currentAP == 0)
        {
            stats.currentHP -= damage;
            Instantiate(hitHealthParticle, transform).SetActive(true);
            PlayAnimation("HIT");

        }
        else if(stats.currentAP - damage < 0)
        {
            stats.currentAP = 0;
            stats.currentHP += stats.currentAP - damage;
            Instantiate(hitHealthParticle, transform).SetActive(true);
            PlayAnimation("HIT");


        }
        else if(stats.currentAP -damage > 0) 
        {
            Instantiate(hitArmorParticle, transform).SetActive(true);
            stats.currentAP -= damage;
            if(stats.currentAP == 0)
            {
                // Armor Break
            }
            PlayAnimation("HIT");

        }
        GetComponent<AudioSource>().PlayOneShot(hitSfx);

        if (stats.currentHP <= 0)
        {
            // Death
            animator.applyRootMotion = true;
            PlayAnimation("DEAD");

            increaseDamageParticle.gameObject.SetActive(false);
            return gameObject;
        }
        else
        {
            return null;
        }
    }
    public int Attack1()
    {
        // Tambahkan effect item disini

        int totalDamage = (int)((float)stats.weapon1.damage * (Random.Range(0.8f, 1.2f)));
     
        if (Random.Range(0,100) > stats.weapon1.criticalStrike)
        {
            totalDamage += Random.Range(10, 20);
        }
        totalDamage = GiveDamageEffect(totalDamage);

        
        PlayAnimation("ATTACK1");
        Instantiate(attackParticle, transform).SetActive(true);

        GetComponent<AudioSource>().PlayOneShot(attackSfx);

        if (isPlayer && GameData.instance.debug)
        {
            return 100;
        }
        return totalDamage;
    }


    public IEnumerator IE_ResetPosAndRot(Transform source)
    {
        while (transform.localPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, 0.1f);
            yield return null;
        }
        while (transform.localRotation != Quaternion.identity)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.identity, 0.1f);
            yield return null;
        }
    }
    public IEnumerator IE_MoveToAttackPos(Transform attackPos)
    {
        while (transform.position != attackPos.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackPos.position, 1f);
            yield return null;
        }
    }
    public IEnumerator IE_ResetPos()
    {
        while (transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, 1f);
            yield return null;
        }
    }
    public int Attack2()
    {
        // Tambahkan effect item disini
        int totalDamage = (int)((float)stats.weapon2.damage * (Random.Range(0.8f,1.2f)));


        if (Random.Range(0, 100) > stats.weapon2.criticalStrike)
        {
            totalDamage += Random.Range(10, 20);
        }

        totalDamage = GiveDamageEffect(totalDamage);

        PlayAnimation("ATTACK2");
        Instantiate(attackParticle, transform).SetActive(true);

        return totalDamage;
    }

    public int GiveDamageEffect(int currentDamage)
    {
        List<ActiveEffect> effects = stats.activeEffect.FindAll(x => x.effect == GameData.ItemEffect.IncreaseDamage);
        foreach (var effect in effects)
        {
            float multiplier = (float)currentDamage * ((float)effect.point / 100f);
            currentDamage += (int)multiplier;
        }

        return currentDamage;
    }
    public void Defend()
    {
        PlayAnimation("DEFEND");
        Instantiate(defendParticle, transform).SetActive(true);

        isDefending = true;
    }
    public void UseItem(GameData.Item item)
    {
        PlayAnimation("USEITEM");
        GetComponent<AudioSource>().PlayOneShot(drinkSfx);

        if (item.effect == GameData.ItemEffect.IncreaseHealth)
        {
            stats.currentHP += item.value;
            stats.currentHP = Mathf.Clamp(0, stats.defaultHP,stats.currentHP);
            Instantiate(healParticle, transform).SetActive(true);

        }
        else if (item.effect == GameData.ItemEffect.IncreaseDamage)
        {
            increaseDamageParticle.gameObject.SetActive(true);
        }

        if (GetComponent<BotController>() != null)
            return;
        Debug.Log("Item ID + : " + item.id);
        GameData.Item usedItem = stats.items.Find(x => x.id == item.id);
        if(usedItem == null)
        {
            Debug.Log("Nothing Erased");
        }
        stats.items.Remove(usedItem);
        FindObjectOfType<RPG_InventoryController>().UpdateItem(item);
        stats.activeEffect.Add( new ActiveEffect(item));
    }

    
}
