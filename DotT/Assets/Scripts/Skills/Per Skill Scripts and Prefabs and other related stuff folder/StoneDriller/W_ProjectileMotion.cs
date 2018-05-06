using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class W_ProjectileMotion : SkillMasterClass {
	void Start (){
		Execute = _Execute;
		SkillName = "W_ProjectileMotion";
	}


	IEnumerator _Execute(ExecutionData data){

		TriggerAnimation ();
		DisplayTelegraph(data);	
		SpendMana (data);

		GameObject myRock = InstantiateEffect (data, 0, data.heroPos);
		Travel (true, myRock, data.heroPos, data.executePos, mySettings.otherValues[0], mySettings.otherValues[1]);

		yield return new WaitForSeconds (mySettings.otherValues[0]); // wait until the rock travels

		List<Health> outerRing = new List<Health> (activeTelegraph.GetAreaTargets (0).healths);
		List<Health> innerRing = new List<Health> (activeTelegraph.GetAreaTargets (1).healths);

		foreach (Health obj in innerRing) {
			outerRing.Remove (obj);
		}

		Damage (data, new MultipleHealths (innerRing.ToArray ()), mySettings.damage * mySettings.otherValues [2]);
		Damage (data, new MultipleHealths (outerRing.ToArray ()), mySettings.damage);

		ApplyEffect(data, new MultipleHealths (innerRing.ToArray ()));
		InstantiateEffect (data, 1);

		HideTelegraph(data);
		if (data.isServer)
			myRock.GetComponent<PooledObject> ().DestroyPooledObject ();
	}

	void Travel (bool _isServer, GameObject myRock, Vector3 heroPos, Vector3 executePos, float travelTime, float travelHeight){
		if (_isServer) 
			RpcSetValues (myRock, heroPos, executePos, travelTime, travelHeight);

		myRock.GetComponentInChildren<ThrowingRock> ().Travel (heroPos, executePos, travelTime, travelHeight);
	}

	[ClientRpc]
	void RpcSetValues (GameObject myRock, Vector3 heroPos, Vector3 executePos, float travelTime, float travelHeight){
		Travel (false, myRock, heroPos, executePos, travelTime, travelHeight);
	}
}
