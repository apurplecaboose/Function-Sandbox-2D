using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class Z_ScoreTest : MonoBehaviour
{
    public DoodleMasterOCR DOODLEMASTERCOMPONENT;
    
    Stopwatch _benchmark;//FOR TESTING ONLY
    float _Keypointscale = 75000;
    
    List<Vector2> _KeyPoints; //top left to bottom right; left to right; top to bottom; 
    List<float> _rawSigma_top, _rawSigma_bottom, _rawSigma_left, _rawSigma_right, _rawSigma_center;
   
    [Header("DEBUG")] //output for check
    [SerializeField] List<Vector2> _InputData;
    [SerializeField] float _obs_mean_top, _obs_mean_bottom, _obs_mean_left, _obs_mean_right, _obs_mean_middle;
    List<List<float>> _rawsigmaNested;
    List<float> _observed_means;

    public List<Cooked_Shape> CookedShapeReferenceData;

    void Awake()
    {
        //_rawSigma_bottom = new List<float>();
        //_rawSigma_center = new List<float>();
        //_rawSigma_left = new List<float>();
        //_rawSigma_right = new List<float>();
        //_rawSigma_top = new List<float>();
        _rawsigmaNested = new List<List<float>>();
        _observed_means = new List<float>();
        //_KeyPoints = new List<Vector2> { new Vector2 (-1,1), new Vector2 (0,1), new Vector2 (1,1),
        //                                new Vector2 (-1,0), new Vector2 (0,0), new Vector2 (1,0),
        //                                new Vector2 (-1,1), new Vector2 (-1,0), new Vector2 (-1,1
        //                                
        _KeyPoints = GenerateGridPoints(9);
        // scale keypoints to new arbitrary scale
        for (int i = 0; i < _KeyPoints.Count; i++)
        {
            _KeyPoints[i] = new Vector2(_KeyPoints[i].x * _Keypointscale, _KeyPoints[i].y * _Keypointscale);
        }
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
        //_obs_mean_top = _rawSigma_top.Average();
        //_obs_mean_bottom = _rawSigma_bottom.Average();
        //_obs_mean_left = _rawSigma_left.Average();
        //_obs_mean_right = _rawSigma_top.Average();
        //_obs_mean_middle = _rawSigma_center.Average();

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
            outputlist.Add(meansquareDistances.Average());

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
                float singlezscore = Mathf.Abs((_observed_means[i] - cookedshape.meanPoints[i]) / cookedshape.STD_Points[i]);
                zscores.Add(singlezscore); 
            }
            float z_sum = zscores.Sum();
            //float z_Left = Mathf.Abs((_obs_mean_left - cookedshape.mean_left) / cookedshape.std_left);
            //float z_Right = Mathf.Abs((_obs_mean_right - cookedshape.mean_right) / cookedshape.std_right);
            //float z_Middle = Mathf.Abs((_obs_mean_middle - cookedshape.mean_middle) / cookedshape.std_middle);
            //float z_Top = Mathf.Abs((_obs_mean_top - cookedshape.mean_top) / cookedshape.std_top);
            //float z_Bottom = Mathf.Abs((_obs_mean_bottom - cookedshape.mean_bottom) / cookedshape.std_bottom);
            //float z_sum = z_Left + z_Right + z_Middle + z_Top + z_Bottom;
            string shapename = cookedshape.CurrentShape.ToString();
            keypairlist.Add(new KeyValuePair<float, string>(z_sum, shapename));
        }
        keypairlist = keypairlist.OrderBy(f => f.Key).ToList();
        float key = keypairlist[0].Key;
        string value = keypairlist[0].Value;
        float runnerupkey = keypairlist[1].Key;
        string runnerupvalue = keypairlist[1].Value;
        print("Shape Should Be: " + value + " with a zsum of: " + key + "runner up: " + runnerupvalue + " z score sum: " + runnerupkey);
        _benchmark.Stop();
        long elapsedMilliseconds = _benchmark.ElapsedMilliseconds;
        print("DONE calculating z scores! Time in ms: " + elapsedMilliseconds);
    }
}
