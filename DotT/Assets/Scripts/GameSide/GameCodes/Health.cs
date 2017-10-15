using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using System.Collections;

public class Health : NetworkBehaviour
{
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
    float maximumHealth = 1000;
    float currentHealth = 1000;
    float physicalArmor = 0;
    float magicalArmor = 0;

    //Damage
    public enum DamageType{physical, magical, real};
    public enum RatioType{current, missing, maximum};

    //Invulnerable
    bool canTakeDamage = true;
    float invulDuration = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
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
        if(agentIsEnabled)
        {
            agent.enabled = true;
        }
        else
        {
            agent.enabled = false;
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
        if (invulDuration > 0)
        {
            invulDuration -= Time.deltaTime;
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

    //Increases or decreases movement speed.
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

    //Adds a defensive stat modifier.
    void AddDefensiveStatModifier(float amount, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.physical:
                physicalArmor = (physicalArmor - 100) * (1 - amount / 100) + 100;
                break;

            case DamageType.magical:
                magicalArmor = (magicalArmor - 100) * (1 - amount / 100) + 100;
                break;

            case DamageType.real:
                physicalArmor = (physicalArmor - 100) * (1 - amount / 100) + 100;
                magicalArmor = (magicalArmor - 100) * (1 - amount / 100) + 100;
                break;
        }
    }

    //Adds a defensive stat modifier for a limited time.
    void AddDefensiveStatModifier(float amount, DamageType damageType, float duration)
    {
        switch(damageType)
        {
            case DamageType.physical:
                physicalArmor = (physicalArmor - 100) * (1 - amount / 100) + 100;
                break;

            case DamageType.magical:
                magicalArmor = (magicalArmor - 100) * (1 - amount / 100) + 100;
                break;

            case DamageType.real:
                physicalArmor = (physicalArmor - 100) * (1 - amount / 100) + 100;
                magicalArmor = (magicalArmor - 100) * (1 - amount / 100) + 100;
                break;
        }
        StartCoroutine(RemoveDefensiveStatModifier(amount, damageType, duration));
    }


    //Removes the defensive stat modifier.
    IEnumerator RemoveDefensiveStatModifier(float amount, DamageType damageType, float duration)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        switch (damageType)
        {
            case DamageType.physical:
                physicalArmor = (physicalArmor - 100) / (1 - amount / 100) + 100;
                break;

            case DamageType.magical:
                magicalArmor = (magicalArmor - 100) / (1 - amount / 100) + 100;
                break;

            case DamageType.real:
                physicalArmor = (physicalArmor - 100) / (1 - amount / 100) + 100;
                magicalArmor = (magicalArmor - 100) / (1 - amount / 100) + 100;
                break;
        }
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

    //Deals damage.
    public void Damage(float damage, DamageType damageType)
    {
        if(canTakeDamage)
        {
            switch(damageType)
            {
                case DamageType.physical:
                    currentHealth -= damage * (100 - physicalArmor);
                    break;

                case DamageType.magical:
                    currentHealth -= damage * (100 - magicalArmor);
                    break;

                case DamageType.real:
                    currentHealth -= damage;
                    break;
            }
        }
    }

    //Deals damage over time.
    public void Damage(float damage, DamageType damageType, float duration)
    {
        damage /= 2 * duration;

        StartCoroutine(DamageOverTime(damage, damageType, duration));
    }

    IEnumerator DamageOverTime(float damage, DamageType damageType, float duration)
    {
        while(duration >= 0)
        {
            yield return new WaitForSeconds(0.5f);
            duration -= 0.5f;
            Damage(damage, damageType);
        }
    }

    //Deals damage proportional to health.
    public void Damage(float damage, DamageType damageType, RatioType ratioType)
    {
        switch(ratioType)
        {
            case RatioType.current:
                damage *= currentHealth;
                break;

            case RatioType.missing:
                damage *= maximumHealth - currentHealth;
                break;

            case RatioType.maximum:
                damage *= maximumHealth;
                break;
        }

        Damage(damage, damageType);
    }

    //Deals damage proportional to health over time.
    public void Damage(float damage, DamageType damageType, RatioType ratioType, float duration)
    {
        switch(ratioType)
        {
            case RatioType.current:
                damage *= currentHealth;
                break;

            case RatioType.missing:
                damage *= maximumHealth - currentHealth;
                break;

            case RatioType.maximum:
                damage *= maximumHealth;
                break;
        }

        Damage(damage, damageType, duration);
    }

    //Makes invulnerabe.
    public void MakeInvulnerable(float duration)
    {
        if (invulDuration < duration)
        {
            canTakeDamage = false;
            invulDuration = duration;
        }
    }

    //Makes vulnerable.
    void RemoveInvulnerability()
    {
        canTakeDamage = true;
    }
}