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
	public SyncListInt playerSlots = new SyncListInt ();

	// Use this for initialization
	void Start () {
		if (playerSlots.Count == 0)
			SetUp ();
	}

	public void SetUp (){
		for (int i = 0; i < 10; i++) {
			heroIds.Add (0);
		}
		for (int i = 0; i < 10; i++) {
			playerSlots.Add (i);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
