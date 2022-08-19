using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class waitSeconds : MonoBehaviour
{
    //This scripts serves to wait for X seconds until the next pulse of the toggle button

    private InteractionToggle Button;
    private float startTime=0;

    private void Awake()
    {
        Button = GetComponent<InteractionToggle>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Button.isPressed)
        {
            startTime = Time.time;
        }

        if ((Time.time - startTime) < 2)
        {
            Button.controlEnabled = false;
        }
        else
            Button.controlEnabled = true;
    }
}
