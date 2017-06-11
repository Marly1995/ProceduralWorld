using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinner : MonoBehaviour {

    public Camera cam;
    public float rotationSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //transform.Rotate(0.03f, 0.05f, 0.02f, Space.Self);
    }

    private void OnMouseDrag()
    {
        float rotx = Input.GetAxis("Mouse X") * rotationSpeed * Mathf.Deg2Rad;
        float roty = Input.GetAxis("Mouse Y") * rotationSpeed * Mathf.Deg2Rad;

        //transform.Rotate(Vector3.up, -rotx);
        //transform.Rotate(Vector3.right, roty);
        GetComponent<Rigidbody>().AddTorque(Vector3.up * -rotx);
        GetComponent<Rigidbody>().AddTorque(Vector3.right * roty);
    }
}
