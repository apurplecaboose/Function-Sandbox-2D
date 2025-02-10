using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "RawShapes", menuName = "ScriptableObjects/RawShapes", order = 1)]
public class RawShapes : ScriptableObject
{
    public List<Vector2> RawVector2DataPoints;
}
