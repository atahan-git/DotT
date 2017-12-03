using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class Health : NetworkBehaviour
{
    //Properties
    public enum Type{minion, hero, tower, nexus, jungle};
    public enum Side{blue, red, neutral};
    public Type myType = Type.jungle;
    public Side mySide = Side.neutral;

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

    //Defensive Stats
    float maximumHealth;
	[SyncVar]
    public float currentHealth = 1000;
    public float physicalArmor = 0;
    public float magicalArmor = 0;
    public float healReduction = 0;

    //HealthModification
    public enum HpModType{physicalDamage, magicalDamage, trueDamage, heal};
    public enum RatioType{current, missing, maximum};
    public float ticksPerSecond = 4;

    //Invulnerable
    bool canTakeDamage = true;
    float invulnerabilityDuration = 0;

    //HealthBar
    public float healthBarOffSet = 100;
    public float linesPerMileStone = 10;
    public Image orangeBar;
    public Image greenBar;
    public Image mainBar;
    public float healthBarWidth;
    public float hpModOverTime;
    public float animatingHpMod;
    public GameObject healthBarLine;
    public GameObject mileStoneLine;

    void Start()
    {
        maximumHealth = currentHealth;
        if(mainBar != null)
        {
            healthBarWidth = mainBar.GetComponent<RectTransform>().sizeDelta.x;
            SetHealthBarLines();
        }
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //Clamps
        currentHealth = Mathf.Clamp(currentHealth, 0, maximumHealth);

        physicalArmor = Mathf.Clamp(physicalArmor, 0, 100);
        magicalArmor = Mathf.Clamp(magicalArmor, 0, 100);
        healReduction = Mathf.Clamp(healReduction, 0, 100);

        animatingHpMod = Mathf.Clamp(animatingHpMod, 0, maximumHealth);

        //Health Bar
        if (hpModOverTime > 0)
        {
            greenBar.fillAmount = (currentHealth + animatingHpMod + hpModOverTime) / maximumHealth;
            orangeBar.fillAmount = 0;
            mainBar.fillAmount = (currentHealth + animatingHpMod) / maximumHealth;
        }
        else if(hpModOverTime < 0)
        {
            greenBar.fillAmount = 0;
            orangeBar.fillAmount = (currentHealth + animatingHpMod) / maximumHealth;
            mainBar.fillAmount = (currentHealth + animatingHpMod + hpModOverTime) / maximumHealth;
        }
        else
        {
            greenBar.fillAmount = 0;
            orangeBar.fillAmount = 0;
            mainBar.fillAmount = (currentHealth + animatingHpMod) / maximumHealth;
        }

        //Main Bar Decay
        if(animatingHpMod > 0 && mainBar != null)
        {
            animatingHpMod -= (animatingHpMod + 400) * Time.deltaTime * 2;
        }

        //========================================================= Crowd Controls and Effects =========================================================

        //Blind
        if(blindDuration > 0)
        {
            blindDuration -= Time.deltaTime;
        }
        else
        {
            RemoveBlind();
        }

        //Silence
        if(silenceDuration > 0)
        {
            silenceDuration -= Time.deltaTime;
        }
        else
        {
            RemoveSilence();
        }

        //Root
        if(rootDuration > 0)
        {
            rootDuration -= Time.deltaTime;
        }
        else
        {
            RemoveRoot();
        }

        //Agent
        if(agent != null)
        {
            if(agentIsEnabled)
            {
                agent.enabled = true;
            }
            else
            {
                agent.enabled = false;
            }
        }

        //Unstoppable
        if(unstopDuration > 0)
        {
            unstopDuration -= Time.deltaTime;
        }
        else
        {
            RemoveUnstoppability();
        }

        //Invulnerable
        if(invulnerabilityDuration > 0)
        {
            invulnerabilityDuration -= Time.deltaTime;
        }
        else
        {
            RemoveInvulnerability();
        }
    }

    //Adds blind effect.
    public void Blind(float duration)
    {
        if(blindDuration < duration && canTakeCC)
        {
            canAutoAttack = false;
            blindDuration = duration;
        }
    }

    //Removes blind effect.
    void RemoveBlind()
    {
        canCastSpell = true;
    }

    //Adds silence effect.
    public void Silence(float duration)
    {
        if(silenceDuration < duration && canTakeCC)
        {
            canCastSpell = false;
            silenceDuration = duration;
        }
    }

    //Removes silence effect.
    void RemoveSilence()
    {
        canAutoAttack = true;
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
        if (invulnerabilityDuration < duration)
        {
            canTakeDamage = false;
            invulnerabilityDuration = duration;
        }
    }

    //Makes vulnerable.
    void RemoveInvulnerability()
    {
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
        switch(type)
        {
            case HpModType.physicalDamage:
                physicalArmor = (physicalArmor - 100) * (1 - amount / 100) + 100;
                break;

            case HpModType.magicalDamage:
                magicalArmor = (magicalArmor - 100) * (1 - amount / 100) + 100;
                break;

            case HpModType.trueDamage:
                physicalArmor = (physicalArmor - 100) * (1 - amount / 100) + 100;
                magicalArmor = (magicalArmor - 100) * (1 - amount / 100) + 100;
                break;

            case HpModType.heal:
                healReduction = (healReduction - 100) * (1 - amount / 100) + 100;
                break;
        }
    }

    //Adds a defensive stat modifier for a limited time.
    public void AddDefensiveStatModifier(float amount, HpModType type, float duration)
    {
        switch(type)
        {
            case HpModType.physicalDamage:
                physicalArmor = (physicalArmor - 100) * (1 - amount / 100) + 100;
                break;

            case HpModType.magicalDamage:
                magicalArmor = (magicalArmor - 100) * (1 - amount / 100) + 100;
                break;

            case HpModType.trueDamage:
                physicalArmor = (physicalArmor - 100) * (1 - amount / 100) + 100;
                magicalArmor = (magicalArmor - 100) * (1 - amount / 100) + 100;
                break;

            case HpModType.heal:
                healReduction = (healReduction - 100) * (1 - amount / 100) + 100;
                break;
        }
        StartCoroutine(RemoveDefensiveStatModifier(amount, type, duration));
    }

    //Removes the defensive stat modifier.
    IEnumerator RemoveDefensiveStatModifier(float amount, HpModType type, float duration)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        switch(type)
        {
            case HpModType.physicalDamage:
                physicalArmor = (physicalArmor - 100) / (1 - amount / 100) + 100;
                break;

            case HpModType.magicalDamage:
                magicalArmor = (magicalArmor - 100) / (1 - amount / 100) + 100;
                break;

            case HpModType.trueDamage:
                physicalArmor = (physicalArmor - 100) / (1 - amount / 100) + 100;
                magicalArmor = (magicalArmor - 100) / (1 - amount / 100) + 100;
                break;

            case HpModType.heal:
                healReduction = (healReduction - 100) * (1 - amount / 100) + 100;
                break;
        }
    }

    //========================================================= Health Modification =========================================================

    //Calculates armors and heal redduction.
    float FindRealAmount(float amount, HpModType type)
    {
        switch(type)
        {
            case HpModType.physicalDamage:
                amount *= (100 - physicalArmor) / 100;
                break;

            case HpModType.magicalDamage:
                amount *= (100 - magicalArmor) / 100;
                break;

            case HpModType.heal:
                amount *= (100 - healReduction) / -100;
                break;
        }
        return amount;
    }

    //Calculates ratio
    float FindRealAmount(float amount, RatioType ratioType)
    {
        switch(ratioType)
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

    float FindRealAmount(float amount, HpModType type, RatioType ratioType)
    {
        amount = FindRealAmount(FindRealAmount(amount, ratioType), type);
        return amount;
    }

    //Deals damage or heals.
    public void ModifyHealth(float amount, HpModType type)
    {
        if(canTakeDamage)
        {
            currentHealth -= FindRealAmount(amount, type);

            if(type != HpModType.heal)
            {
                animatingHpMod += amount;
            }
        }

        if(mainBar != null)
        {
            SetHealthBarLines();
        }
    }

    //Modifies health over time.
    public void ModifyHealth(float amount, HpModType type, float duration)
    {
        amount = (FindRealAmount(amount, type) / ((ticksPerSecond * duration) + 1));

        print(amount);

        hpModOverTime -= amount * ((ticksPerSecond * duration) + 1);

        StartCoroutine(ModifyHealthOverTime(amount, duration));
    }

    IEnumerator ModifyHealthOverTime(float amount, float duration)
    {
        while (duration >= 0)
        {
            if(canTakeDamage || amount < 0)
            {
                currentHealth -= amount;
            }
            hpModOverTime += amount;
            duration -= 1 / ticksPerSecond;
            SetHealthBarLines();

            yield return new WaitForSeconds(1 / ticksPerSecond);
        }
    }

    //Modifiels health proportionally.
    public void ModifyHealth(float amount, HpModType type, RatioType ratioType)
    {
        amount -= FindRealAmount(amount, type, ratioType);
    }

    /*//Modifies health over time proportionally.
    public void ModifyHealth(float amount, HpModType type, RatioType ratioType, float duration)
    {
        amount = FindRealAmount(amount, type);
        amount /= (ticksPerSecond * duration) + 1;

        if (type != HpModType.heal)
        {
            hpModOverTime += FindRealAmount(amount, ratioType) * ticksPerSecond * duration;
        }

        StartCoroutine(ModHealthOverTimePro(amount, type, ratioType, duration));
    }

    IEnumerator ModHealthOverTimePro(float amount, HpModType type, RatioType ratioType, float duration)
    {
        duration -= 1 / ticksPerSecond;
        if (canTakeDamage)
        {
            currentHealth -= FindRealAmount(amount, ratioType);
        }
        SetHealthBarValues();

        while (duration >= 0)
        {
            yield return new WaitForSeconds(1 / ticksPerSecond);

            if (canTakeDamage)
            {
                currentHealth -= FindRealAmount(amount, ratioType);

                if (type != HpModType.heal)
                {
                    hpModOverTime -= FindRealAmount(amount, ratioType);
                }
            }

            SetHealthBarValues();
            duration -= 1 / ticksPerSecond;
        }
    }*/

    //========================================================= Health Bar Arrangement =========================================================

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
            GameObject myLine = healthBarLine;

            if (0 == i % linesPerMileStone)
            {
                myLine = mileStoneLine;
            }

            Instantiate(myLine, new Vector3(i * healthBarWidth * healthBarOffSet / maximumHealth, 0, 0), transform.rotation).transform.SetParent(mainBar.transform, false);
        }
    }
}