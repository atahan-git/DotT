using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobyPlayerPanel : MonoBehaviour {


	[HideInInspector]
	public LobyPlayerController myPlayer;

	public int playerid = -1;
	public int heroType = 0;
	public bool playerState = false;
	public bool isLocalPlayer = false;


	public Text myName;
	public Text myState;
	public Text myHero;
	public GameObject myButton;

	public GameObject b1;
	public GameObject b2;

	// Use this for initialization
	void Start () {
	}

	public void UpdateValues () {
		myName.text = "Player " + playerid;
		if (playerState) {
			myState.text = "Ready";
			myState.color = Color.green;
		} else {
			myState.text = "Not Ready";
			myState.color = Color.red;
		}

		myHero.text = heroType.ToString();

		myButton.SetActive (isLocalPlayer);
		b1.SetActive (isLocalPlayer);
		b2.SetActive (isLocalPlayer);
	}

	public void ChangeHero(int amount){
		myPlayer.ChangeHero (amount);
	}

	public void ChangeState () {
		myPlayer.ChangeReadyState ();
	}
}
