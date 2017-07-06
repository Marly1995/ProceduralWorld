using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTilt : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Transform trans = transform;
        Vector3 newDir = trans.right * -Input.GetAxisRaw("Horizontal") + trans.forward * -Input.GetAxisRaw("Vertical");
        float Angle = 30f;
        Vector3 Axis = Vector3.Cross(newDir, trans.up);
        Quaternion targetRotation = trans.rotation * Quaternion.AngleAxis(Angle, Axis);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
    }
}
