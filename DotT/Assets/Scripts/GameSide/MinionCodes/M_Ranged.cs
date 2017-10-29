using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Ranged : MonoBehaviour {
    override float attackRange = 10f;
    public override int damage = 10;

    override void Attack()
    {
        attackFocus.gameObject.GetComponent<Health>().Damage(damage, Health.DamageType.real);
    }
}
