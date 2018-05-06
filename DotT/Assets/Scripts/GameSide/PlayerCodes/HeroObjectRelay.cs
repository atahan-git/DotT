using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class HeroObjectRelay : NetworkBehaviour {

	[SyncVar]
	public Vector3 movePos = Vector3.zero;

	[SyncVar]
	public int id = -1;


	public ObjectPool myBasicAttackPool;

	Animator myAnim;
	NavMeshAgent myAgent;
	// Use this for initialization
	void Start () {
		myAnim = GetComponentInChildren<Animator> ();
		myAgent = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(myAgent.isActiveAndEnabled)
		myAgent.SetDestination (movePos);

		if (myAnim) {
			//print (myAgent.velocity.magnitude);
			if (myAgent.velocity.magnitude > 0.1f) {
				myAnim.SetBool ("isWalking", true);
			} else {
				myAnim.SetBool ("isWalking", false);
			}

			myAnim.SetFloat ("walkSpeed", myAgent.velocity.magnitude/3.5f);
		}
	}
}
