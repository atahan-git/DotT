using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class HeroController : NetworkBehaviour {

	//these are server side only
	public Vector3 movePos = Vector3.zero;
	public Health attackTarget;

	public float attackSpeed = 1.2f;
	public float attackCounter = 0f;
	public float attackDamage = 50f;
	public float attackrange = 15f;

	public enum MovementMode{stop, move, attackmove}

	public MovementMode mode;

	PlayerSpawner spawn;
	GameObject myHero;
	ObjectPool myPool;

	void Start (){
		spawn = GetComponent<PlayerSpawner> ();
	}
	
	// Update is called once per frame
	void Update () {
		//----------------------------------------------------------------------------------PLAYER SIDE CODE
		if (isLocalPlayer) {
			if (Input.GetMouseButtonDown (1)) {
				RaycastHit hit; 
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
				if (Physics.Raycast (ray, out hit, 100.0f)) { 
					if (hit.transform.root.gameObject.tag == "Hero" && hit.transform.root.gameObject.GetComponent<Health>()!= null) {
						Attack (hit.transform.root.gameObject);
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


		//----------------------------------------------------------------------------------SERVER SIDE CODE
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
					if (Vector3.Distance (spawn.myHero.transform.position, attackTarget.transform.position) < attackrange) {
						if (attackCounter <= 0) {
							if(attackTarget != null)
								ShootProjectile ();
							attackCounter = 1f / attackSpeed;
						} else {
							attackCounter -= Time.deltaTime;
						}
						spawn.myHero.GetComponent<NavMeshAgent> ().enabled = false;
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

	//----------------------------------------------------------------------------------mostly SERVER SIDE CODE
	void ShootProjectile (){
		GameObject projectile = myPool.Spawn (myHero.transform.position);
		projectile.GetComponent<Projectile> ().target = attackTarget;
		projectile.GetComponent<Projectile> ().damage = attackDamage;
	}

	void Attack (GameObject target){
		if (isServer) {
			if(target != null)
			attackTarget = target.GetComponent<Health>();
		} else {
			CmdAttack (target);
		}
	}

	[Command]
	void CmdAttack (GameObject target){
		if(target != null)
		attackTarget = target.GetComponent<Health>();
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
