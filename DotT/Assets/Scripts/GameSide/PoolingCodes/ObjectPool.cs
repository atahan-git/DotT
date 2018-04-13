using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class ObjectPool : NetworkBehaviour {

	public bool autoExpand = true; //dont change this at runtime
	public GameObject myObject;
	public int poolSize = 50;

	void Start (){
		if (myObject.GetComponent<PooledObject> () == null)
			myObject.AddComponent<PooledObject> ();

		myObject.GetComponent<PooledObject> ().myPool = this;

		if(isServer)
			StartCoroutine(SetUp (poolSize, myObject));
	}

	List<GameObject> objs = new List<GameObject>();
	Queue<int> activeIds = new Queue<int>();


	public IEnumerator SetUp (int poolsize, GameObject item){
		for (int i = 0; i < poolsize; i++) {
			GameObject inst = (GameObject)Instantiate (item);
			inst.GetComponent<PooledObject> ().myId = i;
			inst.GetComponent<PooledObject> ().DisableObject ();
			objs.Add (inst);
			NetworkServer.Spawn (inst);
			yield return 0;
		}
	}


	GameObject _Spawn (Vector3 pos, Quaternion rot){
		for (int i = 0; i < objs.Count; i++) {
			if (!objs [i].GetComponent<PooledObject>().isActive) {

				objs [i].transform.position = pos;
				objs [i].transform.rotation = rot;
				objs [i].GetComponent<PooledObject> ().EnableObject ();

				if(!autoExpand)
					activeIds.Enqueue (i);
				
				return objs [i];
			}
		}
		print ("Not enough pooled objects detected");

		//there is no free object left
		if (autoExpand) {
			GameObject inst = (GameObject)Instantiate (objs[0]);
			inst.transform.position = pos;
			inst.transform.rotation = rot;

			NetworkServer.Spawn (inst);
			objs.Add (inst);
			inst.GetComponent<PooledObject> ().myId = poolSize;
			poolSize++;
			return objs [poolSize-1];
		} else {
			int toReuse = activeIds.Dequeue ();
			activeIds.Enqueue (toReuse);

			objs [toReuse].transform.position = pos;
			objs [toReuse].transform.rotation = rot;
			objs [toReuse].GetComponent<PooledObject> ().EnableObject ();
			objs [toReuse].BroadcastMessage ("OnEnabled");
			return objs [toReuse];
		}
	}





	public GameObject Spawn(Vector3 pos, Quaternion rot){
		if (isServer) {
			return _Spawn (pos, rot);
		} else {
			throw new System.Exception("Only server side spawning is allowed!");
		}
		return null;
	}
		
		
	public GameObject Spawn (Vector3 pos){
		return Spawn (pos, Quaternion.identity);
	}
		

	public GameObject Spawn (float x, float y, float z){
		return Spawn (new Vector3 (x, y, z));
	}


	void _DestroyPooledObject(int id){
		if (objs [id] != null) {
			objs [id].GetComponent<PooledObject> ().DisableObject ();
		} else {
			Debug.LogError ("Pooled object with wrong id detected");
		}
	}


	[Command]
	void CmdDestroyPooledObject (int id){
		_DestroyPooledObject (id);
	}

	public void DestroyPooledObject (int id){
		if (isServer)
			_DestroyPooledObject (id);
		else
			CmdDestroyPooledObject (id);
	}
}