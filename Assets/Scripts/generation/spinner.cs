using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinner : MonoBehaviour {

    public Camera cam;
    public float rotationSpeed;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0.03f * rotationSpeed, 0.05f * rotationSpeed, 0f, Space.Self);
    }
}
