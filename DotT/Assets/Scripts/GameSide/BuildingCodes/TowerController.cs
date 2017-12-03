using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public float range = 10;

    public Transform target;

    private void Start()
    {
        FindTarget();
        print("hello, i am " + gameObject.name);
    }

    void FindTarget()
    {
        Collider[] inRangeColliders = Physics.OverlapSphere(transform.position, range);

        target = inRangeColliders[0].gameObject.transform;

        foreach(Collider collider in inRangeColliders)
        {
            print("i am " + collider.gameObject.name + " in " + gameObject.name);
            if (collider.gameObject.GetComponent<Health>() != null)
            {
                if (Vector3.Distance(transform.position, collider.transform.position) < Vector3.Distance(transform.position, target.position))
                {
                    print("new target is " + collider.gameObject.name + " for " + gameObject.name);
                    target = collider.gameObject.transform;
                }
            }
        }
    }
}
