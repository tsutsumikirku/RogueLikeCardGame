using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testResultCall : MonoBehaviour
{
    public void CallResult(bool isV)
    {
        ResultScript.LoadResultScene(isV);
    }
}
