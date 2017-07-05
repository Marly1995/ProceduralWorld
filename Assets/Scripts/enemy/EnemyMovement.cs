using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    Transform player;
    public float speed = 2;
    public float health = 4;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.rotation = Quaternion.LookRotation(player.position - transform.position, transform.up);
        if (health <= 0)
        {
            Destroy(transform.gameObject);
        }
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.forward * speed * Time.fixedDeltaTime);
    }
}
