using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedProjectile : MonoBehaviour {

	public float speed = 5f;
	public float distance = 10f;
	[HideInInspector]
	public float damage = 0f;
	public float myHeight = 1f;

	public Health.Side mySide = Health.Side.neutral;

	public Vector3 startPos;
	// Use this for initialization
	void OnEnable () {
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (speed * Time.deltaTime * Vector3.forward, Space.Self);

		if (Vector3.Distance (startPos, transform.position) > distance) {
			GetComponentInParent<PooledObject> ().DestroyPooledObject ();
		}
	}


	void OnTriggerEnter (Collider col){
		if (GetComponentInParent<UnityEngine.Networking.NetworkIdentity> ().isServer) {
			Health hitHealth = col.gameObject.GetComponentInParent<Health> ();
			if (hitHealth == null)
				return;
			if (hitHealth.mySide != mySide) {
				hitHealth.ModifyHealth (damage, Health.HpModType.magicalDamage);
				GetComponentInParent<PooledObject> ().DestroyPooledObject ();
			}

		}
	}
}
