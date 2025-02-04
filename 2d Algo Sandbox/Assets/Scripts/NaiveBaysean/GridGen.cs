using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGen : MonoBehaviour
{
    public List<Vector2> testlist = new List<Vector2>();
    // Start is called before the first frame update
    void Start()
    {
        testlist = GenerateGridPoints(9);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public List<Vector2> GenerateGridPoints(int gridsize)
    {
        List<Vector2> keyPoints = new List<Vector2>();
        float step = 2.0f / (gridsize - 1); // Calculate the step size based on the range and the number of points
        for (int y = 0; y < gridsize; y++)
        {
            for (int x = 0; x < gridsize; x++)
            {
                keyPoints.Add(new Vector2(-1 + x * step, 1 - y * step));
            }
        }

        return keyPoints;
    }
}

