using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//skills are executed from the server, targets supplied by client
public class SkillController : NetworkBehaviour {

	public delegate void SkillDelegate (ExecutionData data);

	public SkillDelegate QSkill;
	public SkillDelegate WSkill;
	public SkillDelegate ESkill;
	public SkillDelegate RSkill;

	[SyncVar]
	public float mana = 500f;
	public float maxMana = 500f;
	public float manaRegen = 50f;
	float[] curCooldown = new float[4];

	SkillSettings[] skillSettings = new SkillSettings[4];

	public bool isCooldownEnabled = true;

	//this exists only in the server side
	GameObject myHero;
	// Use this for initialization
	void Start () {
		if (isServer) {
			myHero = GetComponent<PlayerSpawner> ().myHero;
			StartCoroutine (HookUpSkills ());
		}
	}

	IEnumerator HookUpSkills (){
		while (true) {
			if (myHero != null && gameObject.activeSelf) {
				foreach (SkillMasterClass skill in GetComponent<PlayerSpawner>().myHero.GetComponentsInChildren<SkillMasterClass>()) {
					switch (skill.mySettings.skillButton) {
					case SkillSettings.Buttons.Q:
						QSkill = skill.ExecuteSkill;
						break;
					case SkillSettings.Buttons.W:
						WSkill = skill.ExecuteSkill;
						break;
					case SkillSettings.Buttons.E:
						ESkill = skill.ExecuteSkill;
						break;
					case SkillSettings.Buttons.R:
						RSkill = skill.ExecuteSkill;
						break;
					}

					int mySkillIndex = NumFromButton (skill.mySettings.skillButton);
					skillSettings [mySkillIndex] = skill.mySettings;

					SkillCooldownDisplay.disps [mySkillIndex].cooldown = skillSettings [mySkillIndex].cooldown;
				}

				RpcHookUpClient (myHero);

				break;
			} else {
				myHero = GetComponent<PlayerSpawner> ().myHero;
				yield return 0;
			}
		}
	}

	[ClientRpc]
	void RpcHookUpClient (GameObject _myHero){
		myHero = _myHero;
		foreach (SkillMasterClass skill in myHero.GetComponentsInChildren<SkillMasterClass>()) {
			switch (skill.mySettings.skillButton) {
			case SkillSettings.Buttons.Q:
				QSkill = skill.ExecuteSkill;
				break;
			case SkillSettings.Buttons.W:
				WSkill = skill.ExecuteSkill;
				break;
			case SkillSettings.Buttons.E:
				ESkill = skill.ExecuteSkill;
				break;
			case SkillSettings.Buttons.R:
				RSkill = skill.ExecuteSkill;
				break;
			}

			int mySkillIndex = NumFromButton (skill.mySettings.skillButton);
			skillSettings [mySkillIndex] = skill.mySettings;

			SkillCooldownDisplay.disps [mySkillIndex].cooldown = skillSettings [mySkillIndex].cooldown;
		}
	}


	
	// Update is called once per frame
	void Update () {
		if (PlayerSpawner.LocalPlayerSpawner.myHealth == null)
			return;
		

		if (isLocalPlayer && !PlayerSpawner.LocalPlayerSpawner.myHealth.isDead) {
			bool[] isKeyPressed = new bool[4];
			if (Input.GetKeyDown (KeyCode.Q))
				isKeyPressed [0] = true;
			if (Input.GetKeyDown (KeyCode.W))
				isKeyPressed [1] = true;
			if (Input.GetKeyDown (KeyCode.E))
				isKeyPressed [2] = true;
			if (Input.GetKeyDown (KeyCode.R))
				isKeyPressed [3] = true;


			if (isKeyPressed[0] || isKeyPressed[1] || isKeyPressed[2] || isKeyPressed[3]) {
				RaycastHit FloorHit; 
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				int floorMask = (1 << 8);
				if (Physics.Raycast (ray, out FloorHit, 100.0f, floorMask)) {
					float distance = Vector3.Distance (myHero.transform.position.Floorize (), FloorHit.point.Floorize ());

					for (int i = 0; i < 4; i++) {
						if (isKeyPressed[i] && curCooldown [i] <= 0 && distance <= skillSettings [i].range && mana > skillSettings [i].manaCost) {
							Health myTarget = null;
							if (skillSettings [i].skillType == SkillSettings.Types.Targeted) {
								
								RaycastHit heroHit; 
								int heroMask = (1 << 9);
								if (Physics.Raycast (ray, out heroHit, 100.0f, heroMask)) {
									myTarget = heroHit.transform.root.gameObject.GetComponent<Health> ();
									if (myTarget == null)
										continue;
								
									Health.Side mySide = PlayerSpawner.LocalPlayerSpawner.myHealth.mySide;
									Health.Side enemySide =	myTarget.mySide;

									switch (skillSettings [i].skillTarget) {
									case SkillSettings.Target.Both:
									//do nothing as we can target both the enemy and the friend
										break;
									case SkillSettings.Target.Enemy:
										/*if (mySide == enemySide)
											continue;*/
										break;
									case SkillSettings.Target.Ally:
										/*if (mySide != enemySide)
											continue;*/
										break;
									}
								} else {
									continue;
								}
							}
							GameObject target = null;
							if (myTarget != null)
								target = myTarget.gameObject;
							curCooldown [i] = skillSettings [i].cooldown;
							ExecuteSkill (ButtonFromNum (i), FloorHit.point, target);
						}
					}
				}
			}



			for (int i = 0; i < skillSettings.Length; i++) {
				if (skillSettings [i] != null) {
					if (!isCooldownEnabled) {
						curCooldown [i] = 0;
						mana = maxMana;
					}

					curCooldown [i] -= Time.deltaTime;
					curCooldown [i] = Mathf.Clamp (curCooldown [i], 0, skillSettings [i].cooldown);
					SkillCooldownDisplay.disps [i].curCooldown = curCooldown [i];
				}
			}

			mana += manaRegen * Time.deltaTime;
			mana = Mathf.Clamp (mana, 0, maxMana);
		}
	}


