using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class universalGravityAttraction : MonoBehaviour {

    public void Attract(Transform body, float gravity)
    {
        Vector3 targetDir = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;
        body.GetComponent<Rigidbody>().AddForce(targetDir * gravity);

    }
}
