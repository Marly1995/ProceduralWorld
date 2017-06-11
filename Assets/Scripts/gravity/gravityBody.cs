using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class gravityBody : MonoBehaviour {

    universalGravityAttraction planet;
    public float gravity = -6f;

    private void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<universalGravityAttraction>();
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        planet.Attract(transform, gravity);
    }
}
