using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//-----------------------------THIS SCRIPT SHOULD BE CALLED BOTH IN THE SERVER AND IN THE CLIENT TO WORK
public class SkillMasterClass : MonoBehaviour {
	protected delegate IEnumerator ChildDelegate (ExecutionData data);

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

	public TelegraphController activeTelegraph; //put this as a child of skill object

	//--------------------------------------------------------------EXECUTE BLOCK
	public void ExecuteSkill (ExecutionData data){
		StartCoroutine (Execute (data));
	}

	//use IEnumerable Execute(){} to define your own method inside this guy
	//REMEMBER TO REFERENCE THIS IN YOUR CHILD SCRIPT
	//Check an example script for details
	protected ChildDelegate Execute;


	//built in triggered methods
	protected ChildDelegate Casting;	//this will be triggered when the cast time begins, use this for showing telegrapghs
	//allways gets triggered even if the cast time is zero

	//for toggle only
	protected ChildDelegate ToggleOpen;
	protected ChildDelegate ToggleOpenStay;
	protected ChildDelegate ToggleClose;
	protected ChildDelegate ToggleCloseStay;


	//----------------------------------------------------------------------------------------------------------------------HELPER METHODS
	protected void TriggerAnimation(string name){	//triggers an animation in the player model
		GetComponentInParent<Animator> ().SetTrigger (name);
	}

	protected void SpendMana (ExecutionData data){
		if(data.isServer)
			((MonoBehaviour)self.myRespawnManager).GetComponent<SkillController> ().mana -= mySettings.manaCost;
	}

	//--------------------------------------------------------------TELEGRAPHS
	//displays the set telegrapgh based on the skill type.
	protected void DisplayTelegraph(ExecutionData data){
		try {
			activeTelegraph.ShowTelegraph ();
			activeTelegraph.SetPosition (data.executePos);
			print (gameObject.name + " ShowTelegraph");
		} catch (System.Exception e) {
			Debug.LogError (e.StackTrace);
		}
	}
	//over head effect for targeted, ground telegrapgh for area and directional,
	//There wont be any telegraphs for istant and toggle. There will be amazingly advanced vectors for vector stuff (hopefully)

	protected void HideTelegraph(){
		try {
			activeTelegraph.HideTelegraph ();
			print (gameObject.name + " HideTelegraph");
		} catch (System.Exception e) {
			Debug.LogError (e.StackTrace);
		}
	}

	protected void ChangeTelegrapghState(int state){}	//for changing telegrapgh state (talents etc.)


	//--------------------------------------------------------------EFFECT STUFF
	//for spawning Effects[-index-] prefab. location is the same as DisplayTelegrapgh() location
	//InstantiateEffect(-index-, settings);	//will add settings as needed - maybe custom locations?
	public ObjectPool[] effectPools = new ObjectPool[1];

	protected void InstantiateEffect(ExecutionData data, int index){
		if (!data.isServer)
			return;
		effectPools [index].Spawn (data.executePos);
	}	


	//--------------------------------------------------------------DAMAGE & OTHER EFFECTS STUFF
	//damages the target based on skill type (target/area/instant(self)/directional(same as area)/toggle(self))
	//any one of this can be added for more control:
	//-target-	//self/target/areatargets/areatargets[id]	pass the built in variable
	//-value-		//any other value in otherValues array		pass the index in the array
	protected void Damage(ExecutionData data){
		if (!data.isServer)
			return;

		switch(mySettings.skillType){
		case SkillSettings.Types.Area:
			Damage (data, areaTargets);
			break;

		}
	}

	protected void Damage (ExecutionData data, MultipleHealths myTargets){
		if (!data.isServer)
			return;

		foreach (Health hl in myTargets.healths) {
			if (hl != null) {
				hl.Damage (mySettings.damage, mySettings.damageType, self.mySide);
			}
		}
	}

	//Damage(), Damage(-target-), Damage(-value-), Damage(-target-, -value-);

	protected void Heal(){}		//same as damage but heals, custom values can also be used

	protected void ApplyEffect(ExecutionData data){}		//Aplies status effect


	//--------------------------------------------------------------STRUCTURE AND PROJECTILE STUFF
	//Functioning Prefabs with their own settings
	public ObjectPool[] Projectiles;
	public ObjectPool[] Structures;

	protected void InstantiateProjectile(ExecutionData data){	//is only possible for Directional skils, spawns a projectile in the said direction
		if (!data.isServer)
			return;

		GameObject myProjectile = Projectiles [0].Spawn (data.heroPos + (Projectiles [0].myObject.GetComponentInChildren<AdvancedProjectile> ().myHeight * Vector3.up), data.executeRot);
		myProjectile.GetComponentInChildren<AdvancedProjectile> ().damage = mySettings.damage;
		myProjectile.GetComponentInChildren<AdvancedProjectile> ().mySide = self.mySide;
	}
	//InstantiateProjectile(-pos-,-rot-);	//for more special projectiles, Xul things for example

	protected void InstantiateStructure(){
	}		//is only possible for TargetedStructure skills, spawns a structure in the selected position
}
	
public class MultipleHealths{
	public Health[] healths;

	public Health ClosestToPoint (Vector3 point){
		Health closest = null;
		float dist = 0;

		foreach (Health h in healths) {
			float myDist = Vector3.Distance (h.transform.position, point);

			if (myDist < dist) {
				dist = myDist;
				closest = h;
			}
		}

		return closest;
	}

	public MultipleHealths (Health[] _healths){
		healths = _healths;
	}
}

[System.Serializable]
public class ExecutionData{
	public bool isServer;
	public Vector3 heroPos;
	public Vector3 executePos;
	public Vector3 executeDir;
	public Quaternion executeRot{
		get{
			return Quaternion.LookRotation (executeDir);
		}
		set{
			executeDir = value * Vector3.forward;
		}
	}

	public ExecutionData(){}


	public ExecutionData(bool _isServer,Vector3 _heroPos, Vector3 _executePos, Vector3 _executeDir){
		isServer = _isServer;
		heroPos = _heroPos;
		executePos = _executePos;
		executeDir = _executeDir;
	}
}