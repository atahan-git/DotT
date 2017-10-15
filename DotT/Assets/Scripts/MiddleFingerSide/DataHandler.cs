using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataHandler : NetworkBehaviour {

	public static DataHandler s;

	void Awake(){
		if (s != null && s != this) {
			Destroy (this.gameObject);
		} else {
			s = this;
			DontDestroyOnLoad (this.gameObject);
		}
	}
		
	public SyncListInt heroIds = new SyncListInt ();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 10; i++) {
			heroIds.Add (0);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
