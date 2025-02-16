using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Diagnostics;

public class pDollar : MonoBehaviour
{
    Stopwatch _himark;
    [Header("ATTACH Data here")]
    public List<ShapeGroup> RefShapes;
    DoodleMasterOCR _DOODLEMASTERCOMPONENT;

    [Header("Tweak values")]
    int _ExpectedArraySize = 50;

    List<Vector2> _currentPatternData; // the local varible version is not working.... so use private var here
    List<Vector2> _InputData;
    void Start()
    {
        _DOODLEMASTERCOMPONENT = this.GetComponent<DoodleMasterOCR>();
        _himark = new Stopwatch();
        _currentPatternData = new List<Vector2>();
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Invoke("DoodleNumberSorting", 0); //run next frame so to prevent race condition
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _himark.Start();
            MatchShapes();
            _himark.Stop();
            long elapsedMilliseconds = _himark.ElapsedMilliseconds;
            UnityEngine.Debug.Log("Done Matching! Time in ms:   " + elapsedMilliseconds);
        }
    }
    ShapeGroup.SubShape MatchShapes()
    {
        if (_InputData.Count < _ExpectedArraySize)
        {
            print("Null Pattern not enough data this is probably a point");
            //null pattern
            return ShapeGroup.SubShape.POINT;
        }
        float min_overall_aligmentcost = Mathf.Infinity;//set the lowest cost to absurdly high value
        float thresholdCost = Mathf.Infinity;
        ShapeGroup.SubShape lowestcostShapeEnum = ShapeGroup.SubShape.NULL; // initalize local var
        foreach (var shape in RefShapes)
        {
            //shape.CurrentAlignCost = Mathf.Infinity; //currently only writing to scriptable align cost never reading value may not be nessicary;
            float min_var_alignmentcost = Mathf.Infinity;//set the lowest cost to absurdly high value
            foreach (var variation in shape.RawData)
            {
                _currentPatternData.Clear();
                _currentPatternData.AddRange(variation.RawVector2DataPoints);

                float var_alignmentcost = 0;
                foreach (Vector2 in_v in _InputData)
                {
                    if (var_alignmentcost > min_overall_aligmentcost) break; // if larger than the current overall smallest;break
                    if (var_alignmentcost > min_var_alignmentcost) break; //if larger than the other variations; break
                    var output = NearestNeighbor_OnePoint(in_v, _currentPatternData);                 
                    _currentPatternData.Remove(output.Item1); // remove the pair from the list from points that need to be checked
                    var_alignmentcost += output.Item2;
                }
                if(var_alignmentcost < min_overall_aligmentcost) min_var_alignmentcost = var_alignmentcost;//find the min without using list.min    if less than current min, then it is the new min
            }
            float alignCost = min_var_alignmentcost;
            //shape.CurrentAlignCost = alignCost;
            if (alignCost < min_overall_aligmentcost)//find the min without using list.min; also cache the enum
            {
                min_overall_aligmentcost = alignCost;
                lowestcostShapeEnum = shape.CurrentShape;
                thresholdCost = shape.ShapeAlignCost_Threshold;
            }
        }
        if (min_overall_aligmentcost > thresholdCost)
        {
            print("Null Pattern u prob drew garbage. Align cost:    " + min_overall_aligmentcost);
            return ShapeGroup.SubShape.NULL;
        }
        print("Pattern: " + lowestcostShapeEnum.ToString() + "   Match Cost: " + min_overall_aligmentcost);
        return lowestcostShapeEnum;
    }
    (Vector2, float) NearestNeighbor_OnePoint(Vector2 inputPoint, List<Vector2> inputMatchList)
    {
        KeyValuePair<Vector2, float> nearestpoint = new KeyValuePair<Vector2, float>();
        float currentSmallesDistance = 100000000000;
        foreach (Vector2 p_point in inputMatchList)
        {
            float pointdist = Vector2.Distance(inputPoint, p_point);
            if (pointdist < currentSmallesDistance)
            {
                currentSmallesDistance = pointdist;
                nearestpoint = new KeyValuePair<Vector2, float>(p_point, pointdist);
            }
        }
        return (nearestpoint.Key, nearestpoint.Value);
    }

    void DoodleNumberSorting()
    {
        _InputData = new List<Vector2>();
        foreach (Vector3 datapoint in _DOODLEMASTERCOMPONENT.DrawingOutput)
        {
            if (datapoint.z == 0)
            {
                _InputData.Add(new Vector2(datapoint.x, datapoint.y));
            }
        }
    }
}

