using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varjo.XR;

public class StartRender : MonoBehaviour
{
    private void Start()
    {

    }

    private void OnEnable()
    {
        // Enable Video-Pass
        VarjoMixedReality.StartRender();
        //VarjoMixedReality.EnableDepthEstimation();  //Calcula distancias de objetos reales. Va muy petado
    }

    private void OnDisable()
    {
        // Disable Video-Pass
        VarjoMixedReality.StopRender();
        //VarjoMixedReality.DisableDepthEstimation();
    }

    public void OnDepth()
    {
        VarjoMixedReality.EnableDepthEstimation();
    }

    public void OffDepth()
    {
        VarjoMixedReality.DisableDepthEstimation();
    }
}