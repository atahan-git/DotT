using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingRock : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Travel(Vector3 heroPos, Vector3 executePos, float travelTime, float travelHeight){

		StopAllCoroutines ();

		StartCoroutine (_Travel (heroPos, executePos, travelTime, travelHeight));
	}

	IEnumerator _Travel(Vector3 heroPos, Vector3 executePos, float travelTime, float travelHeight){
		GetComponentInParent<PooledObject> ().lifeTime = travelTime;

		float totalDistance = Vector3.Distance (heroPos, executePos);
		float currentDistance = totalDistance;


		while (currentDistance > 0.01f) {

			transform.position = Vector3.MoveTowards (transform.position.Floorize(),executePos.Floorize(), (totalDistance/travelTime) * Time.deltaTime);
			currentDistance = Vector3.Distance(transform.position, executePos.Floorize());

			transform.position += Vector3.up * Mathf.Lerp (travelHeight, executePos.y, Mathf.Pow (Mathf.Abs (((totalDistance / 2) - currentDistance) / (totalDistance / 2)), 2));

			yield return null;
		}
	}
}
