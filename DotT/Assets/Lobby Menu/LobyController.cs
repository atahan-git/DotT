﻿using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobyController : NetworkBehaviour {

	public static LobyController s;

	public int playerCount = 2;
	[HideInInspector]
	public Text textPlayer;

	NetworkLobbyManager manager;

	[HideInInspector]
	public GameObject startGUI;
	[HideInInspector]
	public GameObject lobbyGUI;

	GameObject buttonUp;

	void Awake (){
		if (s != null && s != this) {
			Destroy (this.gameObject);
		} else {
			s = this;
			DontDestroyOnLoad (this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		manager = GetComponent<NetworkLobbyManager> ();
		if (lobbyGUI == null) {
			SceneManager.LoadScene (0);
		}

	}

	NetworkLobbyPlayer[] oldSlots;
	GameObject[] myPanels;
	// Update is called once per frame

	public void IncreasePlayerCount () {
		ChangePlayerCount (1);

	}

	public void DecreasePlayerCount () {
		ChangePlayerCount (-1);
	}

	void ChangePlayerCount (int amount){

		playerCount += amount;
		playerCount = Mathf.Clamp (playerCount, 1, 4);
		MenuMaster.s.textPlayer.text = playerCount.ToString ();

		if(manager == null)
			manager = GetComponent<NetworkLobbyManager> ();

		manager.maxPlayers = playerCount;
		manager.minPlayers = playerCount;
	}

	public bool isHost = false;

	public void HostaGame () {

		MenuMaster.s.startGUI.SetActive (false);
		MenuMaster.s.lobbyGUI.SetActive (true);

		manager.StartHost ();
		isHost = true;
	}

	public void JoinaGame () {

		MenuMaster.s.startGUI.SetActive (false);
		MenuMaster.s.lobbyGUI.SetActive (true);

		manager.StartClient ();
		isHost = false;
	}

	public void ExitaGame () {
		if(DataHandler.s != null)
			Destroy (DataHandler.s.gameObject);

		MenuMaster.s.startGUI.SetActive (true);
		MenuMaster.s.lobbyGUI.SetActive (false);

		if (isHost) {
			manager.StopHost ();
		} else {
			manager.StopClient ();
		}

		textPlayer = null;
		//UnityEngine.SceneManagement.SceneManager.LoadScene (0);
	}


	//-----------------------------------------------------------------BOT STUFF

	int connectedPlayers = -1;
	public GameObject botLobyPanel;
	public List<GameObject> allBotLobyPanels = new List<GameObject>();

	void Update (){

		if (manager.isNetworkActive) {
			if (manager.numPlayers != connectedPlayers) {

				if (allBotLobyPanels.Count == 0 && manager.numPlayers != 10) {
					while (manager.numPlayers + allBotLobyPanels.Count < 10) {
						GameObject extraPanel = (GameObject)Instantiate (botLobyPanel, transform.position, transform.rotation);
						allBotLobyPanels.Add (extraPanel);
					}
				}

				if (manager.numPlayers > connectedPlayers) {
					
					while (manager.numPlayers + allBotLobyPanels.Count > 10) {

						int num = allBotLobyPanels.Count;
						GameObject toDestroy = allBotLobyPanels [num - 1];
						allBotLobyPanels.Remove (toDestroy);
						Destroy (toDestroy);

						allBotLobyPanels.TrimExcess ();
					}


				} else if (manager.numPlayers < connectedPlayers) {


					while (manager.numPlayers + allBotLobyPanels.Count < 10) {
						GameObject extraPanel = (GameObject)Instantiate (botLobyPanel);
						allBotLobyPanels.Add (extraPanel);
					}

				}


				connectedPlayers = manager.numPlayers;
			}
		}
	}
}
