using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STORAGE_HealthPrefabs : MonoBehaviour
{
	public static STORAGE_HealthPrefabs s;

    public GameObject healthBarLine;
    public GameObject mileStoneLine;
    public GameObject heroHealthBar;

    void Awake()
    {
		s = this;
	}
}
