using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PoolingController : MonoBehaviour {

	public static PoolingController s;

	// Use this for initialization
	void Awake () {
		s = this;
	}
}


public class Pool : NetworkBehaviour {

	List<GameObject> objs = new List<GameObject>();

	public Pool (int poolsize, GameObject item){
		if (isServer) {
			for (int i = 0; i < poolsize; i++) {
				GameObject inst = (GameObject)Instantiate (item);
				inst.SetActive (false);
				NetworkServer.Spawn (inst);
				objs.Add (inst);
			}
		}
	}
		


	//these will make an object from the pool active and return its id
	[Command]
	public int Spawn (Vector3 pos, Quaternion rot){
		for (int i = 0; i < objs.Count; i++) {
			if (!objs [i].activeInHierarchy) {
				objs [i].SetActive (true);
				objs [i].transform.position = pos;
				objs [i].transform.rotation = rot;
				return i;
			}
		}
	}

	[Command]
	public int Spawn (Vector3 pos){
		Spawn (pos, Quaternion.identity);
	}

	[Command]
	public int Spawn (float x, float y, float z){
		Spawn (new Vector3 (x, y, z));
	}




	[Command]
	public void Destroy (GameObject item){
		if(objs.Contains(item)){
			int i = objs.FindIndex(item);
			objs [i].SetActive (false);
		}
	}

	//use the id you got from spawn functions
	[Command]
	public void Destroy (int itemId){
		if (objs [itemId] != null) {
			objs [itemId].SetActive (false);
		}
	}
}