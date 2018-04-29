using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_BlizzardSkill : SkillMasterClass {
	void Start (){
		Execute = _Execute;
	}


	IEnumerator _Execute(ExecutionData data){

		DisplayTelegraph(data);	
		SpendMana (data);

		yield return new WaitForSeconds(mySettings.waitTimes[0]);

		for(int i = 0; i < 3; i++){

			InstantiateEffect(data, 0);

			yield return new WaitForSeconds(mySettings.waitTimes[1]);

			Damage(data);
			ApplyEffect(data);

			yield return new WaitForSeconds(mySettings.waitTimes[2]);
		}

		yield return new WaitForSeconds(mySettings.waitTimes[3]);

		HideTelegraph();
	}
}
