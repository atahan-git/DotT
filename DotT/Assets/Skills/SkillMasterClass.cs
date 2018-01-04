using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-----------------------------THIS SCRIPT SHOULD BE CALLED BOTH IN THE SERVER AND IN THE CLIENT TO WORK
public class SkillMasterClass : MonoBehaviour {
	protected delegate IEnumerator ChildDelegate (bool isServer);

	public SkillSettings mySettings;

	Health self{
		get{
			return GetComponent<Health> ();
		}
	}
	Health target{
		get{
			return activeTelegraph.GetTarget ();
		}
	}
	MultipleHealths areaTargets{
		get{
			return activeTelegraph.GetAreaTargets ();
		}
	}
	Health firstInLine;
	Health[] targetsInLine; //if gonna hit more than one thing in line
	MultipleHealths[] allTriggerValues; //skills that damage more in the middle for example

	public Vector3 executePos;
	public TelegraphController activeTelegraph; //put this as a child of skill object

	//--------------------------------------------------------------EXECUTE BLOCK
	public void ExecuteSkill (bool isServer, Vector3 _executePos){
		executePos = _executePos;
		StartCoroutine (Execute (isServer));
	}

	//use new IEnumerable Execute(){} to define your own method inside this guy
	protected ChildDelegate Execute;


	//built in triggered methods
	protected ChildDelegate Casting;	//this will be triggered when the cast time begins, use this for showing telegrapghs
	//allways gets triggered even if the cast time is zero

	//IEnumerable Execute(){}	//this will be triggered as soon as the cast time ends

	//for toggle only
	protected ChildDelegate ToggleOpen;
	protected ChildDelegate ToggleOpenStay;
	protected ChildDelegate ToggleClose;
	protected ChildDelegate ToggleCloseStay;


	//----------------------------------------------------------------------------------------------------------------------HELPER METHODS
	protected void TriggerAnimation(string name){}	//triggers an animation in the player model

	//--------------------------------------------------------------TELEGRAPHS
	//displays the set telegrapgh based on the skill type.
	protected void DisplayTelegraph(){
		try {
			activeTelegraph.ShowTelegraph ();
			activeTelegraph.SetPosition (executePos);
			print (gameObject.name + " ShowTelegraph");
		} catch (System.Exception e) {
			print (e.StackTrace);
		}
	}
	//over head effect for targeted, ground telegrapgh for area and directional, 
	//disabled for istant and toggle, amazingly advanced vectors for vector stuff
	protected void HideTelegraph(){
		try {
			activeTelegraph.HideTelegraph ();
			print (gameObject.name + " HideTelegraph");
		} catch (System.Exception e) {
			print (e.StackTrace);
		}
	}

	protected void ChangeTelegrapghState(int state){}	//for changing telegrapgh state (talents etc.)


	//--------------------------------------------------------------EFFECT STUFF
	//for spawning Effects[-index-] prefab. location is the same as DisplayTelegrapgh() location
	//InstantiateEffect(-index-, settings);	//will add settings as needed - maybe custom locations?
	public ObjectPool[] effectPools = new ObjectPool[1];

	protected void InstantiateEffect(bool isServer, int index){
		if (!isServer)
			return;
		effectPools [index].Spawn (executePos);
	}	


	//--------------------------------------------------------------DAMAGE & OTHER EFFECTS STUFF
	//damages the target based on skill type (target/area/instant(self)/directional(same as area)/toggle(self))
	//any one of this can be added for more control:
	//-target-	//self/target/areatargets/areatargets[id]	pass the built in variable
	//-value-		//any other value in otherValues array		pass the index in the array
	protected void Damage(bool isServer){
		if (!isServer)
			return;

		switch(mySettings.skillType){
		case SkillSettings.Types.Area:
			Damage (isServer, areaTargets);
			break;

		}
	}

	protected void Damage (bool isServer, MultipleHealths myTargets){
		if (!isServer)
			return;

		foreach (Health hl in myTargets.healths) {
			if (hl != null) {
				hl.ModifyHealth (mySettings.damage, mySettings.damageType);
			}
		}
	}

	//Damage(), Damage(-target-), Damage(-value-), Damage(-target-, -value-);

	protected void Heal(){}		//same as damage but heals, custom values can also be used

	protected void ApplyEffect(bool isServer){}		//Aplies status effect


	//--------------------------------------------------------------STRUCTURE AND PROJECTILE STUFF
	protected void InstantiateProjectile(){}	//is only possible for Directional skils, spawns a projectile in the said direction
	//InstantiateProjectile(-pos-,-rot-);	//for more special projectiles, Xul things for example

	protected void InstantiateStructure(){
	}		//is only possible for TargetedStructure skills, spawns a structure in the selected position
}
	
public class MultipleHealths{
	public Health[] healths;

	public void ClosestToPoint (Vector3 point){

	}

	public MultipleHealths (Health[] myHealths){
		healths = myHealths;
	}
}