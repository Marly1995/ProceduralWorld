using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTilt : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Transform trans = transform.parent;
        Vector3 newDir = -(Vector3.right * 2) * Input.GetAxisRaw("Horizontal") + -Vector3.forward * Input.GetAxisRaw("Vertical");
        float Angle = 15f;
        Vector3 Axis = Vector3.Cross(newDir, Vector3.up);
        Quaternion targetRotation = trans.rotation * Quaternion.AngleAxis(Angle, Axis);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);
    }
}
