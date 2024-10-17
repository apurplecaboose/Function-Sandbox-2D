using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlPoints : MonoBehaviour
{
    [SerializeField] SubSigilController _sigilMaster;
    public int ControlPointNumber;
    public Acc_n_Failure F_in_Chat;

    private void Awake()
    {
        _sigilMaster = transform.parent.GetComponent<SubSigilController>();
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.CompareTag("Doodle"))
        {
            _sigilMaster.CurrentControlPointNumber += 1;
            if(_sigilMaster.CurrentControlPointNumber != ControlPointNumber)
            {
                Debug.Log("Sigil FAILED Missed Controll Point");
                F_in_Chat.FAIL();
            }
        }
    }
}
