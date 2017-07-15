using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour {

    Rigidbody rb;
    bool stop = false;
    int stoptime;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
            stoptime++;
        if (stoptime >= 120)
        {
            stop = true;
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (stop)
        {
            rb.isKinematic = true;
            Destroy(GetComponent<gravityBody>());
        }
    }
}
