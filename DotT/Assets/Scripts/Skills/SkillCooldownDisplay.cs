using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownDisplay : MonoBehaviour {

	public static SkillCooldownDisplay[] disps = new SkillCooldownDisplay[4];

	public SkillSettings.Buttons myType;

	public float cooldown = 5f;
	public float curCooldown;

	bool isReady = true;

	public Text txt;
	public Slider sld;
	public Image img;

	public Text typeText;

	// Use this for initialization
	void Start () {
		switch (myType) {
		case SkillSettings.Buttons.Q:
			disps[0] = this;
			typeText.text = "Q";
			break;
		case SkillSettings.Buttons.W:
			disps[1] = this;
			typeText.text = "W";
			break;
		case SkillSettings.Buttons.E:
			disps[2] = this;
			typeText.text = "E";
			break;
		case SkillSettings.Buttons.R:
			disps[3] = this;
			typeText.text = "R";
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (curCooldown <= 0)
			isReady = true;
		else
			isReady = false;
		curCooldown = Mathf.Clamp (curCooldown, 0, cooldown);

		if (isReady) {
			img.color = new Color (1,1,1);
			sld.enabled = false;
			txt.enabled = false;
		} else {
			img.color = new Color (0.5f,0.5f,0.5f);
			sld.enabled = true;
			txt.enabled = true;

			sld.maxValue = cooldown;
			sld.value = cooldown - curCooldown;

			txt.text = (((int)(curCooldown*10f))/10f).ToString();
		}
	}
}
