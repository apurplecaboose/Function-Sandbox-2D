using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acc_n_Failure : MonoBehaviour
{
    public GameObject reee;
    public int AccuracyIndex;
    public int MaxAccuracy;
    public float AccPercentage;
    void Start()
    {
        AccuracyIndex = 0;
    }

    void Update()
    {
        AccPercentage = (float)AccuracyIndex / (float)MaxAccuracy * 100f;
        //Debug.Log(AccPercentage);
    }
    public void FAIL()
    {
        Destroy(gameObject);
        reee.SetActive(true);

    }
}
