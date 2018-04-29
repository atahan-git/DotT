using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STORAGE_HeroPrefabs : MonoBehaviour
{
	public static STORAGE_HeroPrefabs s;

	public GameObject[] heroes = new GameObject[4];
	public Sprite[] heroIcons = new Sprite[4];
	[TextArea]
	public string[] heroTooltips = new string[4];

	void Awake()
    {
		s = this;
	}
}
