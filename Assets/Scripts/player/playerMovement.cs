﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour {

    public float acceleration;
    public float decelaration;
    public float maxSpeed;
    float currentSpeed;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    Rigidbody rb;
        
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (moveDir.magnitude >= 0.2)
        {
            currentSpeed = currentSpeed >= maxSpeed ? currentSpeed : currentSpeed + acceleration;
        }
        else
        {
            currentSpeed = currentSpeed <= 0 ? currentSpeed : 0 - decelaration;
        }
        Vector3 targetMoveAmount = moveDir * currentSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
}
