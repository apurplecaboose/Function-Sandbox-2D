using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acc_Counter : MonoBehaviour
{
    [SerializeField] Acc_n_Failure Acc;
    void Start()
    {
        Acc.MaxAccuracy += transform.childCount;
    }
}
