using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSpawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Instantiate (STORAGE_HeroPrefabs.s.heroes [DataHandler.s.heroIds [DataHandler.s.localid]], Vector3.up *5, Quaternion.identity);
	}
}
