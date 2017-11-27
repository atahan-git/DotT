using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TheAmazingDebugBoard : MonoBehaviour {

	public Image mainPanel;

	[Header("Network Vals")]

	public DebugPanel heroIdPanel;
	public DebugPanel playerSlotPanel;
	public DebugPanel playerCountPanel;

	Color onColor = new Color (0.5f, 1f, 0.5f);
	Color offColor = new Color (1f, 0.5f, 0.5f);

	void Start (){
		
		heroIdPanel.myName = "Hero Ids=";
		playerSlotPanel.myName = "Player Slots=";
		playerCountPanel.myName = "Player Count=";

		mainPanel.color = onColor;
	}

	void Update (){
		if (DataHandler.s != null) {
			
			heroIdPanel.myValue = SyncListIntToString (DataHandler.s.heroIds);
			playerSlotPanel.myValue = SyncListIntToString (DataHandler.s.playerSlots);
			playerCountPanel.myValue = DataHandler.s.playerCount.ToString ();

		} else {
			
			mainPanel.color = offColor;
		}
	}

	string SyncListIntToString (SyncListInt myList) {
		string myString = "";

		for (int i = 0; i < myList.Count; i++) {
			
			myString += myList [i].ToString();

			if (i != 0 || i != myList.Count - 1)
				myString += ",";
		}

		return myString;
	}
}
