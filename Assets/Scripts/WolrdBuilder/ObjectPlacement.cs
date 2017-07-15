using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour {

    public GameObject obj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("b"))
        {
            GameObject op = Instantiate(obj, transform);
            op.AddComponent<GravityObject>();
            op.transform.parent = null;
        }
	}
}
