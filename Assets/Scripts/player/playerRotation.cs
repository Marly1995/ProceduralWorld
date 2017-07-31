using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 newDir = transform.parent.transform.right * Input.GetAxisRaw("hRight") + -transform.parent.transform.forward * Input.GetAxisRaw("vRight");
        if (newDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newDir, transform.up), 0.075f);
        }
    }
}
