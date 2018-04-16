using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STORAGE_HealthPrefabs : MonoBehaviour
{
	public static STORAGE_HealthPrefabs s;

    public GameObject healthBarLine;
    public GameObject mileStoneLine;
    public GameObject heroHealthBar;

	public Color enemyColor1 = Color.red;
	public Color enemyColor2 = Color.magenta;
	public Color playerColor = Color.green;
	public Color allyColor = Color.blue;
	public Color neutralColor = Color.yellow;

    void Awake()
    {
		s = this;
	}
}
