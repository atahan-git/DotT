using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMasterClass : MonoBehaviour {

	Health self;
	Health target;
	MultipleHealths areaTargets;
	Health firstInLine;
	Health[] targetsInLine; //if gonna hit more than one thing in line
	MultipleHealths[] allTriggerValues; //skills that damage more in the middle for example

	public ScriptSettings mySettings;

	//use new IEnumerable Execute(){} to define your own method inside this guy
	IEnumerable Execute(){
		//do something amazing
		yield return 0;
	}

	protected void TriggerAnimation(string name){}	//triggers an animation in the player model

	//displays the set telegrapgh based on the skill type.
	protected void DisplayTelegraph(){


	}
	//over head effect for targeted, ground telegrapgh for area and directional, 
	//disabled for istant and toggle, amazingly advanced vectors for vector stuff
	protected void HideTelegraph(){

	}

	protected void ChangeTelegrapghState(int state){}	//for changing telegrapgh state (talents etc.)



	protected void InstantiateEffect(int index){
	}	//for spawning Effects[-index-] prefab. location is the same as DisplayTelegrapgh() location
	//InstantiateEffect(-index-, settings);	//will add settings as needed - maybe custom locations?

	protected void Damage(){}	//damages the target based on skill type (target/area/instant(self)/directional(same as area)/toggle(self))
	//any one of this can be added for more control:
	//-target-	//self/target/areatargets/areatargets[id]	pass the built in variable
	//-value-		//any other value in otherValues array		pass the index in the array

	//Damage(), Damage(-target-), Damage(-value-), Damage(-target-, -value-);

	protected void Heal(){}		//same as damage but heals, custom values can also be used

	protected void ApplyEffect(){}		//Aplies status effect

	protected void InstantiateProjectile(){}	//is only possible for Directional skils, spawns a projectile in the said direction
	//InstantiateProjectile(-pos-,-rot-);	//for more special projectiles, Xul things for example

	protected void InstantiateStructure(){
	}		//is only possible for TargetedStructure skills, spawns a structure in the selected position

	//built in triggered methods
	IEnumerable Casting(){yield return 0;}	//this will be triggered when the cast time begins, use this for showing telegrapghs
	//allways gets triggered even if the cast time is zero

	//IEnumerable Execute(){}	//this will be triggered as soon as the cast time ends

	//for toggle only
	IEnumerable ToggleOpen(){yield return 0;}
	IEnumerable ToggleOpenStay(){yield return 0;}
	IEnumerable ToggleClose(){yield return 0;}
	IEnumerable ToggleCloseStay(){yield return 0;}
}
	
public class MultipleHealths{

}

[CreateAssetMenu(fileName = "Script Settings", menuName = "Script Settings", order = 3)]
public class ScriptSettings : ScriptableObject {
	public enum RatioType{};
	public enum Buttons {Q,W,E,R,D,one,two,three,four,five};
	public enum Types {Targeted, Area, Instant, Directional, Toggle, Vector, TargetedStructure};
	public enum Target {Enemy, Ally, Both};
	public enum StatusEffect {Slow};


	public Buttons scriptButton = Buttons.Q ;//Q,W,E,R,D,1-5

	//see the end for the rest
	public Types scriptType = Types.Area;

	public Target scriptTarget = Target.Enemy;

	//only one available, will show up on the ground, can be left empty, can have multiple states, triggers for talents, and animations
	public GameObject telegrapgh;	//see the end for more details

	//add as many as needed - non functional visible effects
	public GameObject[] Effects;

	//Functioning Prefabs with their own settings, see the end for more details
	public GameObject[] Projectiles;
	public GameObject[] Structures;

	public int damage;
	public RatioType damageType;	//also includes a "normal" in addition to other values in the Health script
	public int heal;
	public RatioType healType;

	//multiple values for more complex skills
	public int[] otherValues;
	public RatioType[] otherValueType;

	//any status effects the skill may apply - maybe this should be an array?
	public StatusEffect myEffect = StatusEffect.Slow;
	public float myEffectDuration = 1f;
	public float myEffectAmount = 0.4f;

	//values used in the script itself
	public float[] waitTimes = {0.2f,0.1f,0.3f,0.2f};


	public int manaCost;

	public float cooldown;

	//use 0 for insta cast skills
	public float castTime;
	public string castAnimation = "ShootBlizzard";	//will triggered said animation in the hero
}
