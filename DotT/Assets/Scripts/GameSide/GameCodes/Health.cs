using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class Health : NetworkBehaviour
{
    //public Text armorText;
    //public Text hpModText;
    //public Text hpText;

    //========== Properties ==========

    public enum Type {minion, hero, fort, keep, towerFort, towerKeep, nexus, jungle};
    public enum Side {blue, red, green, neutral};
    public Type myType = Type.jungle;
    public Side mySide = Side.neutral;
    public bool isLocalPlayerHealth;

	public IRespawnManager myRespawnManager;
	public bool isDead = false;

    //========== Crowd Controls ==========

    //Blind
    bool canAutoAttack = true;
    float blindDuration = 0;

    //Silence
    bool canCastSpell = true;
    float silenceDuration = 0;

    //Movement
    float movementSpeed = 100;

    //Root
    NavMeshAgent agent;
    bool agentIsEnabled = true;
    float rootDuration;

    //Unstoppable
    bool canTakeCC = true;
    float unstopDuration = 0;

    //Invulnerable
    bool canTakeDamage = true;
    float invulnerabilityDuration = 0;

	//========== Defensive Stats ==========
	[SyncVar]
    public float maximumHealth;
    [SyncVar]
    public float currentHealth = 1000;

    Dictionary<HpModType, float> DefensiveStats = new Dictionary<HpModType, float>()
    {
        {HpModType.physicalDamage, 0},
        {HpModType.magicalDamage, 0},
        {HpModType.heal, 0},
    };

    //========== HealthModification ==========
    public enum HpModType { physicalDamage, magicalDamage, trueDamage, heal };
    public enum RatioType { current, missing, maximum };
    public float ticksPerSecond = 4;

    //HealthBarLines
    float healthBarOffSet = 100;
    float linesPerMileStone = 10;

    //HealthBarImages
    Image orangeBar;
    Image greenBar;
    Image mainBar;
    Image greyBar;

    GameObject healthBar;
    float healthBarWidth;

    //HpModOverTime
    Dictionary<HpModType, float> HpModOverTime = new Dictionary<HpModType, float>()
    {
        {HpModType.physicalDamage, 0},
        {HpModType.magicalDamage, 0},
        {HpModType.trueDamage, 0},
        {HpModType.heal, 0},
    };

    float hpModOverTime;

    //HpMod
    float animatingDamage;
    float animatingHeal;

    //============================================================== void Start() ==============================================================

    void Start()
    {
        switch (myType)
        {
        case Type.minion:
            Debug.Log("Type Error");
            break;

        case Type.hero:
            healthBar = Instantiate(STORAGE_HealthPrefabs.s.heroHealthBar);
            break;

		default:
			Debug.Log ("Type Error");
			break;
        }

        healthBar.transform.SetParent(gameObject.transform, false);

        orangeBar = healthBar.transform.Find("Orange").GetComponent<Image>();
        greenBar = healthBar.transform.Find("Green").GetComponent<Image>();
        mainBar = healthBar.transform.Find("Main").GetComponent<Image>();
        greyBar = healthBar.transform.Find("Grey").GetComponent<Image>();

        maximumHealth = currentHealth;
        if (mainBar != null)
        {
            healthBarWidth = mainBar.GetComponent<RectTransform>().sizeDelta.x;
            SetHealthBarLines();
        }
        agent = GetComponent<NavMeshAgent>();

		SetColor ();
    }


	void SetColor (){
		if (mySide != PlayerSpawner.LocalPlayerSpawner.mySide) {
			if (mySide == Side.red || (PlayerSpawner.LocalPlayerSpawner.mySide == Side.red && mySide == Side.blue))
				mainBar.color = STORAGE_HealthPrefabs.s.enemyColor1;
			else
				mainBar.color = STORAGE_HealthPrefabs.s.enemyColor2;
		}
	}

    //============================================================== void Update() ==============================================================

    void Update()
    {
        //Clamps
        currentHealth = Mathf.Clamp(currentHealth, 0, maximumHealth);

        animatingDamage = Mathf.Clamp(animatingDamage, 0, maximumHealth);
        animatingHeal = Mathf.Clamp(animatingHeal, 0, maximumHealth);

		//============= Diedededing check =============

		if (isServer) {
			if (currentHealth <= 0 && !isDead) {
				if (myRespawnManager != null)
					myRespawnManager.Die (this);

				XPMaster.s.AddXp (mySide, myAttackerSides.ToArray(), myType); //ATTACKER SIDE LOGIC
				isDead = true;
			}
		}


        //========== Health Bar Modifcation ==========

        //Calculate hpModOverTime
        foreach (HpModType type in HpModOverTime.Keys)
        {
            hpModOverTime += FindRealAmount(HpModOverTime[type], type);
        }

        //Text
        /*if(armorText)
            armorText.text = "DefStats: " + DefensiveStats[HpModType.physicalDamage] + "/"
           + DefensiveStats[HpModType.magicalDamage] + "/" + DefensiveStats[HpModType.heal];

        if (hpModText)
            hpModText.text = "HpModOT: " + Mathf.RoundToInt(HpModOverTime[HpModType.physicalDamage]) + "/"
            + Mathf.RoundToInt(HpModOverTime[HpModType.magicalDamage]) + "/" + Mathf.RoundToInt(HpModOverTime[HpModType.trueDamage]) + "/"
            + Mathf.RoundToInt(HpModOverTime[HpModType.heal]) + "/" + Mathf.RoundToInt(hpModOverTime);

        if (hpText)
            hpText.text = Mathf.RoundToInt(currentHealth).ToString();*/

        //Bar fillAmounts
        if (hpModOverTime > 0)
        {
            greenBar.fillAmount = (currentHealth + animatingDamage - animatingHeal + hpModOverTime) / maximumHealth;
            orangeBar.fillAmount = 0;
            mainBar.fillAmount = (currentHealth + animatingDamage - animatingHeal) / maximumHealth;
        }
        else if (hpModOverTime < 0)
        {
            greenBar.fillAmount = 0;
            orangeBar.fillAmount = (currentHealth + animatingDamage - animatingHeal) / maximumHealth;
            mainBar.fillAmount = (currentHealth + animatingDamage - animatingHeal + hpModOverTime) / maximumHealth;
        }
        else
        {
            greenBar.fillAmount = 0;
            orangeBar.fillAmount = 0;
            mainBar.fillAmount = (currentHealth + animatingDamage - animatingHeal) / maximumHealth;
        }
        hpModOverTime = 0;

        //Main Bar Decay
        if (animatingDamage > 0 && mainBar != null)
        {
            animatingDamage -= (animatingDamage + 400) * Time.deltaTime * 2;
        }
        if (animatingHeal > 0 && mainBar != null)
        {
            animatingHeal -= (animatingHeal + 400) * Time.deltaTime * 2;
        }

        //========== Crowd Controls ==========

        //Blind
        if (blindDuration > 0)
        {
            blindDuration -= Time.deltaTime;
        }
        else
        {
            RemoveBlind();
        }

        //Silence
        if (silenceDuration > 0)
        {
            silenceDuration -= Time.deltaTime;
        }
        else
        {
            RemoveSilence();
        }

        //Root
        if (rootDuration > 0)
        {
            rootDuration -= Time.deltaTime;
        }
        else
        {
            RemoveRoot();
        }
        if (agent != null)
        {
            if (agentIsEnabled)
            {
                agent.enabled = true;
            }
            else
            {
                agent.enabled = false;
            }
        }

        //Unstoppable
        if (unstopDuration > 0)
        {
            unstopDuration -= Time.deltaTime;
        }
        else
        {
            RemoveUnstoppability();
        }

        //Invulnerable
        if (invulnerabilityDuration > 0)
        {
            invulnerabilityDuration -= Time.deltaTime;
        }
        else
        {
            RemoveInvulnerability();
        }
    }

    //========================================================= Crowd Control Methods ==========================================================

    //Adds blind effect.
    public void Blind(float duration)
    {
        if (blindDuration < duration && canTakeCC)
        {
            canAutoAttack = false;
			blindDuration = duration;
        }
    }

    //Removes blind effect.
    void RemoveBlind()
    {
        canAutoAttack = true;
    }

    //Adds silence effect.
    public void Silence(float duration)
    {
        if (silenceDuration < duration && canTakeCC)
        {
            canCastSpell = false;
            silenceDuration = duration;
        }
    }

    //Removes silence effect.
    void RemoveSilence()
    {
        canCastSpell = true;
    }

    //Modifies movement speed.
    public void ModifyMovementSpeed(float ratio, float duration)
    {
        movementSpeed *= 100 + ratio;
        StartCoroutine(RemoveMovementModifier(ratio, duration));
    }

    //Removes the movement modifier's effect.
    IEnumerator RemoveMovementModifier(float ratio, float duration)
    {

        while (duration > 0 && !canTakeCC)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        movementSpeed /= 100 + ratio;
    }

    //Adds root effect.
    public void Root(float duration)
    {
        if (rootDuration < duration && canTakeCC)
        {
            agentIsEnabled = false;
            rootDuration = duration;
        }
    }

    //Removes root effect.
    void RemoveRoot()
    {
        agentIsEnabled = true;
    }

    //Makes unstoppable.
    public void MakeUnstoppable(float duration)
    {
        if (unstopDuration < duration)
        {
            canTakeCC = false;
            unstopDuration = duration;
            RemoveBlind();
            RemoveSilence();
            RemoveRoot();
        }
    }

    //Makes stoppable.
    void RemoveUnstoppability()
    {
        canTakeCC = true;
    }

    //Makes invulnerabe.
    public void MakeInvulnerable(float duration)
    {
        greyBar.fillAmount = 1;
        if (invulnerabilityDuration < duration)
        {
            canTakeDamage = false;
            invulnerabilityDuration = duration;
        }
    }

    //Makes vulnerable.
    void RemoveInvulnerability()
    {
        if(greyBar)
        {
            greyBar.fillAmount = 0;
        }
        canTakeDamage = true;
    }

    //Adds disarm effect.
    //RemoveBlind and RemoveSilence removes disarm effect automatically.
    public void Disarm(float duration)
    {
        Blind(duration);
        Silence(duration);
    }

    //Adds stun effect.
    //RemoveBlind, RemoveSilence, and RemoveRoot removes stun effect automatically.
    public void Stun(float duration)
    {
        Blind(duration);
        Silence(duration);
        Root(duration);
    }

    //========================================================= Defensive Stat Modification =========================================================

    //Adds a defensive stat modifier.
    public void AddDefensiveStatModifier(float amount, HpModType type)
    {
        DefensiveStats[type] = (DefensiveStats[type] - 100) * (1 - amount / 100) + 100;
    }

    //Adds a defensive stat modifier for a limited time.
    public void AddDefensiveStatModifier(float amount, HpModType type, float duration)
    {
        DefensiveStats[type] = (DefensiveStats[type] - 100) * (1 - amount / 100) + 100;

        StartCoroutine(RemoveDefensiveStatModifier(amount, type, duration));
    }

    //Removes the defensive stat modifier.
    IEnumerator RemoveDefensiveStatModifier(float amount, HpModType type, float duration)
    {
        yield return new WaitForSeconds(duration);

        DefensiveStats[type] = (DefensiveStats[type] - 100) / (1 - amount / 100) + 100;
    }

    //========================================================= Health Modification =========================================================

    //Calculates defensive stats.
    float FindRealAmount(float amount, HpModType type)
    {
        switch (type)
        {
            case HpModType.physicalDamage:
                amount *= (100 - DefensiveStats[HpModType.physicalDamage]) / -100;
                break;

            case HpModType.magicalDamage:
                amount *= (100 - DefensiveStats[HpModType.magicalDamage]) / -100;
                break;

            case HpModType.trueDamage:
                amount = -amount;
                break;

            case HpModType.heal:
                amount *= (100 - DefensiveStats[HpModType.heal]) / 100;
                break;
        }
        return amount;
    }

    //Calculates ratio
    float FindRealAmount(float amount, RatioType ratioType)
    {
        switch (ratioType)
        {
            case RatioType.current:
                amount *= currentHealth / 100;
                break;

            case RatioType.missing:
                amount *= (maximumHealth - currentHealth) / 100;
                break;

            case RatioType.maximum:
                amount *= maximumHealth / 100;
                break;
        }
        return amount;
    }

    void SetHealthBarValues(float amount, HpModType type)
    {
        HpModOverTime[type] += amount;
    }

	List<Side> myAttackerSides = new List<Side> ();//ATTACKER SIDE LOGIC
    //Deals damage or heals.
	public void ModifyHealth(float amount, HpModType type, Side attackerSide)
    {
		if(!myAttackerSides.Contains(attackerSide))
			myAttackerSides.Add(attackerSide);  //ATTACKER SIDE LOGIC

        amount = FindRealAmount(amount, type);

        if (canTakeDamage || type == HpModType.heal)
        {
            currentHealth += amount;

            if (type == HpModType.heal)
            {
                animatingHeal += amount;
            }
            else
            {
                animatingDamage -= amount;
            }
        }
    }

    //Modifies health over time.
    public void ModifyHealth(float amount, HpModType type, float duration)
    {
        amount = (amount / ((ticksPerSecond * duration) + 1));

        SetHealthBarValues(amount * ((ticksPerSecond * duration) + 1), type);

        StartCoroutine(ModifyHealthOverTime(amount, type, duration));
    }

    IEnumerator ModifyHealthOverTime(float amount, HpModType type, float duration)
    {
        while (duration >= 0)
        {
            if (canTakeDamage || type == HpModType.heal)
            {
                currentHealth += FindRealAmount(amount, type);
            }

            SetHealthBarValues(-amount, type);
            SetHealthBarLines();

            duration -= 1 / ticksPerSecond;

            yield return new WaitForSeconds(1 / ticksPerSecond);
        }
    }

    //Modifiels health proportionally.
    public void ModifyHealth(float amount, HpModType type, RatioType ratioType)
    {
        ModifyHealth(FindRealAmount(amount, ratioType), type);
    }

    //====================================================== Health Bar LineArrangement ======================================================

    //Overwrites the health bar.
    void SetHealthBarLines()
    {
        //Destroy Children
        int childs = mainBar.transform.childCount;

        for (int i = 0; i < childs; i++)
        {
            Destroy(mainBar.transform.GetChild(i).gameObject);
        }

        //Lines
        for (int i = 1; i < Mathf.FloorToInt(maximumHealth / healthBarOffSet) + 1; i++)
        {
            GameObject myLine = STORAGE_HealthPrefabs.s.healthBarLine;

            if (0 == i % linesPerMileStone)
            {
                myLine = STORAGE_HealthPrefabs.s.mileStoneLine;
            }

            Instantiate(myLine, new Vector3(i * healthBarWidth * healthBarOffSet / maximumHealth, 0, 0), transform.rotation).transform.SetParent(mainBar.transform, false);
        }
    }
}