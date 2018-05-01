using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TriggerChildObject : MonoBehaviour {

	List<GameObject> _collidingObjects = new List<GameObject>();
	public List<GameObject> collidingObjects{
		get{
			CleanupList ();
			return _collidingObjects;
		}
	}


	void Update (){
		foreach (GameObject myObj in _collidingObjects) {
			if (myObj != null) {
				if (!myObj.activeSelf) {
					_collidingObjects.Remove (myObj);
					break;
				}
			}
		}
	}

	void CleanupList (){
		_collidingObjects.RemoveAll (GameObject => GameObject == null);
		_collidingObjects = _collidingObjects.Distinct ().ToList ();
	}

	void OnTriggerEnter (Collider myCol){
		_collidingObjects.Add (myCol.gameObject);
	}

	void OnTriggerExit (Collider myCol){
		_collidingObjects.Remove (myCol.gameObject);
	}
}
