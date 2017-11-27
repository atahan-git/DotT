using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static CameraController s;

	public bool toggle = true;
	public Vector3 cameraOffset = new Vector3(0,5,5);

	float area = 100;

	void Awake(){
		s = this;
	}

	void Update()
    {
		if (toggle) {
			Rect screenRect = new Rect (0, 0, Screen.width, Screen.height);
			//if (screenRect.Contains (Input.mousePosition)) {
			
				if (Input.mousePosition.x <= area || Input.GetKey (KeyCode.A)) {
					transform.Translate (-0.5f, 0, 0, Space.World);
				}
				if (Input.mousePosition.x >= Screen.width - area || Input.GetKey (KeyCode.D)) {
					transform.Translate (0.5f, 0, 0, Space.World);
				}
				if (Input.mousePosition.y <= area || Input.GetKey (KeyCode.S)) {
					transform.Translate (0, 0, -0.5f, Space.World);
				}
				if (Input.mousePosition.y >= Screen.height - area || Input.GetKey (KeyCode.W)) {
					transform.Translate (0, 0, 0.5f, Space.World);
				}
			//}

		} else {
			if (Input.GetKey (KeyCode.A)) {
				transform.Translate (-0.5f, 0, 0, Space.World);
			}
			if (Input.GetKey (KeyCode.D)) {
				transform.Translate (0.5f, 0, 0, Space.World);
			}
			if (Input.GetKey (KeyCode.S)) {
				transform.Translate (0, 0, -0.5f, Space.World);
			}
			if (Input.GetKey (KeyCode.W)) {
				transform.Translate (0, 0, 0.5f, Space.World);
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape))
			toggle = !toggle;
    }

	public void SetPos (Vector3 pos){
		transform.position = pos + cameraOffset;
		print ("Camera Position Set " + pos.ToString());
	}
}
