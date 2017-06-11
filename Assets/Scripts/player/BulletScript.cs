using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public float life = 6;

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, life);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
