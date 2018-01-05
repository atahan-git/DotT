using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEnabledIsTrue : MonoBehaviour
{
    public GameObject target;

	void Start ()
    {
        target.SetActive(true);
	}
}
