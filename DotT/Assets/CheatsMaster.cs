using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CheatsMaster : MonoBehaviour {

	public Button[] myButtons;
	public bool[] myStates;

	public GameObject dummy;
	
	// Update is called once per frame
	void Start () {
		for (int i = 0; i < myButtons.Length; i++) {
			if(!(i== 2 || i==3))
				myButtons [i].GetComponent<Image>().color = myStates [i] ? Color.green : Color.red;
		}
	}


	public void ToggleCheat (int i){
		switch (i) {
		case 0:
			myStates [i] = !myStates [i];
			myButtons [i].GetComponent<Image>().color = myStates [i] ? Color.green : Color.red;
			PlayerSpawner.LocalPlayerSpawner.GetComponent<SkillController> ().isCooldownEnabled = !myStates [i];
			break;
		case 1:
			myStates [i] = !myStates [i];
			myButtons [i].GetComponent<Image>().color = myStates [i] ? Color.green : Color.red;
			CameraController.s.isBounded = !myStates [i];
			break;
		case 2:
			XPMaster.s.AddXp (Health.Side.blue, new Health.Side[]{Health.Side.blue, Health.Side.green, Health.Side.red}, Health.Type.hero);
			break;
		case 3:
			/*GameObject myDummy = (GameObject)Instantiate (dummy);
			NetworkServer.Spawn (myDummy);*/
			print ("this is disabled because it causes unity crashing bug");
			break;
		}
	}
}
