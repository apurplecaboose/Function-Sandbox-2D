using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class GKNN_Trainer : MonoBehaviour
{
    Stopwatch _benchmark;//FOR TESTING ONLY


    [SerializeField] bool _DrawDebugGrid;
    [SerializeField] int _gridsize = 11;
    float _GridStepSize;
    [SerializeField] float _Keypointscale = 75000;
    [SerializeField]List<Vector2> _KeyGridControlPoints;

    public Cooked_Shape TrainingTarget;
    public List<RawTrainingData> TrainingData;
    void OnDrawGizmos()
    {
        if (_DrawDebugGrid) // draw point grid
        {
            foreach (Vector2 point in _KeyGridControlPoints)
            {
                Gizmos.DrawSphere(new Vector3(point.x, point.y, 0), 500f);
            }
        }
    }
    private void Awake()
    {
        _benchmark = new Stopwatch();
        _KeyGridControlPoints = new List<Vector2>();
    }
    void Start()
    {
        var grid_and_step = HELPER_FUNCS.GenerateThenScaleGridPoints(_gridsize, _Keypointscale);
        _KeyGridControlPoints = grid_and_step.Item1;
        _GridStepSize = grid_and_step.Item2;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            TrainAndOutputToScriptable();
        }
    }
    void TrainAndOutputToScriptable()
    {
        UnityEngine.Debug.Log("Start Training");
        _benchmark.Start();
        var output = CalcWeightsAndStatsForGrid();
#if UNITY_EDITOR
        TrainingTarget.Mean_Point_Weights = output.Item1;
        TrainingTarget.STD_Point_Weights = output.Item2;
        HELPER_FUNCS.LetsGetDirty(TrainingTarget);
#endif
        _benchmark.Stop();
        long elapsedMilliseconds = _benchmark.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Done Training! Time in ms: " + elapsedMilliseconds);
        _benchmark.Reset();
    }
    (List<float>, List<float>) CalcWeightsAndStatsForGrid()
    {
        List<float> meanpointweightsOutput = new List<float>();
        List<float> stdpointweightsOutput = new List<float>();
        for ( int i = 0; i < _KeyGridControlPoints.Count; i++ )
        {
            Vector2 data = Find_KNN_OneControlPoint(_KeyGridControlPoints[i], _GridStepSize, 5);
            meanpointweightsOutput.Add(data.x);
            stdpointweightsOutput.Add(data.y);
        }
        return (meanpointweightsOutput, stdpointweightsOutput);
    }

    /// <summary>
    /// This function returns in Vector2 (mean_weights, std_weights) for one point<br/>
    /// NOTE: for value k adhere to ratio k >= averageInputData.count/controlpoint.count 
    /// </summary>
    /// <param name="input_ControlPoint"></param>
    /// <param name="gridStepSize"></param>
    /// <param name="k"></param>
    /// <returns></returns>
    Vector2 Find_KNN_OneControlPoint(Vector2 input_ControlPoint, float gridStepSize, int k) 
    {
        //This function return mean weights and std weights (weights are calculated through KNN to control point where step^5/dis^5 gives weight value)
        List<float> controlP_KNN_distances = new List<float>();// raw distances from control point to nearest pattern points
        List<float> weights = new List<float>();// list of overall weights
        List<float> subweights = new List<float>();//these are the weights from run to run, from each training data to training data
        float mean_weights;
        float std_weights;

        foreach (var trainingdata in TrainingData) // foreach training data in trainingdata scriptable list
        {
            foreach (Vector2 raw_point in trainingdata.RawVector2DataPoints)
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
            }
            
            for (int i = 0; i < controlP_KNN_distances.Count; i++)// calculating the weighting using step^2/dis^2
            {
                float step = gridStepSize / 1000; // make the weights a more readable number
                float dis = controlP_KNN_distances[i] / 1000;
                float stepSquared = Mathf.Pow(step, 5);
                float disSquared = Mathf.Pow(dis, 5);
                float singlePointWeight = stepSquared / disSquared;
                subweights.Add(singlePointWeight);
            }
            float mean_subweights = subweights.Average();
            subweights.Clear();
            weights.Add(mean_subweights);
        }
        mean_weights = weights.Average();
        std_weights = HELPER_FUNCS.CalculateStdDev(weights);
        return new Vector2 (mean_weights, std_weights);
    }
}
