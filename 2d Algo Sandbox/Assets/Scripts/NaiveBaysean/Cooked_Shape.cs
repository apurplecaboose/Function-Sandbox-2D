using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CookedTrainingData", menuName = "ScriptableObjects/Cooked Data", order = 1)]
public class Cooked_Shape : ScriptableObject
{
    public enum ShapeToMatch
    {
        NULL,
        Circle,
        Square,
        Triangle
    }
    //identifiers
    [Header("Shape Type")]
    public ShapeToMatch CurrentShape;

    [Header("Processed Data DEBUG")]
    //processed data
    [Header("Mean")]
    public float mean_top;
    public float mean_bottom, mean_left, mean_right, mean_middle;
    public List<float> meanPoints;
    [Header("STD")]
    public float std_top;
    public float std_bottom, std_left, std_right, std_middle;
    public List<float> STD_Points;

}
