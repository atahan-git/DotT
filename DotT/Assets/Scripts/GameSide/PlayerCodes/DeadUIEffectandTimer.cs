using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadUIEffectandTimer : MonoBehaviour {

	public static DeadUIEffectandTimer s;

	public Image black;
	public Text txt;

	public float timer;

	public bool isDead = false;

	// Use this for initialization
	void Start () {
		s = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (isDead) {
			black.enabled = true;
			txt.enabled = true;
			txt.text = (((int)(timer*10f))/10f).ToString();

			timer -= Time.deltaTime;
		} else {
			black.enabled = false;
			txt.enabled = false;
		}
	}
}
