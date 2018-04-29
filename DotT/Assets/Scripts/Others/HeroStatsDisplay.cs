using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStatsDisplay : MonoBehaviour {

	public Image myImg;

	public Slider healthBar;
	public Slider manaBar;

	public Text hpTex;
	public Text manaTex;

	public float myHp_percent;
	public float myMana_percent;

	void Start (){
		StartCoroutine (SetUp());
	}
	IEnumerator SetUp (){
		yield return null;
		while (PlayerSpawner.LocalPlayerSpawner == null)
			yield return null;
		while (PlayerSpawner.LocalPlayerSpawner.heroType == -1)
			yield return null;

		myImg.sprite = STORAGE_HeroPrefabs.s.heroIcons [PlayerSpawner.LocalPlayerSpawner.heroType];
	}
	
	// Update is called once per frame
	void Update () {
		if (PlayerSpawner.LocalPlayerSpawner == null)
			return;
		
		if (PlayerSpawner.LocalPlayerSpawner.myHealth == null)
			return;

		myHp_percent = PlayerSpawner.LocalPlayerSpawner.myHealth.currentHealth / PlayerSpawner.LocalPlayerSpawner.myHealth.maximumHealth;
		myMana_percent = PlayerSpawner.LocalPlayerSpawner.GetComponent<SkillController> ().mana / PlayerSpawner.LocalPlayerSpawner.GetComponent<SkillController> ().maxMana;

		healthBar.value = myHp_percent;
		manaBar.value = myMana_percent;

		hpTex.text = ((int)PlayerSpawner.LocalPlayerSpawner.myHealth.currentHealth).ToString ();
		manaTex.text = ((int)PlayerSpawner.LocalPlayerSpawner.GetComponent<SkillController> ().mana).ToString ();
	}
}
