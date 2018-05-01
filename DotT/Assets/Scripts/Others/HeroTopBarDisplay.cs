using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroTopBarDisplay : MonoBehaviour {


	public int myPlayerId = -1;
	public int mySlotId = -1;
	public int myHeroId = -1;


	public Image myHeroImg;
	public Slider myHeatlhBar;

	public float myHp_percent;

	public void SetUp (){
		mySlotId = DataHandler.s.playerSlots [myPlayerId];
		myHeroId = DataHandler.s.heroIds [mySlotId];
		myHeroImg.sprite = STORAGE_HeroPrefabs.s.heroIcons [myHeroId];
	}


	void Update(){
		if (myPlayerId == -1)
			return;

		if (PlayerSpawner.AllSpawners [mySlotId] != null) {
			myHp_percent = PlayerSpawner.AllSpawners [mySlotId].myHealth.currentHealth / PlayerSpawner.AllSpawners [mySlotId].myHealth.maximumHealth;
			myHeatlhBar.value = myHp_percent;
		} else {
			myHeatlhBar.enabled = false;
		}
	}
}
