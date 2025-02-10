using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class DoodleOCR : MonoBehaviour
{
    public DoodleMasterOCR parentDoodler;

    LineRenderer _LineRend;
    List<Vector2> _PointsListRaw;

    float _MinPointDistance = 0.2f, // do not go below 0.2f
          _MaxLineLength = 50, _LineLength,
          _targetPathLength = 50;

    Vector3 NewCenterPoint;
    void OnDrawGizmos()
    {
        foreach (Vector2 vec2point in _PointsListRaw) 
            {
            Gizmos.DrawSphere(new Vector3(vec2point.x, vec2point.y, 0), 0.51f);
            }
    }
    void Awake()
    {
        _LineRend = this.GetComponent<LineRenderer>();
    }
    public void DEV_PatternCreationDoodleMode(float interpointdistance, float maxlinelength)
    {
        _MinPointDistance = interpointdistance;
        _MaxLineLength = maxlinelength;
    }
    void Update()
    {
        IsLineTooLong();
    }
    public void UpdateLine(Vector2 inputmouseposition)
    {
        if (_PointsListRaw == null)
        {
            _PointsListRaw = new List<Vector2>(); //initialize list
            SetPoint(inputmouseposition);// set the first position
            return;
        }
        Vector2 lastrecordedpoint = _PointsListRaw[_PointsListRaw.Count - 1];
        float pointsDistance = Vector2.Distance(lastrecordedpoint, inputmouseposition);
        if (pointsDistance < _MinPointDistance) return; //less than min threshold do not need to record

        Vector2 unitvector = inputmouseposition - lastrecordedpoint;
        unitvector = unitvector.normalized;

        if (pointsDistance >= 2 * _MinPointDistance)
        {
            int interpolationPointsCount = Mathf.FloorToInt(pointsDistance / _MinPointDistance);
            for (int i = 1; i <= interpolationPointsCount; i++)
            {
                SetPoint(lastrecordedpoint + unitvector * _MinPointDistance * i); //underdraws i(interpolationPointsCount) amount of times
            }
            return;
        }
        if (pointsDistance > _MinPointDistance)
        {
            SetPoint(lastrecordedpoint + unitvector * _MinPointDistance); //underdraws
            return;
        }
        if (pointsDistance == _MinPointDistance) //this case will almost never happen
        {
            SetPoint(inputmouseposition);
            return;
        }
        else return;// this case should NEVER happen :)
    } 
    void SetPoint(Vector2 point)
    {
        _PointsListRaw.Add(point);
        _LineRend.positionCount = _PointsListRaw.Count;
        _LineRend.SetPosition(_PointsListRaw.Count - 1, point);
    }
    void IsLineTooLong()
    {
        if (_LineLength < 0) return;
        _LineLength = (_PointsListRaw.Count - 1) * _MinPointDistance;
        if (_LineLength >= _MaxLineLength)
        {
            parentDoodler.EndDoodlin();
            _LineLength = -_LineLength;
            Debug.Log("Watch it! Line is too Loooooong it was cut short");
        }
    }
    public List<Vector3> PrintPointOutput(int doodleNumber)
    {
        ///center shape and rescale
        if(_LineLength > 0) _LineLength = (_PointsListRaw.Count - 1) * _MinPointDistance;//find line length;

        ////find center
        //float min_X = _PointsListRaw.Min((v => v.x));
        //float max_X = _PointsListRaw.Max((v => v.x));
        //float min_Y = _PointsListRaw.Min((v => v.y));
        //float max_Y = _PointsListRaw.Max((v => v.y));
        //float xcenter = (max_X + min_X) / 2;
        //float ycenter = (max_Y + min_Y) / 2;
        //Vector2 AABB_center = new Vector2(xcenter, ycenter);

        Vector2 AABB_center = CalculateCentroid(_PointsListRaw);
        float scalingfactor = _targetPathLength / Mathf.Abs(_LineLength);//scale according to line length
        _PointsListRaw = PruneAndTrim(_PointsListRaw, 5f, _MinPointDistance * 1.5f);
        for (int i = 0; i < _PointsListRaw.Count; i++)
        {
            _PointsListRaw[i] = _PointsListRaw[i] - AABB_center;
            _PointsListRaw[i] *= scalingfactor;
        }

        List<Vector3> returnList = new List<Vector3>();
        foreach (Vector2 drawpoint in  _PointsListRaw)
        {
            returnList.Add(new Vector3(drawpoint.x, drawpoint.y, doodleNumber));
        }
        return returnList;  
    }





    Vector2 CalculateCentroid(List<Vector2> points)
    {
        Vector2 sum = Vector2.zero;
        foreach (Vector2 point in points)
        {
            sum += point;
        }
        return sum / points.Count;
    }
    List<Vector2> PruneAndTrim(List<Vector2> inputdata, float thresholdangle, float pointdistanceThreshold)
    {
        /// <summary>
        /// returns true if useless, false if useful
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="anglethreshold"></param>
        /// <returns></returns>
        bool UselessPointCheck(Vector2 A, Vector2 B, Vector2 C, float anglethreshold)
        {
            Vector2 AB = B - A;
            Vector2 BC = C - B;

            AB.Normalize();
            BC.Normalize();

            float dotProduct = Vector2.Dot(AB, BC);
            float angleInDegrees = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

            if (angleInDegrees < anglethreshold) return true;
            else return false;
        }

        if (inputdata.Count <= 3) return inputdata;
        int previousCount;
        do
        {
            previousCount = inputdata.Count;
            List<Vector2> uselessanglepoints = new List<Vector2>();
            for (int i = 0; i < inputdata.Count - 2; i++)
            {
                Vector2 a = inputdata[i];
                Vector2 b = inputdata[i + 1];
                Vector2 c = inputdata[i + 2];
                if (UselessPointCheck(a, b, c, thresholdangle))
                {
                    uselessanglepoints.Add(b);
                }
            }
            foreach (Vector2 garbage in uselessanglepoints)
            {
                inputdata.Remove(garbage);
            }
            print("one angle pass");
        } while (inputdata.Count < previousCount);
        do
        {
            previousCount = inputdata.Count;
            List<Vector2> uselessdistancepoints = new List<Vector2>();
            for (int i = 0; i < inputdata.Count; i++)
            {
                Vector2 a = inputdata[i];
                Vector2 b = inputdata[0];
                if (i != inputdata.Count - 1)
                {
                    b = inputdata[i + 1];
                }

                float dis = Vector2.Distance(a, b);
                if (dis < pointdistanceThreshold)
                {
                    uselessdistancepoints.Add(b);
                }
            }
            foreach (Vector2 garbage in uselessdistancepoints)
            {
                inputdata.Remove(garbage);
            }
            print("one dis pass");
        } while (inputdata.Count < previousCount);

        return inputdata;
    }




    //for visuals
    public void SetLineColor(Color targetColor)
    {
        _LineRend.colorGradient = SingleColorToGradient(targetColor);
    }
    Gradient SingleColorToGradient(Color color)
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(color, 0.0f);
        colorKeys[1] = new GradientColorKey(color, 1.0f);
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(color.a, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(color.a, 1.0f);
        gradient.SetKeys(colorKeys, alphaKeys);
        return gradient;
    }



}
