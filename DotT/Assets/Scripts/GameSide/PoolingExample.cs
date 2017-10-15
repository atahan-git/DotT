using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(ObjectPool))]
public class PoolingExample :  NetworkBehaviour {

	public GameObject cubePrefab;
	ObjectPool myPool;


	Queue<GameObject> myObjs = new Queue<GameObject>();

	void Start (){
		myPool = GetComponent<ObjectPool> ();

		InvokeRepeating ("SpawnStuffPOOLED", 1f, 0.005f);		//~80fps
		//InvokeRepeating ("SpawnStuffNORMAL", 0f, 0.005f);		//~30fps
	}
		

	void SpawnStuffPOOLED (){
		Vector3 rndOffset = new Vector3 (Random.Range (-2, 2), Random.Range (-2, 2) + 5, Random.Range (-2, 2));
		myPool.Spawn (transform.position + rndOffset);
	}

	void SpawnStuffNORMAL (){
		Vector3 rndOffset = new Vector3 (Random.Range (-2, 2), Random.Range (-2, 2) + 5, Random.Range (-2, 2));
		rndOffset = transform.position + rndOffset;
		GameObject myObj = (GameObject)Instantiate (cubePrefab, rndOffset, Quaternion.identity);
		myObjs.Enqueue (myObj);
		NetworkServer.Spawn (myObj);

		if (myObjs.Count > 50) {
			myObj = myObjs.Dequeue ();
			NetworkServer.UnSpawn (myObj);
			Destroy (myObj);
		}
	}
}
