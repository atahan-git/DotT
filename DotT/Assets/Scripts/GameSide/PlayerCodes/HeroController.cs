using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class HeroController : NetworkBehaviour {

	//these are server side only
	public Vector3 movePos = Vector3.zero;
	public Health attackTarget;
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			if (Input.GetMouseButtonDown (1)) {
				RaycastHit hit; 
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
				if (Physics.Raycast (ray, out hit, 100.0f)) { 
					if (hit.transform.root.gameObject.tag == "Hero") {
						
					} else {
						ChangePos(hit.point);
					}
				}
			}
		}
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
}
