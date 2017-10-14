using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ObjectPool : NetworkBehaviour {

	public bool autoExpand = true;
	public GameObject myObject;
	public int poolSize = 50;

	void Start (){
		if (myObject.GetComponent<PooledObject> () == null)
			myObject.AddComponent<PooledObject> ();

		myObject.GetComponent<PooledObject> ().myPool = this;

		if(isServer)
			SetUp (poolSize, myObject);
	}

	List<GameObject> objs = new List<GameObject>();
	Queue<int> activeIds = new Queue<int>();


	public void SetUp (int poolsize, GameObject item){
		for (int i = 0; i < poolsize; i++) {
			GameObject inst = (GameObject)Instantiate (item);
			myObject.GetComponent<PooledObject> ().myId = i;
			NetworkServer.Spawn (inst);
			inst.SetActive (false);
			objs.Add (inst);
		}
	}

		


	[Command]
	public void CmdSpawn (Vector3 pos, Quaternion rot){
		for (int i = 0; i < objs.Count; i++) {
			if (!objs [i].activeInHierarchy) {
				objs [i].transform.position = pos;
				objs [i].transform.rotation = rot;
				objs [i].SetActive (true);

				activeIds.Enqueue (i);
				return;
			}
		}

		//there is no free object left
		if (autoExpand) {
			GameObject inst = (GameObject)Instantiate (objs[0]);
			inst.transform.position = pos;
			inst.transform.rotation = rot;
		
			NetworkServer.Spawn (inst);
			objs.Add (inst);
			activeIds.Enqueue (poolSize);
			poolSize++;
			return;
		} else {
			int toReuse = activeIds.Dequeue ();
			activeIds.Enqueue (toReuse);

			objs [toReuse].transform.position = pos;
			objs [toReuse].transform.rotation = rot;
			objs [toReuse].SetActive (true);
			objs [toReuse].BroadcastMessage ("OnEnabled");
			return;
		}
	}
		
		
	public void CmdSpawn (Vector3 pos){
		CmdSpawn (pos, Quaternion.identity);
	}
		

	public void CmdSpawn (float x, float y, float z){
		CmdSpawn (new Vector3 (x, y, z));
	}


	[Command]
	public void CmdDestroy (int id){
		if(objs[id] != null){
			objs [id].SetActive (false);
		}
	}
}