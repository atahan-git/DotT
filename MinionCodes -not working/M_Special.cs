using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Special : Minion{
    float attackRange = 3f;
    new public int damage = 10;

    void Attack(){
        attackFocus.gameObject.GetComponent<Health>().ModifyHealth(damage, Health.HpModType.trueDamage);
    }
}
