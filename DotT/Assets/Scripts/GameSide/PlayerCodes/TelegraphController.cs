using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//everything but the rpcs are server sided
public class TelegraphController : MonoBehaviour {

	public bool isVisibleToEnemies = true;

	//Remember that these are not prefabs but actual gameobjects
	public GameObject[] allyGfx = new GameObject[1];
	public GameObject[] enemyGfx = new GameObject[1];
	int currentState = 0;

	public TriggerChildObject[] myTriggers = new TriggerChildObject[1];

	//[HideInInspector]
	public Health myHealth;

	void Start (){
		myHealth = GetComponentInParent<Health> ();
		transform.parent = null;
		HideTelegraph ();
	}

	public void ShowTelegraph (){
		print (PlayerSpawner.LocalPlayerSpawner.name + " - " + PlayerSpawner.LocalPlayerSpawner.mySide.ToString() + " - " + myHealth.mySide);
		if (myHealth.mySide != PlayerSpawner.LocalPlayerSpawner.mySide) {
			//we are enemy to the local player
			enemyGfx [currentState].SetActive (true);
		} else {
			//we are ally to the local player
			allyGfx [currentState].SetActive (true);
		}
	}


	public void HideTelegraph (){
		foreach (GameObject go in allyGfx)
			if (go != null)
				go.SetActive (false);

		foreach (GameObject go in enemyGfx)
			if (go != null)
				go.SetActive (false);
	}


	public void SetPosition (Vector3 pos){
		transform.position = pos;
	}



	//----------------------------------------------------------------------------------SERVER SIDE CODE
	public Health GetTarget (){


		return null;
	}
	public List<Health> allTargets = new List<Health>();
	public MultipleHealths GetAreaTargets (){
		allTargets = new List<Health> ();

		foreach (GameObject myObj in myTriggers[0].collidingObjects) {
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
