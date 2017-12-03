using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Ranged : MonoBehaviour {
    float attackRange = 10f;
    public int damage = 10;

	public override void Attack()
    {
        attackFocus.gameObject.GetComponent<Health>().Damage(damage, Health.DamageType.real);
    }
}
