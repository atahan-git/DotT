using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobyBotPanel : MonoBehaviour {


	public int playerid = -1;
	public int playerSlot = -1;
	public int heroType = 0;
	public bool playerState = false;
	public bool isLocalPlayer = false;


	public Text myName;
	public Text myState;
	public Text myHero;
	public GameObject myButton;

	public GameObject b1;
	public GameObject b2;

	Image bg;
	public Color blueTeam = Color.blue;
	public Color redTeam = Color.red;

	// Use this for initialization
	void Start () {
		playerState = true;
		GameObject panelParrent = GameObject.Find ("PanelParent");
		transform.SetParent (panelParrent.transform);
		transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);

		bg = GetComponent<Image> ();
	}

	public void UpdateValues () {
		myName.text = "Bot " + playerid;
		if (playerState) {
			myState.text = "Ready";
			myState.color = Color.green;
		} else {
			myState.text = "Not Ready";
			myState.color = Color.red;
		}

		if (playerSlot < 5) {
			bg.color = blueTeam;
		} else {
			bg.color = redTeam;
		}

		heroType = DataHandler.s.heroIds [playerid];

		myHero.text = (heroType + 1).ToString();
	}

	void Update (){
		playerSlot = transform.GetSiblingIndex ();
		playerid = DataHandler.s.playerSlots.IndexOf (playerSlot);

		UpdateValues ();
	}

	public void ChangeHero(int amount){
		LobyPlayerController.localPlayer.ChangeBotHero (playerSlot, amount);
	}

	public void ChangePlayerPosition () {
		LobyPlayerController.localPlayer.ChangeSlot (playerSlot);
	}
}
