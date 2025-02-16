using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Universal_Functions
{
    public static float ABS_Z_Score(float observed, float mean, float std)
    {
        float output = Mathf.Abs((observed - mean) / std);   
        return output;
    }
    
    public static float CalculateStdDev(List<float> data)
    {
        // Calculate mean
        float mean = data.Average();

        // Calculate standard deviation
        float sumOfSquaresOfDifferences = data.Select(val => (val - mean) * (val - mean)).Sum();
        float stdDev = Mathf.Sqrt(sumOfSquaresOfDifferences / data.Count);

        return stdDev;
    }
    public static float CalculateStdDev(List<float> data, float mean)
    {
        // Calculate standard deviation
        float sumOfSquaresOfDifferences = data.Select(val => (val - mean) * (val - mean)).Sum();
        float stdDev = Mathf.Sqrt(sumOfSquaresOfDifferences / data.Count);

        return stdDev;
    }

    /// <summary>
    /// Generates a even grid with size (x,x) with range (-1,1) then scales it by a scalar
    /// </summary>
    /// <param name="gridsize"></param>
    /// <param name="scalar"></param>
    /// <returns></returns>
    public static (List<Vector2>, float) GenerateThenScaleGridPoints(int gridsize, float scalar)
    {
        List<Vector2> keyPoints = new List<Vector2>();
        float gridStepSize = 2.0f / (gridsize - 1); // Calculate the step size based on the range and the number of points
        for (int y = 0; y < gridsize; y++)
        {
            for (int x = 0; x < gridsize; x++)
            {
                keyPoints.Add(new Vector2(-1 + x * gridStepSize, 1 - y * gridStepSize));
            }
        }
        for (int i = 0; i < keyPoints.Count; i++)
        {
            keyPoints[i] = new Vector2(keyPoints[i].x * scalar, keyPoints[i].y * scalar);
        }
        gridStepSize *= scalar;

        return (keyPoints, gridStepSize);
    }
    /// <summary>
    /// Generates a even grid with size (x,x) with range (-1,1)
    /// </summary>
    /// <param name="gridsize"></param>
    /// <returns></returns>
    public static List<Vector2> GenerateGridPoints(int gridsize)
    {
        List<Vector2> keyPoints = new List<Vector2>();
        float gridStepSize = 2.0f / (gridsize - 1); // Calculate the step size based on the range and the number of points
        for (int y = 0; y < gridsize; y++)
        {
            for (int x = 0; x < gridsize; x++)
            {
                keyPoints.Add(new Vector2(-1 + x * gridStepSize, 1 - y * gridStepSize));
            }
        }
        return keyPoints;
    }

#if UNITY_EDITOR
    public static void LetsGetDirty(UnityEngine.Object scriptableObject)
    {
        UnityEditor.EditorUtility.SetDirty(scriptableObject);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
