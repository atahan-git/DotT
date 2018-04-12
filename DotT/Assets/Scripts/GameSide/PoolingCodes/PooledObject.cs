using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PooledObject : NetworkBehaviour {

	[SyncVar]
	public int myId = -1;
	public ObjectPool myPool;

	public bool isActive = false;

	public float lifeTime = -1f; //if a value bigger than zero will auto disable after that time

	void Start (){
		transform.GetChild(0).gameObject.SetActive (false);
	}

	//These two should only be called from server side
	/// <summary>
	/// DONT CALLS THIS. this is only for internal ObjectPool use. Use ObjectPool.Spawn() instead
	/// </summary>
	public void EnableObject (){
		if (isServer) {
			RpcSyncEnable (transform.position, transform.rotation);
			transform.GetChild(0).gameObject.SetActive (true);
			isActive = true;
			ResetValues ();
			if (lifeTime > 0f)
				Invoke ("DisableObject", lifeTime);
		}
	}


	//only server side
	/// <summary>
	/// DONT CALLS THIS. this is only for internal ObjectPool use. Use PooledObject.DestroyPooledObject() instead
	/// </summary>
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


	public void DestroyPooledObject (){
		myPool.DestroyPooledObject (myId);
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
