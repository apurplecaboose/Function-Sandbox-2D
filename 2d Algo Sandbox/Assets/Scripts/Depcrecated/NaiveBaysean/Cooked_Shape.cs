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
        Triangle,
        Fish,
        Z,
        Star
    }
    //[Range(0.0001f,3)]
    //public float DifficultyMultiplier = 1;// values less than 1 are difficult shapes; values higher than 1 are easy shapes or overrepresented shapes
    
    //identifiers
    [Header("Shape Type")]
    public ShapeToMatch CurrentShape;

    [Header("Processed Data DEBUG")]
    //processed data
    [Header("Mean")]
    public List<float> Mean_Point_Weights;
    [Header("STD")]
    public List<float> STD_Point_Weights;

}
