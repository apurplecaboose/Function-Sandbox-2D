using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReferenceShape", menuName = "ScriptableObjects/New Reference Shape", order = 1)]
public class PatternStorageObject :  ScriptableObject
{
    public bool IsOpenShape;
    public string Pattern_Name;
    public List<Vector2> ReferenceShapeData;
    //required pattern accuracy 0-100 see how easy this pattern is and how closely it should required to be matched for it to return
}
