using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float speed = 0.2f;

	void Update()
    {
		if(Input.mousePosition.x <= 0)
        {
            transform.Translate(-speed, 0, 0, Space.World);
        }
        if(Input.mousePosition.x >= Screen.width - 1)
        {
            transform.Translate(speed, 0, 0, Space.World);
        }
        if(Input.mousePosition.y <= 0)
        {
            transform.Translate(0, 0, -speed, Space.World);
        }
        if(Input.mousePosition.y >= Screen.height - 1)
        {
            transform.Translate(0, 0, speed, Space.World);
        }
    }
}
