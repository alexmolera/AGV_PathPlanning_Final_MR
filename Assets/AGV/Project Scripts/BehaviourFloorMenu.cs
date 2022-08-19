using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;


public class BehaviourFloorMenu : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject Button1;  //Locate Marker and place plane
    public GameObject Button2;
    public GameObject Slider3;
    public GameObject Slider4;
    public GameObject Reset5;   //Reset Button

    [Header("PlaneDimensioner")]
    public GameObject PlaneDimensioner;

    [Header("Walls Animator")]
    public Animator animator;

    [Header("Plane and Wall Creator")]
    public GameObject wallCreator;
    public GameObject plane;
    public Material planeOn;
    public Material planeOff;

    [Header("Materials On/Off")]
    public Material ButonEnableMaterial;
    public Material ButonDisableMaterial;

    [Header("XRig")]
    public GameObject xRig;

    [Header("States")]
    private const int waiting = 0;
    private const int startRender = 1;
    private const int dimensionXZ = 2;
    private const int acceptDimension = 3;

    [Header("Debug Log text")]
    public GameObject debugLog;


    [Header("ID Constants")]    //For the switch function
    private const int ID_BUTTON_1 = 1;  //Start Render Button
    private const int ID_BUTTON_2 = 2;  //Confirm Button
    private const int ID_BUTTON_3 = 3;  //Dimension X Plane
    private const int ID_BUTTON_4 = 4;  //Dimension Z Plane
    private const int ID_BUTTON_5 = 5;  //Reset Button


    [HideInInspector]   public int currentState;
    [HideInInspector]   public int previousState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = waiting;
        previousState = waiting;

        SelectAction(currentState);

    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == startRender)
        {
            if (!VarjoMarkerManager.detected)
            {
                DisableButton(Button2);
            }
            else
            {
                EnableButton(Button2);
            }
        }
    }

    //Al ser presionado, se envía un ID desde el propio bottón.
    public void GetButtonNumber(int button_id)
    {
        switch (button_id)
        {
            //Button 1: comienza a trackear el marker. Se necesita confirmación para dejar el suelo estático y dejar de trackear.
            case ID_BUTTON_1:
                if (previousState == waiting)
                {
                    currentState = startRender;
                }
                break;

            //Button 2: confirmar cambios. Por lo tanto,´el estado siguiente dependerá del anterior
            case ID_BUTTON_2:
                switch (previousState) {
                    case startRender:
                        currentState = dimensionXZ;
                        break;
                    case dimensionXZ:
                        currentState = acceptDimension;
                        break;
                    default:
                        Debug.Log("Error in: GetButtonNumber/switch/case ID_BUTTON_2");
                        break;
                }
                break;

            case ID_BUTTON_3:
                break;

            case ID_BUTTON_4:
                break;

            case ID_BUTTON_5:
                if (previousState == acceptDimension)
                {
                    currentState = waiting;
                }
                break;

            default:
                Debug.Log("Error in: GetButtonNumber/switch");
                break;
        }
        SelectAction(currentState); //
    }

    private void SelectAction(int currentState)
    {
        switch (currentState)
        {
            case waiting:
                EnableButton(Button1);
                DisableButton(Button2);
                DisableSlider(Slider3);
                DisableSlider(Slider4);
                DisableButton(Reset5);
                DisableVarjoMarker();
                DisableDimension();
                plane.GetComponent<MeshRenderer>().enabled = true;    
                plane.GetComponent<MeshRenderer>().material = planeOn;     

                //En caso de que se resetee, los valores del slider vuelven a 0.
                if (previousState == acceptDimension) { 
                    ResetSliderValues(Slider3, Slider4);
                    wallCreator.GetComponent<MakeWalls>().ResetWalls();     //HACER FUNCION DIFERENTE PARA ESTA LINEA
                }

                PrintDebugLog("Waiting for search Varjo marker.");

                previousState = waiting;    //Esto realmente podria ir al final

                break;

            case startRender:
                DisableButton(Button1);
                EnableButton(Button2);
                DisableSlider(Slider3);
                DisableSlider(Slider4);
                DisableButton(Reset5);
                EnableVarjoMarker();

                Debug.Log("Searching Floor...");
                PrintDebugLog("Searching floor.\nPress confirm if it is visible and correctly placed.");

                previousState = startRender;

                break;

            case dimensionXZ:
                DisableButton(Button1);
                EnableButton(Button2);
                EnableSlider(Slider3);
                EnableSlider(Slider4);
                DisableButton(Reset5);
                DisableVarjoMarker();
                EnableDimension();

                Debug.Log("Floor Confirmed. Dimensioning Floor...");
                PrintDebugLog("Floor position confirmed. Scale floor if needed.");

                previousState = dimensionXZ;
                break;

            case acceptDimension:
                DisableButton(Button1);
                DisableButton(Button2);
                DisableSlider(Slider3);
                DisableSlider(Slider4);
                EnableButton(Reset5);
                DisableDimension();


                animator.SetTrigger("AnimateWalls");
                PrintDebugLog("Floor correctly placed. Please go to the following steps");

                plane.GetComponent<MeshRenderer>().material = planeOff;

                // Se crean los muros (HACER FUNCION APARTE PARA ESTO)
                wallCreator.SetActive(true);
                wallCreator.GetComponent<MakeWalls>().CreateWalls();

                previousState = acceptDimension;
                break;
                
                //******************************************************************
                //Falta añadir boton de restart o de error
                //******************************************************************
        }
    }

    private void EnableVarjoMarker()
    {
        xRig.GetComponent<VarjoMarkerManager>().ManualEnable(); //Habilita el trackeado del marker
    }

    private void DisableVarjoMarker()
    {
        xRig.GetComponent<VarjoMarkerManager>().ManualDisable(); //Deshabilita el trackeado del marker
    }

    private void DisableButton(GameObject Button)
    {

        Button.GetComponent<InteractionButton>().controlEnabled = false;
        //Button.transform.Find("Text").gameObject.SetActive(false);            //QUITAR EL TEXTO NO ES NECESARIO AQUI
        //Button.GetComponent<SimpleInteractionGlow>().enabled = false;
        Button.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = ButonDisableMaterial;

        for (int i = 1; i < Button.transform.GetChild(0).transform.childCount; i++)
        {
            Button.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void EnableButton(GameObject Button)
    {
        Button.GetComponent<InteractionButton>().controlEnabled = true;
        //Button.transform.Find("Text").gameObject.SetActive(true);
        //Button.GetComponent<SimpleInteractionGlow>().enabled = true;
        Button.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = ButonEnableMaterial;

        for (int i = 1; i < Button.transform.GetChild(0).transform.childCount; i++)
        {
            Button.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void EnableSlider(GameObject Button)
    {
        Button.GetComponent<InteractionSlider>().controlEnabled = true;
        //Button.GetComponent<SimpleInteractionGlow>().enabled = true;
        Button.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = ButonEnableMaterial;

        for (int i = 1; i < Button.transform.GetChild(0).transform.childCount; i++)
        {
            Button.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void DisableSlider(GameObject Button)
    {

        Button.GetComponent<InteractionSlider>().controlEnabled = false;
        //Button.GetComponent<SimpleInteractionGlow>().enabled = false;
        Button.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = ButonDisableMaterial;

        for (int i = 1; i < Button.transform.GetChild(0).transform.childCount; i++)
        {
            Button.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void EnableDimension()
    {
        PlaneDimensioner.SetActive(true);
    }

    private void DisableDimension()
    {
        PlaneDimensioner.SetActive(false);
    }

    private void ResetSliderValues(GameObject Slider2, GameObject Slider3)
    {
        Slider2.GetComponent<InteractionSlider>().VerticalSliderPercent = 0f;
        Slider3.GetComponent<InteractionSlider>().VerticalSliderPercent = 0f;
    }

    void PrintDebugLog(string n)
    {
        debugLog.GetComponent<TMPro.TextMeshPro>().text = "[" + System.DateTime.UtcNow.ToString("HH:mm:ss") + "]" + " " + n + "\n\n" + debugLog.GetComponent<TMPro.TextMeshPro>().text;
    }
}