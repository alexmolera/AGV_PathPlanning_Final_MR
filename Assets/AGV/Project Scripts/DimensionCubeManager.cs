using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class DimensionCubeManager : MonoBehaviour
{
    [Header("Objects and Sliders")]
    public InteractionSlider sliderX;
    public InteractionSlider sliderZ;
    public InteractionSlider sliderRot;

    public GameObject FloorPlane;       //Para rotar respecto a este plano
    public GameObject CubeObject;
    public GameObject RTSParent;

    private float initialX, initialY, initialZ;
    private bool firstRotation;
    // Start is called before the first frame update
    void Start()
    {
        initialX = CubeObject.transform.localScale.x;
        initialZ = CubeObject.transform.localScale.z;
        initialY = CubeObject.transform.localScale.y;

        firstRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //La siguiente linea es debido a que cuando aparece el cubo, el angulo no es 0 puesto que nunca se ha declarado asi. Es un poco cutre pero sirve
        if (firstRotation)
        {
            CubeObject.transform.parent = FloorPlane.transform; //Es una fumada pero no se me ocurre otra cosa que rotar respecto al plano
            CubeObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            CubeObject.transform.parent = RTSParent.transform;
            firstRotation = false;
        }

        if (sliderX.isPressed || sliderZ.isPressed || sliderRot.isPressed)
        {
            CubeObject.transform.localScale = new Vector3((initialX*sliderX.HorizontalSliderValue), initialY, (initialZ*sliderZ.HorizontalSliderValue));
        }

        if (sliderRot.isPressed)
        {
            CubeObject.transform.parent = FloorPlane.transform; //Es una fumada pero no se me ocurre otra cosa que rotar respecto al plano
            CubeObject.transform.localEulerAngles = new Vector3(0,sliderRot.VerticalSliderValue, 0);
            CubeObject.transform.parent = RTSParent.transform;  // Vuelve a tener de padre al RTS para que se siga pudiendo modificar, lo de arriba es solo para poder rotar respecto al plano
        }
    }
}
