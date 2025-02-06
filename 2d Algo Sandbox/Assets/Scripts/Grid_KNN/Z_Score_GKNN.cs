using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Z_Score_GKNN : MonoBehaviour
{
    public DoodleMasterOCR DOODLEMASTERCOMPONENT;

    Stopwatch _benchmark;//FOR TESTING ONLY\


    [SerializeField] float _Keypointscale = 30000;
    int _gridsize = 11;
    float _GridStepSize;
    List<Vector2> _KeyGridControlPoints;

    [Header("DEBUG")] //output for check
    [SerializeField] List<Vector2> _InputData;
    [SerializeField] List<float> _ObservedMeanWeights;
    [Header("INPUT")]
    public List<Cooked_Shape> CookedShapes;

    void Awake()
    {
        _benchmark = new Stopwatch();
        _KeyGridControlPoints = new List<Vector2>();
    }
    void Start()
    {
        var grid_and_step = HELPER_FUNCS.GenerateThenScaleGridPoints(_gridsize, _Keypointscale);
        _KeyGridControlPoints = grid_and_step.Item1;
        _GridStepSize = grid_and_step.Item2;

        _ObservedMeanWeights = new List<float>();
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Invoke("DoodleNumberSorting", 0); //run next frame so to prevent race condition
        }
        if(Input.GetKeyUp(KeyCode.C))
        {
            CompareGridWeightsWithCooked();
        }
    }

    public void CompareGridWeightsWithCooked() // aka get ur z scores here bozo
    {
        Console.Clear();

        UnityEngine.Debug.Log("Start Checking");
        _benchmark.Start();

        List<KeyValuePair<float, string>> outputKeyValue = new List<KeyValuePair<float, string>>();
        List<float> zscoreList = new List<float>();
        _ObservedMeanWeights = CalcWeightsForGrid();
        foreach (var egg in CookedShapes)
        {
            string shapename = egg.CurrentShape.ToString();
            for ( int i = 0; i < _ObservedMeanWeights.Count; i++ )
            {
                float clamped_std = Mathf.Clamp(egg.STD_Point_Weights[i], 0.00001f, egg.STD_Point_Weights[i]);
                float zscore = HELPER_FUNCS.ABS_Z_Score(_ObservedMeanWeights[i], egg.Mean_Point_Weights[i], clamped_std);

                zscoreList.Add(zscore);
            }
            float z_average = zscoreList.Average();
            outputKeyValue.Add(new KeyValuePair<float, string>(z_average, shapename));
        }
        outputKeyValue = outputKeyValue.OrderBy(f => f.Key).ToList();
        
        print("1.) " + outputKeyValue[0].Value + " score: " + outputKeyValue[0].Key);
        for (int i = 1; i < outputKeyValue.Count; i++)
        {
            print(i + 1 + ".) " + outputKeyValue[i].Value + " score: " + outputKeyValue[i].Key);
        }

        _benchmark.Stop();
        long elapsedMilliseconds = _benchmark.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Done Checking! Time in ms: " + elapsedMilliseconds);
        _benchmark.Reset();
    }
    List<float> CalcWeightsForGrid()
    {
        List<float> meanpointweightsOutput = new List<float>();
        for (int i = 0; i < _KeyGridControlPoints.Count; i++)
        {
            meanpointweightsOutput.Add(Find_KNN_OneControlPoint(_KeyGridControlPoints[i], _GridStepSize, 15));
        }
        return meanpointweightsOutput;
    }

    /// <summary>
    /// This function returns in Vector2 (mean_weights, std_weights) for one point<br/>
    /// NOTE: for value k adhere to ratio k >= averageInputData.count/controlpoint.count 
    /// </summary>
    /// <param name="input_ControlPoint"></param>
    /// <param name="gridStepSize"></param>
    /// <param name="k"></param>
    /// <returns></returns>
    float Find_KNN_OneControlPoint(Vector2 input_ControlPoint, float gridStepSize, int k)
    {
        //This function return mean weights and std weights (weights are calculated through KNN to control point where step^5/dis^5 gives weight value)
        List<float> controlP_KNN_distances = new List<float>();// raw distances from control point to nearest pattern points
        List<float> weights = new List<float>();// list of overall weights
        List<float> subweights = new List<float>();//these are the weights from run to run, from each training data to training data
        float mean_weights;

        foreach (Vector2 raw_point in _InputData)
        {
            float pointdist = Vector2.Distance(input_ControlPoint, raw_point);
            controlP_KNN_distances.Add(pointdist);

            if (controlP_KNN_distances.Count > k) //if list is larger than requested size trim the max value
            {
                float farthestKNN = controlP_KNN_distances[0];
                // Find the furthest KNN with the maximum float value in a single pass and remove it
                for (int i = 1; i < controlP_KNN_distances.Count; i++)
                {
                    if (controlP_KNN_distances[i] > farthestKNN)
                    {
                        farthestKNN = controlP_KNN_distances[i];
                    }
                }
                controlP_KNN_distances.Remove(farthestKNN);
            }
            weights.AddRange(controlP_KNN_distances);
        }
        mean_weights = weights.Average();
        return mean_weights;
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
