using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityObject : MonoBehaviour
{

    universalGravityAttraction planet;
    public float gravity = -6f;

    private void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<universalGravityAttraction>();
        GetComponent<Rigidbody>().useGravity = false;
    }

    private void FixedUpdate()
    {
        planet.AttractObject(transform, gravity);
    }
}
