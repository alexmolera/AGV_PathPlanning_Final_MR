using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class UpdateRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<TMPro.TextMeshPro>().text = gameObject.transform.parent.GetComponent<InteractionSlider>().defaultVerticalValue + "°";
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<TMPro.TextMeshPro>().text = Mathf.Round(gameObject.transform.parent.GetComponent<InteractionSlider>().VerticalSliderValue) + "°";
    }
}
