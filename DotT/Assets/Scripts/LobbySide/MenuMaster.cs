using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMaster : MonoBehaviour {

	public static MenuMaster s;

	public Text textPlayer;

	public GameObject startGUI;
	public GameObject lobbyGUI;

	GameObject buttonUp;


	// Use this for initialization
	void Start () {
		s = this;

		//-------------------Warning!
		//----------------Cancer Ahead
		if (textPlayer == null) {
			textPlayer = GameObject.Find ("PlayerText").GetComponent<Text> ();
			startGUI = GameObject.Find ("Start GUI");
			lobbyGUI = GameObject.Find ("Lobby GUI");
			GameObject.Find ("ButtonUp").GetComponent<Button> ().onClick.AddListener (IncreasePlayerCount);
			GameObject.Find ("ButtonDown").GetComponent<Button> ().onClick.AddListener (DecreasePlayerCount);
			GameObject.Find ("Host").GetComponent<Button> ().onClick.AddListener (HostaGame);
			GameObject.Find ("Join").GetComponent<Button> ().onClick.AddListener (JoinaGame);
			GameObject.Find ("Back").GetComponent<Button> ().onClick.AddListener (ExitaGame);
			lobbyGUI.SetActive (false);
			startGUI.SetActive (true);
		}

		DecreasePlayerCount ();
	}

	public void IncreasePlayerCount () {
		LobyController.s.IncreasePlayerCount ();
	}

	public void DecreasePlayerCount () {
		LobyController.s.DecreasePlayerCount ();
	}
		

	public void HostaGame () {
		LobyController.s.HostaGame ();
	}

	public void JoinaGame () {
		LobyController.s.JoinaGame ();
	}

	public void ExitaGame () {
		LobyController.s.ExitaGame ();
	}
}
