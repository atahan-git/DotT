using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Health : NetworkBehaviour
{
    [SyncVar]
    public bool canCastSpell = true;
    [SyncVar]
    public bool canAutoAttack = true;

    [SyncVar]
    public int movementSpeed = 100;
    [SyncVar]
    public int slowAmount = 0;
    
    [SyncVar]
    public int myHealth = 0;
    [SyncVar]
    public int physicalArmor = 0;
    [SyncVar]
    public int magicArmor = 0;

    public enum DamageType { physical, magic, real };

    public void Damage(int damage, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.physical:
                myHealth -= damage * (100 - physicalArmor);
                break;

            case DamageType.magic:
                myHealth -= damage * (100 - magicArmor);
                break;

            case DamageType.real:
                myHealth -= damage;
                break;
        }
    }

    public void Damage(int damage, DamageType damageType, float duration)
    {
        damage = (int)(damage / (2 * duration));

        StartCoroutine(DamageOverTime(damage, damageType, duration));
    }

    IEnumerator DamageOverTime(int damage, DamageType damageType, float duration)
    {
        while (duration > 0)
        {
            yield return new WaitForSeconds(0.5f);
            switch (damageType)
            {
                case DamageType.physical:
                    myHealth -= damage * (100 - physicalArmor);
                    break;

                case DamageType.magic:
                    myHealth -= damage * (100 - magicArmor);
                    break;

                case DamageType.real:
                    myHealth -= damage;
                    break;
            }
        }
    }

    public void Slow(int slowAmount, float duration)
    {
        movementSpeed *= 100 - slowAmount;
        StartCoroutine(SlowDuration(slowAmount, duration));
    }

    IEnumerator SlowDuration(int slowAmount, float duration)
    {
        yield return new WaitForSeconds(duration);
        movementSpeed /= 100 - slowAmount;
    }

    public void Silence(float duration)
    {
        canCastSpell = false;
        StartCoroutine(SilenceDuration(duration));
    }

    IEnumerator SilenceDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        canCastSpell = true;
    }

    public void Stun(float duration)
    {
        int i = movementSpeed;
        movementSpeed = 0;
        canCastSpell = false;
        canAutoAttack = false;
        StartCoroutine(StunDuration(duration, i));
    }

    IEnumerator StunDuration(float duration, int i)
    {
        yield return new WaitForSeconds(duration);
        movementSpeed = i;
        canCastSpell = true;
        canAutoAttack = true;
    }
}