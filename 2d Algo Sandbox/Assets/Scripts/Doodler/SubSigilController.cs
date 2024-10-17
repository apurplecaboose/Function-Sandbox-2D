using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSigilController : MonoBehaviour
{
    public int CurrentControlPointNumber = 0;
    [SerializeField] int _ExpectedC_PointNumb;
    public GameObject NextSubSigil;

    public DoodleMaster dood;
    public Acc_n_Failure F_in_Chat; 
    private void Awake()
    {
        _ExpectedC_PointNumb = transform.childCount - 1; //minus acc
        CurrentControlPointNumber = 0;
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (CurrentControlPointNumber == _ExpectedC_PointNumb)
            {
                Debug.Log("All controll points hit!!!");
                if(NextSubSigil != null)
                {
                    NextSubSigil.SetActive(true);
                    Destroy(gameObject);
                }
                else
                {
                    if (F_in_Chat.AccPercentage <= 25f)
                    {
                        Debug.Log("too innaccurate. Accuracy:" + F_in_Chat.AccPercentage);
                        F_in_Chat.FAIL();
                        return;
                    }
                    Debug.Log("Accuracy:" + F_in_Chat.AccPercentage);
                    dood.SetFinalColor(Color.red, 1);
                    Destroy(transform.parent.gameObject);
                }
            }
            else
            {
                Debug.Log("Sigil FAILED Missed Controll Point");
                F_in_Chat.FAIL();
            }
        }
    }
}
