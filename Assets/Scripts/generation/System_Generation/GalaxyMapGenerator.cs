using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyMapGenerator : MonoBehaviour {

    public GameObject planet;
    public GameObject galaxy;

    GameObject newGalaxy;
    public int width;
    public int height;
    public int stepMax;
    public int stepMin;
    public float distance;

    int[,] map;
    List<Vector2> planets;
    int m_step;

    private void Update()
    {
        if (Input.GetKey("a"))
        {
            galaxy = newGalaxy;
            planets = new List<Vector2>();
            m_step = Random.Range(stepMin, stepMax);
            map = new int[width, height];
            GenerateGalaxy();
            DisplayGalaxy();
        }
    }

    private void Start()
    {
        newGalaxy = galaxy;
        planets = new List<Vector2>();
        m_step = Random.Range(stepMin, stepMax);
        map = new int[width, height];
        GenerateGalaxy();
        DisplayGalaxy();
    }

    private void DisplayGalaxy()
    {
        for (int i = 0; i < planets.Count; i++)
        {
            Instantiate(planet, new Vector3(planets[i].x/100, planets[i].y/100, 0), Quaternion.identity, galaxy.transform);
        }
    }
    
    private void GenerateGalaxy()
    {
        for (int x = 0; x < 1000; x++)
        {
            for (int y = 0; y < 1000; y++)
            {
                if (!CheckPlanetCollisions(x, y))
                {
                    m_step--;
                    if (m_step <= 0)
                    {
                        planets.Add(new Vector2(x, y));
                        m_step = Random.Range(100, 1000);
                    }
                }
            }
        }
    }

    private bool CheckPlanetCollisions(int x, int y)
    {
        for (int i = 0; i < planets.Count; i++)
        {
            if (GridDistance(x, planets[i].x, y, planets[i].y) <= distance)
            { return true; }
        }
        return false;
    }

    private float GridDistance(int x1, float x2, int y1, float y2)
    {
        return Mathf.Abs((x1 - x2) * (y1 - y2));
    }
}