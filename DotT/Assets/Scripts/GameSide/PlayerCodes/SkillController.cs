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
		}
	}


	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			if (Input.GetKeyDown (KeyCode.Q))
				ExecuteSkill (SkillSettings.Buttons.Q);
			if (Input.GetKeyDown (KeyCode.W))
				ExecuteSkill (SkillSettings.Buttons.W);
			if (Input.GetKeyDown (KeyCode.E))
				ExecuteSkill (SkillSettings.Buttons.E);
			if (Input.GetKeyDown (KeyCode.R))
				ExecuteSkill (SkillSettings.Buttons.R);
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
		ExecutionData myData = new ExecutionData (true, executePos, executePos - myHero.transform.position);
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
