using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class DoodleOCR : MonoBehaviour
{
    LineRenderer _LineRend;
    List<Vector2> _PointsListRaw;
    float _MinPointDistance = 0.1f; // do not go below 0.2f
    float _LineLength;
    void OnDrawGizmos()
    {
        foreach (Vector2 vec2point in _PointsListRaw) 
            {
            Gizmos.DrawSphere(new Vector3(vec2point.x, vec2point.y, 0), 0.1f);
            }
    }
    void Awake()
    {
        _LineRend = this.GetComponent<LineRenderer>();
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
    public bool IsLineTooLong(float maxallowablelength)
    {
        _LineLength = _PointsListRaw.Count - 1 * _MinPointDistance;
        if (_LineLength >= maxallowablelength)
        {
            Debug.Log("Watch it! Line is too Loooooong it was cut short");
            return true;
        }
        else return false;
    }
    void SetPoint(Vector2 point)
    {
        _PointsListRaw.Add(point);
        _LineRend.positionCount = _PointsListRaw.Count;
        _LineRend.SetPosition(_PointsListRaw.Count - 1, point);
    }
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
    public List<Vector3> PrintPointOutput(int doodleNumber)
    {
        List<Vector3> returnList = new List<Vector3>();
        foreach (Vector2 drawpoint in  _PointsListRaw)
        {
            returnList.Add(new Vector3(drawpoint.x, drawpoint.y, doodleNumber));
        }
        return returnList;  
    }
}
