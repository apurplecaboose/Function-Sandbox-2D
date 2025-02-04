using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class Trainer : MonoBehaviour
{
    [SerializeField] float _Keypointscale = 75000;
    int _gridsize = 9;
    public Cooked_Shape TrainingTarget;
    public List<RawTrainingData> TrainingData;

    Stopwatch _benchmark;//FOR TESTING ONLY
    List<Vector2> _KeyPoints; //top left to bottom right; left to right; top to bottom; 
    private List<float> _rawSigma_top, _rawSigma_bottom, _rawSigma_left, _rawSigma_right, _rawSigma_center;
    List<List<float>> _rawsigmaNested;
    void Awake()
    {
        _rawsigmaNested = new List<List<float>>();
        _rawSigma_bottom = new List<float>();
        _rawSigma_center = new List<float>();
        _rawSigma_left = new List<float>();
        _rawSigma_right = new List<float>();
        _rawSigma_top = new List<float>();
        //_KeyPoints = new List<Vector2> { new Vector2 (-1,1), new Vector2 (0,1), new Vector2 (1,1),
        //                                new Vector2 (-1,0), new Vector2 (0,0), new Vector2 (1,0),
        //                                new Vector2 (-1,1), new Vector2 (-1,0), new Vector2 (-1,1)};
        //// scale keypoints to new arbitrary scale
        ///
        _KeyPoints = GenerateGridPoints(_gridsize);
        for (int i = 0; i < _KeyPoints.Count; i++)
        {
            _KeyPoints[i] = new Vector2(_KeyPoints[i].x * _Keypointscale, _KeyPoints[i].y * _Keypointscale);
        }

        if(TrainingTarget == null) UnityEngine.Debug.LogError("No Target!!!");
        if (TrainingData.Count == 0) UnityEngine.Debug.LogError("No Training Data!!!");
        _benchmark = new Stopwatch();
    }
    List<Vector2> GenerateGridPoints(int gridsize)
    {
        List<Vector2> keyPoints = new List<Vector2>();
        float step = 2.0f / (gridsize - 1); // Calculate the step size based on the range and the number of points
        for (int y = 0; y < gridsize; y++)
        {
            for (int x = 0; x < gridsize; x++)
            {
                keyPoints.Add(new Vector2(-1 + x * step, 1 - y * step));
            }
        }
        return keyPoints;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            print("start training");
            _benchmark.Start();
            EXECUTE_the_Training();
        }
    }
    void EXECUTE_the_Training()
    {
        for (int i = 0; i < _KeyPoints.Count; i++)
        {
            List<float> rawsigmas = new List<float>();
            rawsigmas.AddRange(RawOnePoint_MeanSquareDistance(_KeyPoints[i]));
            _rawsigmaNested.Add(rawsigmas);


            //switch (i)
            //{
            //    case 0:
            //        _rawSigma_top.AddRange(rawsigmas);
            //        _rawSigma_left.AddRange(rawsigmas);
            //        break;
            //    case 1:
            //        _rawSigma_top.AddRange(rawsigmas);
            //        break;
            //    case 2:
            //        _rawSigma_top.AddRange(rawsigmas);
            //        _rawSigma_right.AddRange(rawsigmas);
            //        break;
            //    case 3:
            //        _rawSigma_left.AddRange(rawsigmas);
            //        break;
            //    case 4:
            //        _rawSigma_center.AddRange(rawsigmas); //middle
            //        break;
            //    case 5:
            //        _rawSigma_right.AddRange(rawsigmas);
            //        break;
            //    case 6:
            //        _rawSigma_bottom.AddRange(rawsigmas);
            //        _rawSigma_left.AddRange(rawsigmas);
            //        break;
            //    case 7:
            //        _rawSigma_bottom.AddRange(rawsigmas);
            //        break;
            //    case 8:
            //        _rawSigma_bottom.AddRange(rawsigmas);
            //        _rawSigma_right.AddRange(rawsigmas);
            //        break;
            //}
        }
        FillData();
    }

    void FillData()
    {
        List<float> meanoutput = new List<float>();
        List<float> stdoutput = new List<float>();
        for (int i = 0; i <_KeyPoints.Count; i++)
        {
            Vector2 mean_and_STD = CalculateMeanAndStdDev(_rawsigmaNested[i]);
            meanoutput.Add(mean_and_STD.x);
            stdoutput.Add(mean_and_STD.y);
        }
        TrainingTarget.meanPoints = meanoutput;
        TrainingTarget.STD_Points = stdoutput;

        //Vector2 top = CalculateMeanAndStdDev(_rawSigma_top);
        //TrainingTarget.mean_top = top.x;
        //TrainingTarget.std_top = top.y;
        //Vector2 bot = CalculateMeanAndStdDev(_rawSigma_bottom);
        //TrainingTarget.mean_bottom = bot.x;
        //TrainingTarget.std_bottom = bot.y;
        //Vector2 left = CalculateMeanAndStdDev( _rawSigma_left);
        //TrainingTarget.mean_left = left.x;
        //TrainingTarget.std_left = left.y;
        //Vector2 right = CalculateMeanAndStdDev(_rawSigma_top);
        //TrainingTarget.mean_right = right.x;
        //TrainingTarget.std_right = right.y;
        //Vector2 middle = CalculateMeanAndStdDev(_rawSigma_center);
        //TrainingTarget.mean_middle =middle.x;
        //TrainingTarget.std_middle = middle.y;

        LetsGetDirty(TrainingTarget);
        _benchmark.Stop();
        long elapsedMilliseconds = _benchmark.ElapsedMilliseconds;
        print("DONE Training Data! Time in ms: " + elapsedMilliseconds);
    }
    List<float> RawOnePoint_MeanSquareDistance(Vector2 keypoint) // will output the names of the nearest neighbor reference pattern names
    {
        List<float> outputlist = new List<float>();
        List<float> meansquareDistances = new List<float>();
        foreach (var raw in TrainingData)
        {
            foreach (Vector2 v in raw.RawVector2DataPoints)
            {
                float distancetokeypoint = Vector2.Distance(v, keypoint);
                distancetokeypoint /= 1000;// reduce the scale of the numbers to something fathomable
                float squareDistance = Mathf.Pow(distancetokeypoint, 2); // squared distance to increase error
                meansquareDistances.Add(squareDistance);
            }
            outputlist.Add(meansquareDistances.Average());
        }
        return outputlist;
    }

    public Vector2 CalculateMeanAndStdDev(List<float> data)
    {
        // Calculate mean
        float mean = data.Average();

        // Calculate standard deviation
        float sumOfSquaresOfDifferences = data.Select(val => (val - mean) * (val - mean)).Sum();
        float stdDev = Mathf.Sqrt(sumOfSquaresOfDifferences / data.Count);

        return new Vector2 (mean, stdDev);
    }
#if UNITY_EDITOR
    void LetsGetDirty(UnityEngine.Object scriptableObject)
    {
        UnityEditor.EditorUtility.SetDirty(scriptableObject);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
