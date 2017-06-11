using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotScript : MonoBehaviour {

    public float shotSpeed;
    public int fireRate;

    public int shotTimer;

    public GameObject bullet;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        shotTimer--;
		if (Input.GetButton("Fire1") && shotTimer <= 0)
        {
            shotTimer = fireRate;
            GameObject obj = Instantiate(bullet, transform.position, transform.rotation);
            obj.transform.parent = null;
            obj.GetComponent<Rigidbody>().velocity = (transform.right * shotSpeed);
        }
	}
}
