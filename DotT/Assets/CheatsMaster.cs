using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatsMaster : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.U))
			XPMaster.s.AddXp (Health.Side.blue, new Health.Side[]{Health.Side.blue, Health.Side.green, Health.Side.red}, Health.Type.hero);
	}
}
