using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour {

	public Text textName;
	public Text textValue;

	string _name;
	public string myName {
		get {
			return _name;
		}
		set {
			_name = value;
			UpdatePanelValues ();
		}
	}

	string _value;
	public string myValue {
		get {
			return _value;
		}
		set {
			_value = value;
			UpdatePanelValues ();
		}
	}

	// Use this for initialization
	void Start () {
		UpdatePanelValues ();
	}

	void UpdatePanelValues (){
		textName.text = _name;
		textValue.text = _value;
	}
}
