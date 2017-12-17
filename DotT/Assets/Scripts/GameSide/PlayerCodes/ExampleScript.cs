using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : SkillMasterClass {

	new IEnumerable Execute(){
		
		DisplayTelegraph();	

		yield return new WaitForSeconds(mySettings.waitTimes[0]);

		for(int i = 0; i < 3; i++){

			InstantiateEffect(0);

			yield return new WaitForSeconds(mySettings.waitTimes[1]);

			Damage();
			ApplyEffect();

			yield return new WaitForSeconds(mySettings.waitTimes[2]);
		}

		yield return new WaitForSeconds(mySettings.waitTimes[3]);

		HideTelegraph();
	}
}
