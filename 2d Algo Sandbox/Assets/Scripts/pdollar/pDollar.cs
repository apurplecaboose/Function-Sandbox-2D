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
    public float ThresholdCost = 1000000;
    int _ExpectedArraySize = 50;

    List<Vector2> _currentPatternData;
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
    void MatchShapes()
    {
        if (_InputData.Count < _ExpectedArraySize)
        {
            print("Null Pattern not enough data this is probably a point");
            //null pattern
            return;
        }
        List<float> alignmentcosts = new List<float>();
        foreach (var shape in RefShapes)
        {
            shape.CurrentAlignCost = 10000000000000000; // reset old data
            List<float> variations_alignmentcost = new List<float>();
            foreach (var variation in shape.RawData)
            {
                _currentPatternData.Clear();
                _currentPatternData.AddRange(variation.RawVector2DataPoints);

                //List<Vector2> currentPatternCopy = variation.RawVector2DataPoints;

                float var_alignmentcost = 0;
                foreach (Vector2 in_v in _InputData)
                {
                    var output = NearestNeighbor_OnePoint(in_v, _currentPatternData);
                    _currentPatternData.Remove(output.Item1); // remove the pair from the list
                    //var output = NearestNeighbor_OnePoint(in_v, currentPatternCopy);
                    //currentPatternCopy.Remove(output.Item1); // remove the pair from the list
                    var_alignmentcost += output.Item2;
                }
                variations_alignmentcost.Add(var_alignmentcost);
            }
            float alignCost = variations_alignmentcost.Min();
            shape.CurrentAlignCost = alignCost;
            alignmentcosts.Add(alignCost);
        }
        float lowestcost = alignmentcosts.Min();
        if (lowestcost > ThresholdCost)
        {
            print("Null Pattern u prob drew garbage. Align cost:    " + lowestcost);
            return;
            //return null pattern error
        }
        foreach (var shape in RefShapes)
        {
            if (lowestcost == shape.CurrentAlignCost)
            {
                print("Pattern: " + shape.CurrentShape.ToString() + "   Match Cost: " + lowestcost);
                return;
            }
        }
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

