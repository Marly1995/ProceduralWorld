using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalPopulation : MonoBehaviour
{
    public GameObject temp;

    public float latitude;
    public float longitude;

    public float radius;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PlaceObject();
        }
    }

    void PlaceObject()
    {
        Vector3 position = ProjectToSurface();
        Instantiate(temp, position, Quaternion.identity);
    }

    Vector3 ProjectToSurface()
    {
        float x = radius * Mathf.Cos(latitude) * Mathf.Cos(longitude);
        float y = radius * Mathf.Cos(latitude) * Mathf.Sin(longitude);
        float z = radius * Mathf.Sin(latitude);

        Ray ray = new Ray(new Vector3(x, y, z) * 100f, new Vector3(x, y, z) * -100f);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}