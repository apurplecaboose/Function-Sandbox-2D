using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class TEST : MonoBehaviour
{
    public A_DoodleMaster DOODLEMASTERCOMPONENT;
    [SerializeField] List<Vector2> _InputData;
    [SerializeField] List<Vector2> _criticalPoints;
    [SerializeField] List<float> _criticalPointsAngles;
    void OnDrawGizmos()
    {
        foreach (Vector2 vec2point in _InputData)
        {
            Gizmos.DrawSphere(new Vector3(vec2point.x, vec2point.y, 0), 0.25f);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Invoke("DoodleNumberSorting", 0); //run next frame so to prevent race condition
        }
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
           _InputData = PruneAndTrim(_InputData, 10);
            Invoke("PrintCriticalPoints", 0);
        }
    }
    void PrintCriticalPoints()
    {
        if (_InputData.Count < 3) return;
        List<KeyValuePair<Vector2, float>> point_angle = new List<KeyValuePair<Vector2, float>>();
        for (int i = 0; i < _InputData.Count; i++)
        {
            Vector2 a = Vector2.zero;
            Vector2 b = Vector2.zero;
            Vector2 c = Vector2.zero;
            if (i < _InputData.Count - 2)
            {
                a = _InputData[i];
                b = _InputData[i + 1];
                c = _InputData[i + 2];
            }
            else if (i == _InputData.Count - 2)
            {
                a = _InputData[i];
                b = _InputData[i + 1];
                c = _InputData[0];
            }
            else
            {
                a = _InputData[i];
                b = _InputData[0];
                c = _InputData[1];
            }


            float angle = CalculateSignedAngle(a, b, c);
            point_angle.Add(new KeyValuePair<Vector2, float>(b, angle));
        }
       // point_angle = point_angle.OrderBy(f => f.Value).ToList();
        foreach (var pair in point_angle)
        {
            _criticalPoints.Add(pair.Key);
            _criticalPointsAngles.Add(pair.Value);
        }
    }
    List<Vector2> PruneAndTrim(List<Vector2> inputdata, float thresholdangle)
    {
        if(inputdata.Count <= 3) return inputdata;
        int previousCount;
        do
        {
            previousCount = inputdata.Count;
            List<Vector2> uselessanglepoints = new List<Vector2>();
            for (int i = 0; i < inputdata.Count - 3; i++)
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
            print("one pass");
        } while (inputdata.Count < previousCount);

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
            if(dis < 2)
            {
                uselessdistancepoints.Add(a);
            }
        }
        foreach (Vector2 garbage in uselessdistancepoints)
        {
            inputdata.Remove(garbage);
        }
        return inputdata;
    }
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
        //else if (angleInDegrees > 180 - anglethreshold) return true;
        else return false;
    }
    float CalculateSignedAngle(Vector2 A, Vector2 B, Vector2 C)
    {
        Vector2 AB = B - A;
        Vector2 BC = C - B;

        AB.Normalize(); 
        BC.Normalize();

        float dotProduct = Vector2.Dot(AB, BC);
        float angleInRadians = Mathf.Acos(dotProduct);
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
        angleInDegrees = 180 - angleInDegrees;
        Vector3 crossProduct = Vector3.Cross(AB, BC);
        if (crossProduct.z > 0)
        {
            angleInDegrees = -angleInDegrees;
        }

        return angleInDegrees;
    }



    void DoodleNumberSorting()
    {
        foreach (Vector3 datapoint in DOODLEMASTERCOMPONENT.DrawingOutput)
        {
            if (datapoint.z == 0)
            {
                _InputData.Add(new Vector2(datapoint.x, datapoint.y));
            }
        }
    }
}
