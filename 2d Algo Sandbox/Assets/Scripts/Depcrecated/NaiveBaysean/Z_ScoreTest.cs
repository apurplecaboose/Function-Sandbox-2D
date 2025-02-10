using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class Z_ScoreTest : MonoBehaviour
{
    public DoodleMasterOCR DOODLEMASTERCOMPONENT;
    
    Stopwatch _benchmark;//FOR TESTING ONLY
    [SerializeField] float _Keypointscale = 30000;
    int _gridsize = 11;
    List<Vector2> _KeyPoints; //top left to bottom right; left to right; top to bottom; 
   
    [Header("DEBUG")] //output for check
    [SerializeField] List<Vector2> _InputData;
    List<List<float>> _rawsigmaNested;
    List<float> _observed_means;

    public List<Cooked_Shape> CookedShapeReferenceData;

    void Awake()
    {
        _rawsigmaNested = new List<List<float>>();
        _observed_means = new List<float>();
        _KeyPoints = new List<Vector2>();
        var grid = HELPER_FUNCS.GenerateThenScaleGridPoints(_gridsize, _Keypointscale);
        _KeyPoints = grid.Item1;

        _benchmark = new Stopwatch();
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Invoke("DoodleNumberSorting", 0); //run next frame so to prevent race condition
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _benchmark.Start();
            CheckAgainstKeyPoints(_InputData);
            GetZ_Scores();
        }
        if (Input.GetKeyDown(KeyCode.N)) // generate noise 
        {
            GenerateNoiseDataSet(60000, 10000);
        }
    }
    void GenerateNoiseDataSet(float maxRange, int arraysize)
    {
        if (_InputData.Count > 0) _InputData.Clear();
        for (int i = 0; i < arraysize; i++)
        {
            float rand_value1 = UnityEngine.Random.Range(-maxRange, maxRange);
            float rand_value2 = UnityEngine.Random.Range(-maxRange, maxRange);
            _InputData.Add(new Vector2(rand_value1, rand_value2));
        }
        print("random Array Generated");
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
    void CheckAgainstKeyPoints(List<Vector2> inputdata)
    {
        for (int i = 0; i < _KeyPoints.Count; i++)
        {
            List<float> rawsigmas = new List<float>();
            rawsigmas.AddRange(RawOnePoint_MeanSquareDistance(_KeyPoints[i]));
            _rawsigmaNested.Add(rawsigmas);
        }

        for(int i = 0; i < _KeyPoints.Count; i++)
        {
            _observed_means.Add(_rawsigmaNested[i].Average());
        }

        List<float> RawOnePoint_MeanSquareDistance(Vector2 keypoint) // will output the names of the nearest neighbor reference pattern names
        {//THIS IS FINE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            List<float> outputlist = new List<float>();
            List<float> meansquareDistances = new List<float>();
            foreach (Vector2 v in inputdata)
            {
                float distancetokeypoint = Vector2.Distance(v, keypoint);
                distancetokeypoint /= 1000;// reduce the scale of the numbers to something fathomable
                float squareDistance = Mathf.Pow(distancetokeypoint, 2); // squared distance to increase error
                meansquareDistances.Add(squareDistance);
            }
            outputlist.Add(meansquareDistances.Sum());

            return outputlist;
        }
    }
     void GetZ_Scores()
    {
        List<KeyValuePair<float, string>> keypairlist = new List<KeyValuePair<float, string>>();
        foreach (var cookedshape in CookedShapeReferenceData)
        {
            List<float> zscores = new List<float>();
            for (int i = 0; i < _observed_means.Count; i++)
            {
                float singlezscore = Mathf.Abs((_observed_means[i] - cookedshape.Mean_Point_Weights[i]) / cookedshape.STD_Point_Weights[i]);
                zscores.Add(singlezscore); 
            }
            float z_sum = /*cookedshape.DifficultyMultiplier **/ zscores.Average();
            string shapename = cookedshape.CurrentShape.ToString();
            keypairlist.Add(new KeyValuePair<float, string>(z_sum, shapename));
        }
        keypairlist = keypairlist.OrderBy(f => f.Key).ToList();
        float key = keypairlist[0].Key;
        string value = keypairlist[0].Value;
        print("0 NUMBER Shape Should Be: " + value + " with a zsum of: " + key);
        for (int i = 1; i < keypairlist.Count; i++)
        {
            float runnerupkey = keypairlist[i].Key;
            string runnerupvalue = keypairlist[i].Value;
            print(i + " runner ups: " + runnerupvalue + " z score sum: " + runnerupkey);
        }
        _benchmark.Stop();
        long elapsedMilliseconds = _benchmark.ElapsedMilliseconds;
        print("DONE calculating z scores! Time in ms: " + elapsedMilliseconds);
    }
}
