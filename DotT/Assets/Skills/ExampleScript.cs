using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : SkillMasterClass {
	void Start (){
		Execute = _Execute;
	}


	IEnumerator _Execute(bool isServer){

		DisplayTelegraph();	

		yield return new WaitForSeconds(mySettings.waitTimes[0]);

		for(int i = 0; i < 3; i++){

			InstantiateEffect(isServer, 0);

			yield return new WaitForSeconds(mySettings.waitTimes[1]);

			Damage(isServer);
			ApplyEffect(isServer);

			yield return new WaitForSeconds(mySettings.waitTimes[2]);
		}

		yield return new WaitForSeconds(mySettings.waitTimes[3]);

		HideTelegraph();
	}
}
