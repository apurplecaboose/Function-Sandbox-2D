using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OCR_Correlation : MonoBehaviour
{
    public DoodleMasterOCR DOODLEMASTERCOMPONENT;
    public List<Vector2> _DoodlePointOutputVector2;


    public List<PatternStorageObject> Patterns;
    public List<string> PatternKeyString;
    public List<float> MatchData_r_squared;

    public float Max_R_squared;
    int _Max_R_squared_IndexNumber;
    void Start()
    {
        foreach (var pattern in Patterns)
        {
            if(pattern.ReferenceShapeData.Count > 100)
            {
                RemoveRandomEntries(pattern.ReferenceShapeData, pattern.ReferenceShapeData.Count - 100);
            }
        }
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Invoke("DoodleNumberSorting", 0); //run next frame so to prevent race condition
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (var pattern in Patterns)
            {
                float r_squaredCorrelationCoefficnent = PatternMatching_CorrelationCoefficient(pattern.ReferenceShapeData, _DoodlePointOutputVector2);
                MatchData_r_squared.Add(r_squaredCorrelationCoefficnent);
            }
            for (int i = 0; i < MatchData_r_squared.Count; i++)
            {
                if (MatchData_r_squared[i] > Max_R_squared)
                {
                    Max_R_squared = MatchData_r_squared[i];
                    _Max_R_squared_IndexNumber = i;
                }
            }
            //MatchData_r_squared.Clear();
            print("Match is probably " + PatternKeyString[_Max_R_squared_IndexNumber] + " with a correlation coefficent of " + Max_R_squared);
            //Max_R_squared = 0;
            //_Max_R_squared_IndexNumber = -1;
        }
    }
    void RemoveRandomEntries(List<Vector2> inputlist, int numbertoremove)
    {
        for(int i = 0;i < numbertoremove;i++)
        {
            inputlist.RemoveAt(Random.Range(0, inputlist.Count - 1));
        }
    }
    public float PatternMatching_CorrelationCoefficient(List<Vector2> inputList1, List<Vector2> inputList2)
    {
        float R_Square(List<float> x1, List<float> x2)
        {
            int n = x1.Count;
            float sum_x1 = 0, sum_x2 = 0, sum_x1x2 = 0, squareSum_x1 = 0, squareSum_x2 = 0;

            for (int i = 0; i < n; i++)
            {
                sum_x1 = sum_x1 + x1[i];// sum of elements of list X.
                sum_x2 = sum_x2 + x2[i];// sum of elements of list Y.
                sum_x1x2 = sum_x1x2 + x1[i] * x2[i];// sum of X[i] * Y[i].
                                                    // sum of square of list elements using Mathf.Pow.
                squareSum_x1 = squareSum_x1 + Mathf.Pow(x1[i], 2);
                squareSum_x2 = squareSum_x2 + Mathf.Pow(x2[i], 2);
            }
            // use formula for calculating correlation coefficient
            float corr = (float)(n * sum_x1x2 - sum_x1 * sum_x2) / (float)(Mathf.Sqrt((n * squareSum_x1 - sum_x1 * sum_x1) * (n * squareSum_x2 - sum_x2 * sum_x2)));
            return Mathf.Abs(corr);// ensure the correlation coefficient is always positive.
        }

        //sort by x then sort by y
        List<Vector2> sortedList1 = inputList1.OrderBy(v => v.x).ThenBy(v => v.y).ToList();
        List<Vector2> sortedList2 = inputList2.OrderBy(v => v.x).ThenBy(v => v.y).ToList();

        List<float> x1_list = new List<float>();
        List<float> x2_list = new List<float>();
        List<float> y1_list = new List<float>();
        List<float> y2_list = new List<float>();

        foreach (Vector2 v in sortedList1)
        {
            x1_list.Add(v.x);
            y1_list.Add(v.y);
        }
        foreach (Vector2 v in sortedList2)
        {
            x2_list.Add(v.x);
            y2_list.Add(v.y);
        }
        float x_cor = R_Square(x1_list, x2_list);
        float y_cor = R_Square(y1_list, y2_list);
        return x_cor * y_cor / 2;
    }
    void DoodleNumberSorting()
    {
        foreach (Vector3 datapoint in DOODLEMASTERCOMPONENT.DrawingOutput)
        {
            if (datapoint.z == 0)
            {
                _DoodlePointOutputVector2.Add(new Vector2(datapoint.x, datapoint.y));
            }
        }
        if(_DoodlePointOutputVector2.Count > 100)
        {
            RemoveRandomEntries(_DoodlePointOutputVector2, _DoodlePointOutputVector2.Count - 100);
        }
    }
}
