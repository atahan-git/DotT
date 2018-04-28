using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackProjectile : MonoBehaviour {

	public Health.Side mySide = Health.Side.neutral;
	public Health target;

	public float damage = 0f;
	public float speed = 5f;

	bool isDealtDmg = false;

	// Use this for initialization
	void Start () {
		
	}

	void OnEnable(){
		isDealtDmg = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			transform.position = Vector3.MoveTowards (transform.position, target.transform.position, speed * Time.deltaTime);

			if (GetComponentInParent<UnityEngine.Networking.NetworkIdentity> ().isServer) {
				if (Vector3.Distance (transform.position, target.transform.position) < 0.2f && !isDealtDmg) {
					target.Damage(damage, Health.HpModType.physicalDamage, mySide);
					target = null;
					GetComponentInParent<PooledObject> ().DestroyPooledObject ();
					isDealtDmg = true;
				}
			}
		}
		if (GetComponentInParent<UnityEngine.Networking.NetworkIdentity> ().isServer && target == null) {
			GetComponentInParent<PooledObject> ().DestroyPooledObject ();
		}
	}
}
