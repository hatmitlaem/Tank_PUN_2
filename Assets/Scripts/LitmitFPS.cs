using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitmitFPS : MonoBehaviour
{
    void Start()
    {
       // Screen.SetResolution(800, 600, false);
        QualitySettings.vSyncCount = 2;
        Application.targetFrameRate = 40;
    }

}
