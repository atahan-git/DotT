using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour {
	public Text version;
	public TextAsset versionText;


	void Start (){
		UpdateVersionText ();
	}

	void UpdateVersionText (){
		try{
			version.text = GetVersionNumber ();
		}catch{
			Invoke ("UpdateVersionText", 2f);
		}
	}

	string GetVersionNumber (){
		try {
			string content = versionText.text;

			if (content != null) {
				return content;
			} else {
				return " ";
			}
		} catch (System.Exception e) {
			Debug.Log ("Can't Get Version Number " + e.Message + e.StackTrace);
		}
		return " ";
	}
}