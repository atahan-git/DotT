using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

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
					target.Damage (damage, Health.DamageType.physical);
					target = null;
					GetComponentInParent<PooledObject> ().Destroy ();
					isDealtDmg = true;
				}
			}
		}
		if (GetComponentInParent<UnityEngine.Networking.NetworkIdentity> ().isServer && target == null) {
			GetComponentInParent<PooledObject> ().Destroy ();
		}
	}
}
