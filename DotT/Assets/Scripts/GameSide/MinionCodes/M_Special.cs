using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Special : Minion{
    override float attackRange = 3f;
    public override int damage = 10;

    override void Attack(){
        attackFocus.gameObject.GetComponent<Health>().Damage(damage, Health.DamageType.real);
    }
}
