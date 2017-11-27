using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PooledObject : NetworkBehaviour {

	[SyncVar]
	public int myId = -1;
	public ObjectPool myPool;

	public bool isActive = false;

	void Start (){
		transform.GetChild(0).gameObject.SetActive (false);
	}

	//These two should only be called from server side
	public void EnableObject (){
		if (isServer) {
			RpcSyncEnable (transform.position, transform.rotation);
			transform.GetChild(0).gameObject.SetActive (true);
			isActive = true;
			ResetValues ();
		}
	}



	public void DisableObject (){
		if (isServer) {
			RpcSyncDisable ();
			transform.GetChild(0).gameObject.SetActive (false);
			isActive = false;
		}
	}
		

	[ClientRpc]
	void RpcSyncEnable (Vector3 pos, Quaternion rot){
		transform.GetChild(0).gameObject.SetActive (true);
		isActive = true;
		transform.position = pos;
		transform.rotation = rot;

		ResetValues ();
	}

	[ClientRpc]
	void RpcSyncDisable (){
		transform.GetChild(0).gameObject.SetActive (false);
		isActive = false;
	}


	public void Destroy (){
		myPool.Destroy (myId);
	}

	void ResetValues (){
		if (GetComponentInChildren<TrailRenderer> () != null) {
			GetComponentInChildren<TrailRenderer> ().Clear ();
		}
		if (GetComponent<Rigidbody> () != null) {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		}

		transform.GetChild (0).transform.position = transform.position;
		transform.GetChild (0).transform.rotation = transform.rotation;
	}
}
