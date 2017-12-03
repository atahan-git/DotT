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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Buggy
		//GetComponent<NavMeshAgent> ().SetDestination (movePos);
	}
}
