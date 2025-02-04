using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "RawTrainingData", menuName = "ScriptableObjects/Raw", order = 1)]
public class RawTrainingData : ScriptableObject
{
    public enum SubShape
    {
        NULL,
        Circle,
        Square,
        Triange
    }
    public SubShape CurrentShape;  
    public List<Vector2> Rawvec2data;
}
