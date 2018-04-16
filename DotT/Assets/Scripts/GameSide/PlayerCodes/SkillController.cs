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

	float[] cooldown = new float[4];
	float[] curCooldown = new float[4];

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
			if (myHero != null) {
				foreach (SkillMasterClass skill in GetComponent<PlayerSpawner>().myHero.GetComponentsInChildren<SkillMasterClass>()) {
					switch (skill.mySettings.skillButton) {
					case SkillSettings.Buttons.Q:
						QSkill = skill.ExecuteSkill;
						cooldown [0] = skill.mySettings.cooldown;
						SkillCooldownDisplay.disps [0].cooldown = cooldown [0];
						break;
					case SkillSettings.Buttons.W:
						WSkill = skill.ExecuteSkill;
						cooldown [1] = skill.mySettings.cooldown;
						SkillCooldownDisplay.disps [1].cooldown = cooldown [1];
						break;
					case SkillSettings.Buttons.E:
						ESkill = skill.ExecuteSkill;
						cooldown [2] = skill.mySettings.cooldown;
						SkillCooldownDisplay.disps [2].cooldown = cooldown [2];
						break;
					case SkillSettings.Buttons.R:
						RSkill = skill.ExecuteSkill;
						cooldown [3] = skill.mySettings.cooldown;
						SkillCooldownDisplay.disps [3].cooldown = cooldown [3];
						break;
					}
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
	void RpcHookUpClient (GameObject myHero){
		foreach (SkillMasterClass skill in myHero.GetComponentsInChildren<SkillMasterClass>()) {
			switch (skill.mySettings.skillButton) {
			case SkillSettings.Buttons.Q:
				QSkill = skill.ExecuteSkill;
				cooldown [0] = skill.mySettings.cooldown;
				SkillCooldownDisplay.disps [0].cooldown = cooldown [0];
				break;
			case SkillSettings.Buttons.W:
				WSkill = skill.ExecuteSkill;
				cooldown [1] = skill.mySettings.cooldown;
				SkillCooldownDisplay.disps [1].cooldown = cooldown [1];
				break;
			case SkillSettings.Buttons.E:
				ESkill = skill.ExecuteSkill;
				cooldown [2] = skill.mySettings.cooldown;
				SkillCooldownDisplay.disps [2].cooldown = cooldown [2];
				break;
			case SkillSettings.Buttons.R:
				RSkill = skill.ExecuteSkill;
				cooldown [3] = skill.mySettings.cooldown;
				SkillCooldownDisplay.disps [3].cooldown = cooldown [3];
				break;
			}
		}
	}


	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer && !PlayerSpawner.LocalPlayerSpawner.myHealth.isDead) {
			if (Input.GetKeyDown (KeyCode.Q) && curCooldown [0] <= 0) {
				curCooldown [0] = cooldown [0];
				ExecuteSkill (SkillSettings.Buttons.Q);
			}
			if (Input.GetKeyDown (KeyCode.W) && curCooldown [1] <= 0) {
				curCooldown [1] = cooldown [1];
				ExecuteSkill (SkillSettings.Buttons.W);
			}
			if (Input.GetKeyDown (KeyCode.E) && curCooldown [2] <= 0) {
				curCooldown [2] = cooldown [2];
				ExecuteSkill (SkillSettings.Buttons.E);
			}
			if (Input.GetKeyDown (KeyCode.R) && curCooldown [3] <= 0) {
				curCooldown [3] = cooldown [3];
				ExecuteSkill (SkillSettings.Buttons.R);
			}

			for (int i = 0; i < cooldown.Length; i++) {
				curCooldown[i] -= Time.deltaTime;
				curCooldown [i] = Mathf.Clamp (curCooldown [i], 0, cooldown [i]);
				SkillCooldownDisplay.disps [i].curCooldown = curCooldown [i];
			}
		}
	}


	void ExecuteSkill (SkillSettings.Buttons mySkillType){
		print (gameObject.name + " skill event triggered");
		RaycastHit hit; 
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		int layerMask = 1 << 8;
		if (Physics.Raycast (ray, out hit, 100.0f, layerMask)) {
			CmdExecuteSkill (mySkillType, hit.point);
		}
	}

	[Command]
	void CmdExecuteSkill (SkillSettings.Buttons mySkillType, Vector3 executePos){
		print (gameObject.name + " executed skill in Server "+ mySkillType.ToString());
		ExecutionData myData = new ExecutionData (true, myHero.transform.position, executePos, executePos.Floorize() - myHero.transform.position.Floorize());
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
	}

	[ClientRpc]
	void RpcExecuteSkill (SkillSettings.Buttons mySkillType, ExecutionData myData){
		print (gameObject.name + " executed skill in Client "+ mySkillType.ToString());
		myData.isServer = false;
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
	}
}
