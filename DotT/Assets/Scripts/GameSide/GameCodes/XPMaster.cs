using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LevelUpEvent : UnityEvent<int,Health.Side>{}

public class XPMaster : MonoBehaviour {

	public static XPMaster s;

	/// <summary>
	/// Add various level up related functions to this thread
	/// </summary>
	public LevelUpEvent LevelUpFunctions;

	public int[] level = new int[3];
	public float[] xp = new float[3];
	public float[] reqXp = new float[3];

	float baseHero = 300;
	float heroAdd = 50;
	float heroDiffMax = 3.2f;
	float heroMultBase = 3.9f;

	float baseMinion = 474;
	float minionMult = 1.73f;
	float jungle = 200;

	float fort = 800;
	float keep = 1300;
	float towerFort = 400;
	float towerKeep = 650;

	void Awake () {
		s = this;
		for (int i = 0; i < 3; i++) {
			reqXp [i] = ReqLevelCalculator (level[i]);
		}
	}
	
	public void AddXp (Health.Side deadSide, Health.Side[] killerSides, Health.Type deadType){
		float xpReward = 0;
		switch (deadType) {
		case Health.Type.hero:
			xpReward = baseHero + level [SideToInt (deadSide)] * heroAdd;
			break;
		case Health.Type.jungle:
			xpReward = jungle;
			break;
		case Health.Type.minion:
			xpReward = baseMinion + Mathf.Pow (level [SideToInt (deadSide)], minionMult);
			break;
		case Health.Type.fort:
			xpReward = fort;
			break;
		case Health.Type.keep:
			xpReward = keep;
			break;
		case Health.Type.towerFort:
			xpReward = towerFort;
			break;
		case Health.Type.towerKeep:
			xpReward = towerKeep;
			break;
		}

		foreach(Health.Side rewardSide in killerSides){
			if (rewardSide != null) {
				if (rewardSide != Health.Side.neutral) {
					float mult = 1;
					if (deadType == Health.Type.hero) {
						mult = HeroLevelDifferenceToMultiplier (LevelDifCalculator (deadSide, rewardSide));
					}
					
					xp [SideToInt (rewardSide)] += xpReward * mult;
					print ("Xp Added: " + rewardSide.ToString () + " - " + xpReward * mult);

					if (xp [SideToInt (rewardSide)] > reqXp [SideToInt (rewardSide)]) {
						level [SideToInt (rewardSide)] += 1;
						reqXp [SideToInt (rewardSide)] = ReqLevelCalculator (level [SideToInt (rewardSide)]);
						LevelUpFunctions.Invoke (level [SideToInt (rewardSide)], rewardSide);
					}
				}
			}
		}
	}


	float levInc = 2.37f;
	float levDamp = 24f;
	float ReqLevelCalculator (int curLevel){
		return (2 + (Mathf.Pow (curLevel, levInc) / levDamp) + curLevel)*1000f;
	}



	float LevelDifCalculator (Health.Side dead, Health.Side reward){
		float xpLess = Mathf.Min (xp [SideToInt (dead)], xp [SideToInt (reward)]);
		float xpMore = Mathf.Max (xp [SideToInt (dead)], xp [SideToInt (reward)]);
		int lvlLess = Mathf.Min (level [SideToInt (dead)], level [SideToInt (reward)]);
		int lvlMore = Mathf.Max (level [SideToInt (dead)], level [SideToInt (reward)]);

		float isNegative = 1;
		if (xp [SideToInt (reward)] > xp [SideToInt (dead)]) {
			isNegative = -1;
		}

		float levelDif = lvlMore - lvlLess;
		levelDif += (ReqLevelCalculator (lvlMore+1) - xpMore) / (ReqLevelCalculator (lvlMore+1) - ReqLevelCalculator (lvlMore));

		return levelDif;
	}

	float HeroLevelDifferenceToMultiplier (float dif){
		float isNegative = 1;
		if (dif < 0) {
			isNegative = -1;
			dif = Mathf.Abs (dif);
		}
		Mathf.Clamp (dif, 0, heroDiffMax);

		return (100f + Mathf.Pow (heroMultBase, dif) * isNegative) / 100f;
	}

	int SideToInt (Health.Side mySide){
		switch (mySide) {
		case Health.Side.blue:
			return 0;
			break;
		case Health.Side.red:
			return 1;
			break;
		case Health.Side.green:
			return 2;
			break;
		case Health.Side.neutral:
			return 3;
			break;
		default: 
			return -1;
		}
	}
}
