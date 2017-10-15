using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PooledObject : NetworkBehaviour {

	[SyncVar]
	public int myId = -1;
	public ObjectPool myPool;


	void Start(){
		gameObject.SetActive (false);
	}

	void OnEnabled (){
		RpcSyncEnable (transform.position, transform.rotation);
		if (GetComponent<Rigidbody> () != null) {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		}
	}

	void OnDisabled (){
		RpcSyncDisable ();
	}
		

	[ClientRpc]
	void RpcSyncEnable (Vector3 pos, Quaternion rot){
		gameObject.SetActive (true);
		transform.position = pos;
		transform.rotation = rot;
		if (GetComponent<Rigidbody> () != null) {
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		}
	}

	[ClientRpc]
	void RpcSyncDisable (){
		gameObject.SetActive (false);
	}


	public void Destroy (){
		myPool.Destroy (myId);
	}
}
