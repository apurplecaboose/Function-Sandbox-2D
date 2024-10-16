using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSigilController : MonoBehaviour
{
    public int CurrentControlPointNumber = 0;
    [SerializeField] int _ExpectedC_PointNumb;
    public GameObject NextSubSigil;

    public DoodleMaster dood;
    private void Awake()
    {
        _ExpectedC_PointNumb = transform.childCount;
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
                    dood.SetFinalColor(Color.red, 1);
                    Destroy(transform.parent.gameObject);
                }
            }
            else
            {
                Debug.Log("Sigil FAILED Missed Controll Point");
            }
        }
    }
}
