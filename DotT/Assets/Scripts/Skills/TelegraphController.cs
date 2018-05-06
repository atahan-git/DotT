using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//everything but the rpcs are server sided
public class TelegraphController : MonoBehaviour {

	public bool isVisibleToEnemies = true;

	//Remember that these are not prefabs but actual gameobjects
	public GameObject[] allyGfx = new GameObject[1];
	public GameObject[] enemy1Gfx = new GameObject[1];
	public GameObject[] enemy2Gfx = new GameObject[1];
	public GameObject[] neutralGfx = new GameObject[1];
	int currentState = 0;

	public TriggerChildObject[] myTriggers = new TriggerChildObject[1];

	//[HideInInspector]
	public Health myHealth;

	void Start (){
		myHealth = GetComponentInParent<Health> ();
		transform.parent = null;
		HideTelegraph ();
	}

	public void ShowTelegraph (Transform target){
		transform.parent = target;
		transform.localPosition = Vector3.zero;

		ShowTelegraph ();
	}

	public void ShowTelegraph (){
		print ("Showing telegraph: " + PlayerSpawner.LocalPlayerSpawner.name + " - " + PlayerSpawner.LocalPlayerSpawner.mySide.ToString() + " - " + myHealth.mySide);
		if (myHealth.mySide != PlayerSpawner.LocalPlayerSpawner.mySide) {
			//we are enemy to the local player
			if (myHealth.mySide == Health.Side.red || (PlayerSpawner.LocalPlayerSpawner.mySide == Health.Side.red && myHealth.mySide == Health.Side.blue)) {
				foreach (GameObject go in enemy1Gfx)
					if (go != null)
						go.SetActive (true);
			} else {
				foreach (GameObject go in enemy2Gfx)
					if (go != null)
						go.SetActive (true);
			}
		} else {
			//we are ally to the local player
			foreach (GameObject go in allyGfx)
				if (go != null)
					go.SetActive (true);
		}
		foreach (GameObject go in neutralGfx)
			if (go != null)
				go.SetActive (true);
	}


	public void HideTelegraph (){
		transform.parent = null;
		foreach (GameObject go in allyGfx)
			if (go != null)
				go.SetActive (false);

		foreach (GameObject go in enemy1Gfx)
			if (go != null)
				go.SetActive (false);

		foreach (GameObject go in enemy2Gfx)
			if (go != null)
				go.SetActive (false);
		foreach (GameObject go in neutralGfx)
			if (go != null)
				go.SetActive (false);
	}


	public void SetPosition (Vector3 pos){
		transform.position = pos;
	}



	//----------------------------------------------------------------------------------SERVER SIDE CODE
	public List<Health> allTargets = new List<Health>();
	public MultipleHealths GetAreaTargets (){
		allTargets = new List<Health> ();

		foreach (TriggerChildObject chl in myTriggers) {
			if (chl != null) {
				foreach (GameObject myObj in chl.collidingObjects) {
					Health objHealth = myObj.GetComponentInParent<Health> ();
					if (objHealth == null)
						objHealth = myObj.GetComponent<Health> ();
					if (objHealth == null) {
						objHealth = myObj.GetComponentInChildren<Health> ();
					}

					if (objHealth != null) {
						if (objHealth.mySide != myHealth.mySide)
							allTargets.Add (objHealth);
					}
				}
			}
		}

		return new MultipleHealths(allTargets.ToArray ());
	}

	public MultipleHealths GetAreaTargets (int i){
		allTargets = new List<Health> ();

		foreach (GameObject myObj in myTriggers[i].collidingObjects) {
			Health objHealth = myObj.GetComponentInParent<Health> ();
			if (objHealth == null)
				objHealth = myObj.GetComponent<Health> ();
			if (objHealth == null) {
				objHealth = myObj.GetComponentInChildren<Health> ();
			}

			if (objHealth != null) {
				if (objHealth.mySide != myHealth.mySide)
					allTargets.Add (objHealth);
			}
		}

		return new MultipleHealths(allTargets.ToArray ());
	}
}
