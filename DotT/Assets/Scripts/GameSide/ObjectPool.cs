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
			StartCoroutine(SetUp (poolSize, myObject));
	}

	List<GameObject> objs = new List<GameObject>();
	Queue<int> activeIds = new Queue<int>();


	public IEnumerator SetUp (int poolsize, GameObject item){
		for (int i = 0; i < poolsize; i++) {
			GameObject inst = (GameObject)Instantiate (item);
			myObject.GetComponent<PooledObject> ().myId = i;
			inst.SetActive (false);
			objs.Add (inst);
			NetworkServer.Spawn (inst);
			yield return 0;
		}
	}


	GameObject _Spawn (Vector3 pos, Quaternion rot){
		for (int i = 0; i < objs.Count; i++) {
			if (!objs [i].activeInHierarchy) {
				objs [i].transform.position = pos;
				objs [i].transform.rotation = rot;
				objs [i].SetActive (true);

				activeIds.Enqueue (i);
				return objs [i];
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
			return objs [poolSize-1];
		} else {
			int toReuse = activeIds.Dequeue ();
			activeIds.Enqueue (toReuse);

			objs [toReuse].transform.position = pos;
			objs [toReuse].transform.rotation = rot;
			objs [toReuse].SetActive (true);
			objs [toReuse].BroadcastMessage ("OnEnabled");
			return objs [toReuse];
		}
	}





	public GameObject Spawn(Vector3 pos, Quaternion rot){
		if (isServer)
			return _Spawn (pos, rot);
		else
			Debug.LogError ("peasant players are tying to spawn something! - this is not allowed");

		return null;
	}
		
		
	public GameObject Spawn (Vector3 pos){
		return Spawn (pos, Quaternion.identity);
	}
		

	public GameObject Spawn (float x, float y, float z){
		return Spawn (new Vector3 (x, y, z));
	}


	void _Destroy(int id){
		if(objs[id] != null){
			objs [id].SetActive (false);
		}
	}


	[Command]
	void CmdDestroy (int id){
		Destroy (id);
	}

	public void Destroy (int id){
		if (isServer)
			_Destroy (id);
		else
			CmdDestroy (id);
	}
}