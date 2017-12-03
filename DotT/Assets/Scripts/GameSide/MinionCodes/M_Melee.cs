using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Melee : Minion {
    float attackRange = 3f;
    public int damage = 10;

    override public void Attack() {
        attackFocus.gameObject.GetComponent<Health>().Damage(damage, Health.DamageType.real);
    }
}
