using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChildObject : MonoBehaviour {

	List<GameObject> _collidingObjects = new List<GameObject>();
	public List<GameObject> collidingObjects{
		get{
			CleanupList ();
			return _collidingObjects;
		}
	}


	void CleanupList (){
		_collidingObjects.RemoveAll (GameObject => GameObject == null);
	}

	void OnTriggerEnter (Collider myCol){
		_collidingObjects.Add (myCol.gameObject);
	}

	void OnTriggerExit (Collider myCol){
		_collidingObjects.Remove (myCol.gameObject);
	}
}
