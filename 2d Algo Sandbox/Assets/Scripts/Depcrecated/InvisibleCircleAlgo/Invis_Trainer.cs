using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Invis_Trainer : MonoBehaviour
{
    RawTrainingData _MasterReferenceShape;
    public List<RawTrainingData> TrainingData;
    List<Vector2> _ChungusData;
    public Cooked_Shape TrainingTarget;
    void Start()
    {
        _MasterReferenceShape = TrainingData[0]; // first shape is the Master Reference Shape
        _ChungusData =new List<Vector2>();
        _ChungusData = Chungusfy();
    }

    void Update()
    {
        
    }
    void TrainData()
    {

    }

    List<Vector2> Chungusfy()
    {
        List<Vector2> output = new List<Vector2>();
        foreach (var dataset in TrainingData)
        {
            output.AddRange(dataset.RawVector2DataPoints);
        }
        return output;
    }

    Vector2 Find_KNN_OneControlPoint(Vector2 refData, List<Vector2> chungusList, int k) // returns std
    {
        List<float> KNN_List = new List<float>();// raw distances from control point to nearest pattern point
        List<KeyValuePair<float, Vector2>> point_and_distance = new List<KeyValuePair<float, Vector2>>();
        List<float> KNN_Vectors_X = new List<float>();
        List<float> KNN_Vectors_Y = new List<float>();

        List<Vector2> output_xySTD = new List<Vector2>();

        foreach (Vector2 rawdata in chungusList)
        {
            float pointdist = Vector2.Distance(rawdata, refData);
            point_and_distance.Add(new KeyValuePair<float, Vector2>(pointdist, rawdata));
        }
        point_and_distance = point_and_distance.OrderBy(pair => pair.Key).ToList();

        for (int i = 0; i < k; i++)
        {
            KNN_Vectors_X.Add(point_and_distance[i].Value.x);
            KNN_Vectors_Y.Add(point_and_distance[i].Value.y);
        }

        float stdx = HELPER_FUNCS.CalculateStdDev(KNN_Vectors_X, refData.x);
        float stdy = HELPER_FUNCS.CalculateStdDev(KNN_Vectors_Y, refData.y);

        return new Vector2(stdx, stdy);
    }

}
