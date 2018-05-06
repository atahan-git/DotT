using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Q_Concentrate : SkillMasterClass {
	void Start (){
		Execute = _Execute;
		SkillName = "Q_Concentrate";
	}


	IEnumerator _Execute(ExecutionData data){

		TriggerAnimation ();
		DisplayTelegraph (data);

		SpendMana (data);
		Damage (data);
		ApplyEffect(data);

		yield return new WaitForSeconds (mySettings.damageOverTime);

		HideTelegraph (data);
	}
}
