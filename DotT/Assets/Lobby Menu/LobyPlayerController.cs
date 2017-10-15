using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobyPlayerController : NetworkBehaviour {

	public GameObject playerPanel;
	public GameObject dataHandler;
	LobyPlayerPanel panelScript;
	NetworkLobbyPlayer manager;

	[SyncVar]
	public bool isReady;

	[SyncVar]
	public int id = -1;
	[SyncVar]
	public int heroType = 0;

	// Use this for initialization
	void Start () {
		manager = GetComponent<NetworkLobbyPlayer> ();

		GameObject panelParrent = GameObject.Find ("PanelParent");
		playerPanel = (GameObject)Instantiate (playerPanel, panelParrent.transform);
		playerPanel.transform.localScale = new Vector3 (0.8f, 0.8f, 0.8f);

		panelScript= playerPanel.GetComponent<LobyPlayerPanel> ();

		panelScript.playerid = manager.slot;
		panelScript.playerState = manager.readyToBegin;
		panelScript.myPlayer = this;

		if (isLocalPlayer)
			panelScript.isLocalPlayer = true;
		else 
			panelScript.isLocalPlayer = false;

		CmdSetUpPlayer ();

		if (isServer && DataHandler.s == null) {
			GameObject _dataHandler = (GameObject)Instantiate (dataHandler, Vector3.zero, Quaternion.identity);
			NetworkServer.Spawn (_dataHandler);
		}
	}

	[Command]
	void CmdSetUpPlayer () {
		RpcGetId (connectionToClient.connectionId);

	}

	[ClientRpc]
	void RpcGetId (int theId) {

		id = theId;
		print ("My Player Id = " + id);
		if (isLocalPlayer) {
			print ("Got Local Player Id = " + id);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene ().buildIndex == 0) {
			panelScript.playerid = manager.slot;
			panelScript.playerState = isReady;
			panelScript.UpdateValues ();
			panelScript.heroType = heroType +1;
			manager.readyToBegin = isReady;
		}
	}

	public void ChangeHero(int amount){
		heroType += amount;
		heroType = Mathf.Clamp (heroType, 0, 3);
		DataHandler.s.heroIds [id] = heroType;
		CmdChangeHerotype (id, heroType);
	}

	[Command]
	public void CmdChangeHerotype (int id, int type){
		heroType = type;
		DataHandler.s.heroIds [id] = type;
		Update ();
		panelScript.UpdateValues ();
		RpcChangeHerotype (id, type);
	}

	[ClientRpc]
	public void RpcChangeHerotype (int id, int type){
		heroType = type;
		DataHandler.s.heroIds [id] = type;
		Update ();
		panelScript.UpdateValues ();
	}

	public void ChangeReadyState() {
		CmdChangeReadyState ();
	}

	[Command]
	public void CmdChangeReadyState () {
		if (isReady) {
			isReady = false;
		} else {
			isReady = true;
		}

		Update ();
		panelScript.UpdateValues ();
		GameObject.Find ("NetWorkManaGer").GetComponent<NetworkLobbyManager>().CheckReadyToBegin();
	}
}
