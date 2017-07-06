using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotScript : MonoBehaviour {

    public float shotSpeed;
    public int fireRate;

    public int shotTimer;

    public GunModule gun;

    public GameObject bullet;

    public Transform bulletSpawnLocation;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (gun != null)
        { 
            shotTimer--;
            if (/*Input.GetButton("Fire1") && */shotTimer <= 0)
            {
                shotTimer = gun.fireRate;
                for (int i = 0; i < gun.shotCount; i++)
                {
                    GameObject obj = Instantiate(bullet, bulletSpawnLocation.position, transform.rotation);
                    obj.transform.Rotate(transform.parent.transform.parent.transform.up, Random.Range(-gun.accuracy, gun.accuracy), Space.World);
                    obj.GetComponent<Rigidbody>().velocity = (-obj.transform.forward * Random.Range(gun.shotSpeed - 2, gun.shotSpeed + 2));
                }
            }
        }
	}
}
