using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STORAGE_HeroPrefabs : MonoBehaviour {

	public static STORAGE_HeroPrefabs s;

	public GameObject[] heroes = new GameObject[4];

	void Awake (){
		s = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
