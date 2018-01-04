//Pre Scriptum: Don't mind the file name. Most of this code isn't stolen.

//Attack not working as writing the health code is not my job
//Also, no, Atahan, putting that foreach loop in Update does not cause any lag. Tried with 10'000 minions and observed no FPS drop.

using UnityEngine;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Minion : MonoBehaviour{
    public Transform target; //Pathfinding target
    public Vector3 destination = Vector3.zero;//Cached version of the target, initialised to (0,0,0)
    UnityEngine.AI.NavMeshAgent agent; //The NavMeshAgent
	
	//<possible targets>
	public Transform enemyNexus;
	public Transform[] enemyMinionsOnLane;
	public Transform[] enemyPlayers;
	public Transform[] enemyTowers;
	//</possible targets>
	
	float minionTrackRange = 30f;
	float playerTrackRange = 30f;
	float towerTrackRange = 0x7FFFFFFF; //INT_MAX
	float attackRange = 5f;
	
	//<Poor man's enum>
	public int trackingType;
	
	public const int TRACKING_MINION = 0;
	public const int TRACKING_PLAYER = 1;
	public const int TRACKING_TOWER = 2;
	//</Poor man's enum>
	
	public int damage = 50; //Damage dealt when attacking
	
	public bool paused = false; //Whether pathfinding is paused or not
	
	public Transform attackFocus;
	public int attackCooldown = 50; // * 0.02 s
	private int attackCooldownCurrent = 0;
	
    void Start(){
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        destination = agent.destination;
    }
	
	void FixedUpdate(){
		if(attackFocus != null){
			//try to attack
			if(attackCooldownCurrent == 0){
				Attack();
				attackCooldownCurrent = attackCooldown;
			}else{
				attackCooldownCurrent--;
			}
		}
	}
	
    void Update(){
		if(!paused){//If pathfinding hasn't been paused,
			target = enemyNexus;//Target the enemy nexus by default
			
			//Lowest priority in pathfinding is players
			float closestDistancePlayer = 0x7FFFFFFF;
			
			foreach(Transform enemyPlayer in enemyPlayers){
				if(enemyPlayer == null) continue;
				float distance = Vector3.Distance(transform.position, enemyPlayer.position);
				if(distance < closestDistancePlayer && distance < playerTrackRange && enemyPlayer.gameObject.activeSelf){
					closestDistancePlayer = distance;
					target = enemyPlayer;
					trackingType = TRACKING_PLAYER;
				}
			}
			
			//Second priority are towers
			float closestDistanceEnemyTower = 0x7FFFFFFF;
			
			foreach(Transform enemyTower in enemyTowers){
				if(enemyTower == null) continue;
				float distance = Vector3.Distance(transform.position, enemyTower.position);
				if(distance < closestDistanceEnemyTower && distance < towerTrackRange){
					if(enemyTower.GetComponent<Health>().currentHealth > 0 && enemyTower.gameObject.activeSelf){
						closestDistanceEnemyTower = distance;
						target = enemyTower;
						trackingType = TRACKING_TOWER;
					}
				}
			}
			
			//Highest priority are enemy minions
			float closestDistanceMinion = 0x7FFFFFFF;
			
			foreach(Transform enemyMinionOnLane in enemyMinionsOnLane){
				if(enemyMinionOnLane == null) continue;
				float distance = Vector3.Distance(transform.position, enemyMinionOnLane.position);
				if(distance < closestDistanceMinion && distance < minionTrackRange && enemyMinionOnLane.gameObject.activeSelf){
					closestDistanceMinion = distance;
					target = enemyMinionOnLane;
					trackingType = TRACKING_MINION;
				}
			}
		}
		
		if(target != null){//If we have found a target
			float f = Vector3.Distance(transform.position, target.position);//Calculate the distance between our current position and our target's position.
			if(f < attackRange){//If it is less than the attack range,
				paused = true;//Pause pathfinding so the minion stops.
				if(attackFocus == null) attackFocus = target;//If there isn't an active attack focus on an entity, set the attack focus to the current target (fixes a bug that is triggered when two enemies of the same priority are int the attack range)
				agent.destination = transform.position; //Set the NavMeshAgent destination to the current position (fixes a bug but honestşy, I don't remmeber which bug it fixes)
			}else{//If not, 
				paused = false;//Unpause pathfinding
				attackFocus = null;//And clear the attackFocus
			}
			
			
			if(Vector3.Distance(destination, target.position) > 1.0f){//If the cached destination and the targets position differ by more than 1 unit
				destination = target.position; //Cache the new position of the target
				agent.destination = destination; //Set the NavMeshAgent destination to the cahced position
			}
		}
		
    }
	
	/**
	*Stand-in for an actual attack method
	*/
	void Attack(){
		print("pew");
		//attackFocus.gameObject.GetComponent<Health>().dealDamage(damage);
	}
	
}