	void ExecuteSkill (SkillSettings.Buttons mySkillType, Vector3 hitPos, GameObject target){
		print (gameObject.name + " skill event triggered");

		CmdExecuteSkill (mySkillType, hitPos, target);
	}

	[Command]
	void CmdExecuteSkill (SkillSettings.Buttons mySkillType, Vector3 executePos, GameObject target){
		print (gameObject.name + " executed skill in Server "+ mySkillType.ToString());
		ExecutionData myData = new ExecutionData (true, myHero.transform.position, executePos, executePos.Floorize() - myHero.transform.position.Floorize(),target);
		switch (mySkillType) {
		case SkillSettings.Buttons.Q:
			if (QSkill != null)
				QSkill (myData);
			break;
		case SkillSettings.Buttons.W:
			if (WSkill != null)
				WSkill (myData);
			break;
		case SkillSettings.Buttons.E:
			if (ESkill != null)
				ESkill (myData);
			break;
		case SkillSettings.Buttons.R:
			if (RSkill != null)
				RSkill (myData);
			break;
		}
		RpcExecuteSkill (mySkillType, myData);

		myHero.transform.LookAt (myData.executePos);
	}

	[ClientRpc]
	void RpcExecuteSkill (SkillSettings.Buttons mySkillType, ExecutionData myData){
		print (gameObject.name + " executed skill in Client "+ mySkillType.ToString());
		/*myData.isServer = false;
		switch (mySkillType) {
		case SkillSettings.Buttons.Q:
			if (QSkill != null)
				QSkill (myData);
			break;
		case SkillSettings.Buttons.W:
			if (WSkill != null)
				WSkill (myData);
			break;
		case SkillSettings.Buttons.E:
			if (ESkill != null)
				ESkill (myData);
			break;
		case SkillSettings.Buttons.R:
			if (RSkill != null)
				RSkill (myData);
			break;
		}*/

		myHero.transform.LookAt (myData.executePos);
	}


	SkillSettings.Buttons ButtonFromNum (int i){
		switch (i) {
		case 0:
			return SkillSettings.Buttons.Q;
			break;
		case 1:
			return SkillSettings.Buttons.W;
			break;
		case 2:
			return SkillSettings.Buttons.E;
			break;
		case 3:
			return SkillSettings.Buttons.R;
			break;
		}

		return SkillSettings.Buttons.Q;
	}

	int NumFromButton (SkillSettings.Buttons i){
		switch (i) {
		case SkillSettings.Buttons.Q:
			return 0;
			break;
		case SkillSettings.Buttons.W:
			return 1;
			break;
		case SkillSettings.Buttons.E:
			return 2;
			break;
		case SkillSettings.Buttons.R:
			return 3;
			break;
		}

		return 0;
	}
}
