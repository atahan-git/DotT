using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public Health target;

	public float damage = 0f;
	public float speed = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.MoveTowards (transform.position, target.transform.position, speed * Time.deltaTime);

		if (Vector3.Distance (transform.position, target.transform.position) < 0.2f) {
			target.Damage (damage, Health.DamageType.physical);
			target = null;
			GetComponent<PooledObject> ().Destroy ();
		}
	}
}
