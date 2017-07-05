using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public float life = 6;
    public float damage = 1;
    public gravityBody gb;
    public Rigidbody rb;

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, life);
        gb.gravity = -1.5f * rb.velocity.magnitude;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "bullet")
        {
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag == "enemy")
        {
            collision.gameObject.GetComponent<EnemyMovement>().health -= damage;
        }
    }
}
