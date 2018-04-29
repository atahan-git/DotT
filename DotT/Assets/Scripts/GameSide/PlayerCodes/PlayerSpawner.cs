using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSpawner : NetworkBehaviour, IRespawnManager {

	public static PlayerSpawner LocalPlayerSpawner;	//used for knowing which color of telegraphs to displays and such
	public static PlayerSpawner[] AllSpawners = new PlayerSpawner[9];

	[SyncVar]
	public int playerid = -1;
	[SyncVar]
	public int heroType = -1;
	[SyncVar]
	public Health.Side mySide = Health.Side.neutral;


	public GameObject myHero;
	public Health myHealth;

	public Transform[] Spawns = new Transform[9];

	// Use this for initialization
	void Start () {
		GameObject spawnParent = GameObject.FindGameObjectWithTag ("SpawnPoint");
		for (int i = 0; i < 9; i++) {
			Spawns [i] = spawnParent.transform.GetChild (i);
		}

		if (isLocalPlayer) {
			gameObject.name = "Local Player Master";

			print ("### Setting up the player " + gameObject.name);
			Invoke ("DelayedCmdSetUpPlayer", 0.5f);
			LocalPlayerSpawner = this;
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
		AllSpawners [playerid] = this;
		heroType = DataHandler.s.heroIds [playerid];
		SpawnHero ();
	}

	[ClientRpc]
	void RpcGetId (int theId) {
		print ("### Received Settings " + gameObject.name);
		playerid = theId;
		AllSpawners [playerid] = this;
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
		GetComponent<HeroController> ().movePos = myHero.transform.position;
		myHero.name = myHero.name + " - " + playerid.ToString();
		NetworkServer.Spawn (myHero);
		RpcSetLocalHero (myHero, myHero.transform.position);

		myHealth = myHero.GetComponent<Health> ();
		myHealth.myRespawnManager = this;
		if (DataHandler.s.playerSlots [playerid] < 3)
			myHealth.mySide = Health.Side.blue;
		else if (DataHandler.s.playerSlots [playerid] < 6)
			myHealth.mySide = Health.Side.red;
		else
			myHealth.mySide = Health.Side.green;

		mySide = myHealth.mySide;
	}

	[ClientRpc]
	void RpcSetLocalHero (GameObject _myHero, Vector3 pos){
		GetComponent<HeroController> ().movePos = pos;

		myHero = _myHero;

		myHealth = _myHero.GetComponent<Health> ();
		if (DataHandler.s.playerSlots [playerid] < 3)
			myHealth.mySide = Health.Side.blue;
		else if (DataHandler.s.playerSlots [playerid] < 6)
			myHealth.mySide = Health.Side.red;
		else
			myHealth.mySide = Health.Side.green;

		mySide = myHealth.mySide;


		if (isLocalPlayer) {
			myHero.GetComponent<Health> ().isLocalPlayerHealth = true;
		}

		print ("Local Player Setup Complete! ->" + playerid.ToString());
	}

	public ObjectPool deadEfectPool;
	public float deadTime = 10f;

	public void Die (Health hp){
		myHero.SetActive (false);

		RpcDie ();

		deadEfectPool.Spawn (myHero.transform.position);

		Invoke ("Respawn",deadTime);

		if (isLocalPlayer) {
			DeadUI (true);
		}
	}
	[ClientRpc]
	void RpcDie (){
		myHero.SetActive (false);

		if (isLocalPlayer) {
			DeadUI (true);
		}
	}

	void DeadUI (bool isDead){
		DeadUIEffectandTimer.s.isDead = isDead;
		DeadUIEffectandTimer.s.timer = deadTime;
	}

	void Respawn(){
		myHealth.currentHealth = myHealth.maximumHealth;
		myHealth.isDead = false;

		myHero.transform.position = Spawns [DataHandler.s.playerSlots [playerid]].position;
		GetComponent<HeroController> ().movePos = myHero.transform.position;

		myHero.SetActive (true);
		RpcRespawn (myHero.transform.position);

		if (isLocalPlayer) {
			DeadUI (false);
		}
	}
	[ClientRpc]
	void RpcRespawn(Vector3 pos){
		myHero.transform.position = pos;
		myHero.SetActive (true);

		if (isLocalPlayer) {
			DeadUI (false);
		}
	}
}

public interface IRespawnManager {
	void Die(Health hp);
}
