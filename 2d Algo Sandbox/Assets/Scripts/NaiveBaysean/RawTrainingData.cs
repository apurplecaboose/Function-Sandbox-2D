using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "RawTrainingData", menuName = "ScriptableObjects/Raw_Data", order = 1)]
public class RawTrainingData : ScriptableObject
{
    //public enum SubShape
    //{
    //    NULL,
    //    Circle,
    //    Square,
    //    Triangle,
    //    Fish,
    //    Z,
    //    Star
    //}
    //public SubShape CurrentShape;  
    public List<Vector2> RawVector2DataPoints;
}
