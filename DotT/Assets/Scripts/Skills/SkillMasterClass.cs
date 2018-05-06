using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//-----------------------------Removed the dumb both server & client stuff. This will be called on server only an the rest will be adjusted
public class SkillMasterClass : NetworkBehaviour{
	protected delegate IEnumerator ChildDelegate (ExecutionData data);

	public SkillSettings mySettings;

	Health self{
		get{
			return GetComponent<Health> ();
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
		if (isServer) {
			data.myType = this.GetType ().Name;
			print (SkillName + " activated from skillmasterclass");
			StartCoroutine (Execute (data));
		}
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
		GetComponentInChildren<Animator> ().SetTrigger (name);
		RpcTriggerAnimation (name);
	}

	protected void TriggerAnimation(){	//triggers the cast animation in the player model
		TriggerAnimation("Cast");
	}
	[ClientRpc]
	void RpcTriggerAnimation (string name){
		GetComponentInChildren<Animator> ().SetTrigger (name);
	}

	protected void SpendMana (ExecutionData data){
		if(data.isServer)
			((MonoBehaviour)self.myRespawnManager).GetComponent<SkillController> ().mana -= mySettings.manaCost;
	}

	//--------------------------------------------------------------TELEGRAPHS
	//displays the set telegrapgh based on the skill type.
	public string SkillName = "def";
	public void DisplayTelegraph(ExecutionData data){
		if (!data.isServer && this.GetType().Name != data.myType) {
			print ("Fuck you UNet, WRONG RPC TYPE... correcting " + this.GetType().Name + " >> " + data.myType);
			((SkillMasterClass)GetComponent (data.myType)).DisplayTelegraph (data);
			return;
		}
			
		try {
			switch(mySettings.skillType){
			case SkillSettings.Types.Area:
				activeTelegraph.ShowTelegraph ();
				activeTelegraph.SetPosition (data.executePos);
				print (gameObject.name + " ShowTelegraph - " + data.isServer + " - " + SkillName);
				break;
			case SkillSettings.Types.Targeted:
				activeTelegraph.ShowTelegraph (data.target.transform);
				print (gameObject.name + " ShowTelegraph over target - " + data.isServer + " - " + SkillName);
				break;
			}
			if(data.isServer)
				RpcDisplayTelegraph(data);
		} catch (System.Exception e) {
			Debug.LogError (e.Message + " - " + e.StackTrace);
		}
	}
	[ClientRpc]
	void RpcDisplayTelegraph(ExecutionData data){
		data.isServer = false;
		DisplayTelegraph (data);
	}

	//over head effect for targeted, ground telegrapgh for area and directional,
	//There wont be any telegraphs for instant and toggle. There will be amazingly advanced vectors for vector stuff (hopefully)

	public void HideTelegraph(ExecutionData data){
		if (!data.isServer && this.GetType().Name != data.myType) {
			print ("Fuck you UNet, WRONG RPC TYPE... correcting " + this.GetType().Name + " >> " + data.myType);
			((SkillMasterClass)GetComponent (data.myType)).HideTelegraph (data);
			return;
		}
		try {
			activeTelegraph.HideTelegraph ();
			print (gameObject.name + " HideTelegraph");
			if(data.isServer)
				RpcHideTelegraph(data);
		} catch (System.Exception e) {
			Debug.LogError (e.StackTrace);
		}
	}

	[ClientRpc]
	void RpcHideTelegraph(ExecutionData data){
		data.isServer = false;
		HideTelegraph (data);
	}

	protected void ChangeTelegrapghState(int state){}	//for changing telegrapgh state (talents etc.)


	//--------------------------------------------------------------EFFECT STUFF
	//for spawning Effects[-index-] prefab. location is the same as DisplayTelegrapgh() location
	//InstantiateEffect(-index-, settings);	//will add settings as needed - maybe custom locations?
	public ObjectPool[] effectPools = new ObjectPool[1];

	protected GameObject InstantiateEffect(ExecutionData data, int index, Vector3 position){
		if (!data.isServer)
			return null;
		GameObject myObj = effectPools [index].Spawn (position);
		return myObj;
	}	

	protected GameObject InstantiateEffect(ExecutionData data, int index){
		return InstantiateEffect (data, index, data.executePos);
	}	

	protected GameObject InstantiateEffectOverTelegraph(ExecutionData data, int index){
		if (!data.isServer)
			return null;
		GameObject myObj = InstantiateEffect (data, index, activeTelegraph.transform.position);
		myObj.transform.parent = activeTelegraph.transform;
		return myObj;
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
		case SkillSettings.Types.Targeted:
			Damage (data, new MultipleHealths (new Health[]{ data.target.GetComponent<Health> () }));
			break;
		}
	}

	protected void Damage (ExecutionData data, MultipleHealths myTargets){
		if (!data.isServer)
			return;

		Damage (data, myTargets, mySettings.damage);
	}

	protected void Damage (ExecutionData data, MultipleHealths myTargets, float damageValue){
		if (!data.isServer)
			return;

		foreach (Health hl in myTargets.healths) {
			if (hl != null) {
				if (mySettings.damageOverTime > 0) {
					hl.Damage (damageValue, mySettings.damageType, mySettings.damageOverTime, self.mySide);
				} else {
					hl.Damage (damageValue, mySettings.damageType, self.mySide);
				}
			}
		}
	}

	//Damage(), Damage(-target-), Damage(-value-), Damage(-target-, -value-);

	protected void Heal(){}		//same as damage but heals, custom values can also be used

	protected void ApplyEffect(ExecutionData data){		//Aplies status effect
		if (!data.isServer)
			return;

		switch(mySettings.skillType){
		case SkillSettings.Types.Area:
			ApplyEffect (data, areaTargets);
			break;
		case SkillSettings.Types.Targeted:
			ApplyEffect (data, new MultipleHealths (new Health[]{ data.target.GetComponent<Health> () }));
			break;
		}
	}

	protected void ApplyEffect (ExecutionData data, MultipleHealths myTargets){
		if (!data.isServer)
			return;

		foreach (Health hl in myTargets.healths) {
			if (hl != null) {
				switch (mySettings.myEffect) {
				case SkillSettings.StatusEffect.Slow:
					hl.ModifyMovementSpeed (mySettings.myEffectAmount,mySettings.myEffectDuration);
					break;
				case SkillSettings.StatusEffect.Stun:
					hl.Stun (mySettings.myEffectDuration);
					break;
				}
			}
		}
	}


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
	public GameObject target; //only has value if its a targeted skill
	public Vector3 heroPos;
	public Vector3 executePos;
	public Vector3 executeDir;
	public string myType;
	public Quaternion executeRot{
		get{
			return Quaternion.LookRotation (executeDir);
		}
		set{
			executeDir = value * Vector3.forward;
		}
	}

	public ExecutionData(){}


	public ExecutionData(bool _isServer,Vector3 _heroPos, Vector3 _executePos, Vector3 _executeDir, GameObject _target){
		isServer = _isServer;
		heroPos = _heroPos;
		executePos = _executePos;
		executeDir = _executeDir;
		target = _target;
	}
}