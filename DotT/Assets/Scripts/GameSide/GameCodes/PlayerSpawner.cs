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

	public Transform[] Spawns = new Transform[10];

	// Use this for initialization
	void Start () {
		GameObject spawnParent = GameObject.FindGameObjectWithTag ("SpawnPoint");
		for (int i = 0; i < 10; i++) {
			Spawns [i] = spawnParent.transform.GetChild (i);
		}

		if (isLocalPlayer) {
			gameObject.name = "Local Player Master";

			print ("### Setting up the player " + gameObject.name);
			Invoke ("DelayedCmdSetUpPlayer", 0.5f);
		}
	}

	void DelayedCmdSetUpPlayer (){
		CmdSetUpPlayer ();
	}

	[Command]
	void CmdSetUpPlayer () {
		print ("### Sending setting to the client " + gameObject.name);
		RpcGetId (connectionToClient.connectionId);

		playerid = connectionToClient.connectionId;
		heroType = DataHandler.s.heroIds [playerid];
		SpawnHero ();
	}

	[ClientRpc]
	void RpcGetId (int theId) {
		print ("### Received Settings " + gameObject.name);
		playerid = theId;
		heroType = DataHandler.s.heroIds [playerid];
		print ("### My Player Id = " + playerid);
		if (isLocalPlayer) {
			print ("### Got Local Player Id = " + playerid);
			CameraController.s.SetPos (Spawns[DataHandler.s.playerSlots[playerid]].position);
			//print ("Position Send: " + Spawns[DataHandler.s.playerSlots[playerid]].position.ToString());
		}
	}

	//network execute only
	void SpawnHero (){
		myHero = (GameObject)Instantiate (STORAGE_HeroPrefabs.s.heroes [heroType], Spawns[DataHandler.s.playerSlots[playerid]].position, Spawns[DataHandler.s.playerSlots[playerid]].rotation);
		myHero.GetComponent<HeroObjectRelay> ().id = playerid;
		NetworkServer.Spawn (myHero);
		RpcSetHeroPos (myHero.transform.position);
	}

	[ClientRpc]
	void RpcSetHeroPos (Vector3 pos){
		GetComponent<HeroController> ().movePos = pos;

		print ("Local Player Setup Complete! ->" + playerid.ToString());
	}
}
