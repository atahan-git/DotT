using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastText : MonoBehaviour {

	Text myText;
	// Use this for initialization
	void Start () {
		myText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit; 
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
		int layerMask = (1 << 8) | (1 << 9);
		if (Physics.Raycast (ray, out hit, 100.0f, layerMask)) { 
			myText.text = hit.transform.root.gameObject.name;
		}
	}
}
