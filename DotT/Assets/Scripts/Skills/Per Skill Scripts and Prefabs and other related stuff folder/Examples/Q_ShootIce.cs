using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Q_ShootIce : SkillMasterClass {
	void Start (){
		Execute = _Execute;
	}


	IEnumerator _Execute(ExecutionData data){
		InstantiateProjectile (data);

		yield return null;
	}
}
