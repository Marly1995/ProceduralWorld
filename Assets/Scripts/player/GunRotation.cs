using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void Update()
    {
        Vector3 newDir = transform.parent.transform.right * -Input.GetAxisRaw("hRight") + transform.parent.transform.forward * Input.GetAxisRaw("vRight");
        if (newDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(newDir, transform.up);
        }
    }
}
