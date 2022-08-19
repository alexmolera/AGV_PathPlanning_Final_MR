using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class DimensionObject : MonoBehaviour
{
    public InteractionSlider sliderX;
    public InteractionSlider sliderZ;
    public GameObject FloorPlaneObject;

    [Header("Plane Dimensions")]
    public float initialX;
    public float initialZ;
    public float maxX;
    public float maxZ;

    private float anchorPlane = 1;

    // Start is called before the first frame update
    void Start()
    {
        FloorPlaneObject.transform.localScale = new Vector3(initialX, anchorPlane, initialZ);
    }

    private void OnEnable()
    {
        FloorPlaneObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(sliderX.VerticalSliderValue + "   " + sliderZ.VerticalSliderValue);

    }

    void FixedUpdate()
    {
        
        if (sliderX.isPressed || sliderZ.isPressed)
        {
            FloorPlaneObject.transform.localScale = new Vector3(initialX + (maxX - initialX) * sliderX.VerticalSliderPercent, anchorPlane, initialZ + (maxZ - initialZ) * sliderZ.VerticalSliderPercent);
        }
        
    }
}
