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
						CmdAttack (hit.transform.root.gameObject);
					} else {
						CmdChangePos(hit.point);
					}
				}
			}
		}


		//----------------------------------------------------------------------------------SERVER SIDE CODE
		if (isServer) {

			if (myHero == null) {
				myHero = spawn.myHero;
				print ("Getting hero");
				return;
			}

			if (myPool == null) {
				myPool = myHero.GetComponent<HeroObjectRelay> ().myBasicAttackPool;
				return;
			}

			CalculateMovement ();
		}
	}

	void CalculateMovement (){
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

					}
					movePos = GetComponent<PlayerSpawner> ().myHero.transform.position;
				} else {
					GetComponent<PlayerSpawner> ().myHero.GetComponent<HeroObjectRelay> ().movePos = movePos;
				}
			} else {
				mode = MovementMode.stop;
			}
			break;
		}

		attackCounter -= Time.deltaTime;
		GetComponent<PlayerSpawner> ().myHero.GetComponent<HeroObjectRelay> ().movePos = movePos;
	}

	//----------------------------------------------------------------------------------SERVER SIDE CODE
	void ShootProjectile (){
		GameObject projectile = myPool.Spawn (myHero.transform.position);
		projectile.GetComponentInChildren<Projectile> (true).target = attackTarget;
		projectile.GetComponentInChildren<Projectile> (true).damage = attackDamage;

		RpcShootProjectile (projectile, attackTarget.gameObject);
	}

	[ClientRpc]
	void RpcShootProjectile (GameObject projectile, GameObject target){
		projectile.GetComponentInChildren<Projectile> (true).target = target.GetComponent<Health>();
		projectile.GetComponentInChildren<Projectile> (true).damage = attackDamage;
	}


	[Command]
	void CmdAttack (GameObject target){
		if (target != null) {
			if (target.GetComponent<Health>() != null) {
				if (target.GetComponent<Health> ().mySide != spawn.mySide) {
					
					attackTarget = target.GetComponent<Health> ();
					mode = MovementMode.attackmove;
					print (gameObject.name + " - attacking");
					return;
				}
			}
		} 
	
		attackTarget = null;
		mode = MovementMode.move;
		print (gameObject.name + " - moving");
	}

	[Command]
	void CmdChangePos (Vector3 pos){
		movePos = pos;
		mode = MovementMode.move;
		print (gameObject.name + " - moving to " + pos.ToString());
		attackTarget = null;
	}

	[Command]
	void CmdStop (){
		CmdChangePos (GetComponent<PlayerSpawner> ().myHero.transform.position);
	}


}
