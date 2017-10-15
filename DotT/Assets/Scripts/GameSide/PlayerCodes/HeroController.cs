using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class HeroController : NetworkBehaviour {

	//these are server side only
	public Vector3 movePos = Vector3.zero;
	public Health attackTarget;

	[HideInInspector]
	public float attackSpeed = 1.2f;
	float attackCounter = 0f;
	[HideInInspector]
	public float attackDamage = 50f;
	[HideInInspector]
	public float attackrange = 3f;

	enum MovementMode{stop, move, attackmove}

	MovementMode mode;

	PlayerSpawner spawn;
	GameObject myHero;
	ObjectPool myPool;

	void Start (){
		spawn = GetComponent<PlayerSpawner> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			if (Input.GetMouseButtonDown (1)) {
				RaycastHit hit; 
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
				if (Physics.Raycast (ray, out hit, 100.0f)) { 
					if (hit.transform.root.gameObject.tag == "Hero") {
						Attack (hit.transform.root.gameObject.GetComponent<Health>());
						Stop ();
						mode = MovementMode.attackmove;
						attackCounter = 0;
					} else {
						mode = MovementMode.move;
						Attack (null);
						ChangePos(hit.point);
					}
				}
			}
		}

		if (isServer) {

			if (myHero == null) {
				myHero = spawn.myHero;
				return;
			}

			if (myPool == null) {
				myPool = myHero.GetComponent<ObjectPool> ();
				return;
			}

			switch (mode) {
			case MovementMode.move:
				if (Vector3.Distance (spawn.myHero.transform.position, movePos) < 0.1f)
					mode = MovementMode.stop;

				spawn.myHero.GetComponent<NavMeshAgent> ().enabled = true;
				break;
			case MovementMode.stop:
				spawn.myHero.GetComponent<NavMeshAgent> ().enabled = false;
				break;
			case MovementMode.attackmove:
				if (attackTarget != null) {
					if (Vector3.Distance (spawn.myHero.transform.position, movePos) < attackrange) {
						if (attackCounter <= 0) {
							ShootProjectile ();
							attackCounter = 1f / attackSpeed;
						} else {
							attackCounter -= Time.deltaTime;
						}
					} else {
						movePos = attackTarget.transform.position;
						GetComponent<PlayerSpawner> ().myHero.GetComponent<HeroObjectRelay> ().movePos = movePos;
						spawn.myHero.GetComponent<NavMeshAgent> ().enabled = true;
					}
				} else {
					mode = MovementMode.stop;
				}
				break;
			}
		}
	}

	void ShootProjectile (){
		GameObject projectile = myPool.Spawn (myHero.transform.position);
		projectile.GetComponent<Projectile> ().target = attackTarget;
		projectile.GetComponent<Projectile> ().damage = attackDamage;
	}

	void Attack (Health target){
		if (isServer) {
			attackTarget = target;
		} else {
			CmdAttack (target);
		}
	}

	[Command]
	void CmdAttack (Health target){
		attackTarget = target;
	}

	[Command]
	void CmdChangePos (Vector3 pos){
		movePos = pos;
		GetComponent<PlayerSpawner> ().myHero.GetComponent<HeroObjectRelay> ().movePos = movePos;
	}

	void ChangePos (Vector3 pos){
		if (isServer) {
			movePos = pos;
			GetComponent<PlayerSpawner> ().myHero.GetComponent<HeroObjectRelay> ().movePos = movePos;
		} else {
			CmdChangePos (pos);
		}
	}

	void Stop (){
		if (isServer) {
			movePos = GetComponent<PlayerSpawner> ().myHero.transform.position;
			GetComponent<PlayerSpawner> ().myHero.GetComponent<HeroObjectRelay> ().movePos = movePos;
		} else {
			CmdChangePos (GetComponent<PlayerSpawner> ().myHero.transform.position);
		}
	}
}
