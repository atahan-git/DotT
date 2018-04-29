using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPandHeroVisualsDisplay : MonoBehaviour {

	public static XPandHeroVisualsDisplay[] ss = new XPandHeroVisualsDisplay[3];

	public int visualId = -1;
	public Health.Side mySide = Health.Side.neutral;

	public int level;
	public float xp_percent;

	public Text txtLvl;
	public Slider sldXp;
	public Image sldBar;

	// Use this for initialization
	void Start () {
		ss [visualId] = this;

		StartCoroutine (SetUpSide());

	}

	IEnumerator SetUpSide (){
		yield return null;
		while (PlayerSpawner.LocalPlayerSpawner == null)
			yield return null;
		while (PlayerSpawner.LocalPlayerSpawner.myHealth == null)
			yield return null;

		switch (visualId) {
		case 0:
			mySide = PlayerSpawner.LocalPlayerSpawner.mySide;
			break;
		case 1:
			if (PlayerSpawner.LocalPlayerSpawner.mySide == Health.Side.blue)
				mySide = Health.Side.red;
			else
				mySide = Health.Side.blue;
			break;
		case 2:
			if (PlayerSpawner.LocalPlayerSpawner.mySide == Health.Side.blue)
				mySide = Health.Side.green;
			else if (PlayerSpawner.LocalPlayerSpawner.mySide == Health.Side.red)
				mySide = Health.Side.green;
			else
				mySide = Health.Side.red;
			break;
		default:
			Debug.LogError ("XPandHeroVisualsDisplay VisualId not set");
			break;
		}

		SetColor ();


		HeroTopBarDisplay[] myDisps = GetComponentsInChildren<HeroTopBarDisplay> ();

		for (int i = 0; i < 3; i++) {
			myDisps [i].myPlayerId = (XPMaster.SideToInt (mySide) * 3) + (2-i);
			myDisps [i].SetUp ();
			print (myDisps [i].gameObject.name);
		}
	}

	void Update(){
		if (XPMaster.s != null) {
			level = XPMaster.s.level [XPMaster.SideToInt (mySide)];
			txtLvl.text = (level + 1).ToString ();
			xp_percent = XPMaster.s.xp [XPMaster.SideToInt (mySide)];
			sldXp.value = xp_percent;
		}
	}

	void SetColor (){
		if (mySide != PlayerSpawner.LocalPlayerSpawner.mySide) {
			if (mySide == Health.Side.red || (PlayerSpawner.LocalPlayerSpawner.mySide == Health.Side.red && mySide == Health.Side.blue))
				sldBar.color = STORAGE_HealthPrefabs.s.enemyColor1;
			else
				sldBar.color = STORAGE_HealthPrefabs.s.enemyColor2;
		} else {
			sldBar.color = STORAGE_HealthPrefabs.s.allyColor;
		}
	}
}
