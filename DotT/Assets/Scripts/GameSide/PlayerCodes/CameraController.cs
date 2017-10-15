using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	void Update()
    {
		if(Input.mousePosition.x <= 0)
        {
            transform.Translate(-0.5f, 0, 0, Space.World);
        }
        if(Input.mousePosition.x >= Screen.width - 1)
        {
            transform.Translate(0.5f, 0, 0, Space.World);
        }
        if(Input.mousePosition.y <= 0)
        {
            transform.Translate(0, 0, -0.5f, Space.World);
        }
        if(Input.mousePosition.y >= Screen.height - 1)
        {
            transform.Translate(0, 0, 0.5f, Space.World);
        }
    }
}
