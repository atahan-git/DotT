using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSpawner : NetworkBehaviour {

	[SyncVar]
	public int playerid = -1;
	[SyncVar]
	public int heroType = -1;

	[HideInInspector]
	public GameObject myHero;

	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			gameObject.name = "Local Player Master";
			CmdSetUpPlayer ();
		}
	}

	[Command]
	void CmdSetUpPlayer () {
		RpcGetId (connectionToClient.connectionId);

		playerid = connectionToClient.connectionId;
		heroType = DataHandler.s.heroIds [playerid];
		SpawnHero ();
	}

	[ClientRpc]
	void RpcGetId (int theId) {
		playerid = theId;
		heroType = DataHandler.s.heroIds [playerid];
		print ("My Player Id = " + playerid);
		if (isLocalPlayer) {
			print ("Got Local Player Id = " + playerid);
		}
	}

	void SpawnHero (){
		myHero = (GameObject)Instantiate (STORAGE_HeroPrefabs.s.heroes [heroType], Vector3.up * 5, Quaternion.identity);
		myHero.GetComponent<HeroObjectRelay> ().id = playerid;
		NetworkServer.Spawn (myHero);
	}
}
