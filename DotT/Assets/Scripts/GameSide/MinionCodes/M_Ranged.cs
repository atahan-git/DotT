using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Ranged : Minion {
    float attackRange = 10f;
    new public int damage = 10;
	
    void Attack()
    {
        attackFocus.gameObject.GetComponent<Health>().ModifyHealth(damage, Health.HpModType.trueDamage);
    }
}
