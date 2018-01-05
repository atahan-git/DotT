using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public class Test
    {
        public bool execute = false;

    }

    public Test[] tests;

    public bool execute = false;

    public Health subjectHealth;

    public enum Function{ModHp, ModHpPro, AddDefStatMod, MakeInvulnerable};
    public Function function;

    public Health.HpModType hpModType;
    public Health.RatioType ratioType;

    public float amount;
    public float duration;
	
	void Update()
    {
		if(execute)
        {
            execute = false;

            switch(function)
            {
                case Function.ModHp:
                    if(duration > 0)
                    {
                        subjectHealth.ModifyHealth(amount, hpModType, duration);

                    }
                    else
                    {
                        subjectHealth.ModifyHealth(amount, hpModType);

                    }
                    break;

                case Function.ModHpPro:
                    if (duration > 0)
                    {
                        //subjectHealth.ModifyHealth(amount, hpModType, ratioType, duration);
                    }
                    else
                    {
                        subjectHealth.ModifyHealth(amount, hpModType, ratioType);
                    }
                    break;

                case Function.AddDefStatMod:
                    if (duration > 0)
                    {
                        subjectHealth.AddDefensiveStatModifier(amount, hpModType, duration);
                    }
                    else
                    {
                        subjectHealth.AddDefensiveStatModifier(amount, hpModType);
                    }
                    break;

                case Function.MakeInvulnerable:
                    if (duration > 0)
                    {
                        subjectHealth.MakeInvulnerable(duration);
                    }
                    break;
            }
        }
	}
}
