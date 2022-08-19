using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Attributes;
using Leap.Unity.Interaction;

public class DebugSlider : MonoBehaviour
{
    public float debugSlider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<InteractionSlider>().HorizontalSliderValue = debugSlider;
    }
}
