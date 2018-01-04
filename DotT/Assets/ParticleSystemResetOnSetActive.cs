using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemResetOnSetActive : MonoBehaviour {

	ParticleSystem mySys;

	void Awake (){
		mySys = GetComponent<ParticleSystem> ();
		if (mySys == null) {
			Debug.LogError ("Particle System not found on " + gameObject.name + "! - destroying this script");
			Destroy (this);
		}
	}

	void OnEnable (){
		mySys.Play ();
	}

	void OnDisable (){
		mySys.Stop ();
	}

}
