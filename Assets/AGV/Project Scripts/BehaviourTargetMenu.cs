using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using Leap.Unity;
using UnityEngine.UI;

public class BehaviourTargetMenu : MonoBehaviour
{

    [Header("Buttons")]
    public GameObject Button1;  //Place Starting Point
    public GameObject Button2;  //Confirm Starting Point
    public GameObject Button3;  //Place Final Point
    public GameObject Button4;  //Confirm Final Point
    public GameObject Reset5;   //Reset Button

    [Header("Target Cubes & RTS")]
    public GameObject StartCube;
    public GameObject FinalCube;
    public GameObject CubesParent;    //Es el padre donde originalmente estan los cubos del start y el final
    public GameObject rtsParent;

    [Header("Floor Plane and Cube Prefab")]
    public GameObject floorPlane;

    [Header("Materials On/Off")]
    public Material ButonEnableMaterial;
    public Material ButonDisableMaterial;
    public Material OnCubeGreen;

    [Header("Laser Lines")]
    public GameObject laserLineLeft;
    public GameObject laserLineRight;

    [Header("Warning Text")]
    public GameObject warningText;

    [Header("Debug Log text")]
    public GameObject debugLog;

    [Header("States")]
    private const int waiting           = 0;        // Waiting for appearing starting point
    private const int waitingStartPoint = 1;        // waiting for placing starting point  
    private const int startPointPlaced  = 2;        // starting point confirmed, waiting for appearing final point
    private const int waitingFinalPoint = 3;        // waiting for placing final point 
    private const int finalPointPlaced  = 4;        // final point is placed. (waiting for Reset)

    [Header("ID Constants")]    //For the switch function
    private const int ID_BUTTON_1 = 1;  //Start Render Button
    private const int ID_BUTTON_2 = 2;  //Confirm Button
    private const int ID_BUTTON_3 = 3;  //Dimension X Plane
    private const int ID_BUTTON_4 = 4;  //Dimension Z Plane
    private const int ID_BUTTON_5 = 5;  //Reset Button

    [HideInInspector]   public int currentState;
    [HideInInspector]   public int previousState;

    public static bool placeStartAllowed = true;     //Servirá para que no se pueda colocar los puntos iniciales y finales en sitios no permitidos
    public static bool placeFinalAllowed = true;

    private Vector3 rotation_null = new Vector3(0,0,0);

    // Start is called before the first frame update
    void Start()
    {
        StartCube.SetActive(false);
        FinalCube.SetActive(false);
        warningText.SetActive(false);
        //animatorStart = animatorStart.GetComponent<Animator>();
        //animatorFinal = animatorFinal.GetComponent<Animator>();

        currentState = waiting;
        previousState = waiting;
        SelectAction(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState==waitingStartPoint && !placeStartAllowed)
        {
            DisableButton(Button2);
            warningText.SetActive(true);
        }
        else if (currentState == waitingStartPoint && placeStartAllowed)
        {
            EnableButton(Button2);
            warningText.SetActive(false);
        }
        
        if(currentState == waitingFinalPoint && !placeFinalAllowed)
        {
            DisableButton(Button4);
            warningText.SetActive(true);
        }
        else if (currentState == waitingFinalPoint && placeFinalAllowed)
        {
            EnableButton(Button4);
            warningText.SetActive(false);
        }
    }

    public void GetButtonNumber(int button_id)
    {
        switch (button_id)
        {
            //Button 1: aparece el cubito del starting point 
            case 0:
                currentState = waiting;
                break;
            
            case ID_BUTTON_1:
                if (previousState == waiting)
                {
                    currentState = waitingStartPoint;
                }
                break;

            //Button 2: confirmar starting point
            case ID_BUTTON_2:
                if (previousState == waitingStartPoint)
                {
                    currentState = startPointPlaced;
                }

                break;

            //Button 3: aparece el cubito del final point
            case ID_BUTTON_3:
                if (previousState == startPointPlaced)
                {
                    currentState = waitingFinalPoint;
                }
                break;

            //Button 4: confirmar final point
            case ID_BUTTON_4:
                if (previousState == waitingFinalPoint)
                {
                    currentState = finalPointPlaced;
                }
                break;

            //Button 5: resetear
            case ID_BUTTON_5:
                if (previousState == finalPointPlaced)
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
                DisableButton(Button3);
                DisableButton(Button4);
                DisableButton(Reset5);

                //Desactivar cubos para que no aparezcan en escena y activar componente de detectar obstaculos
                StartCube.GetComponent<MeshRenderer>().material = OnCubeGreen;
                FinalCube.GetComponent<MeshRenderer>().material = OnCubeGreen;
                StartCube.SetActive(false);
                FinalCube.SetActive(false);   

                previousState = waiting;

                break;

            case waitingStartPoint:
                DisableButton(Button1);
                EnableButton(Button2);
                DisableButton(Button3);
                DisableButton(Button4);
                DisableButton(Reset5);

                AppearCube(StartCube);

                PrintDebugLog("Moving start point.");

                previousState = waitingStartPoint;

                break;

            case startPointPlaced:
                DisableButton(Button1);
                DisableButton(Button2);
                EnableButton(Button3);
                DisableButton(Button4);
                DisableButton(Reset5);

                DettachCube(StartCube);

                previousState = startPointPlaced;
                break;

            case waitingFinalPoint:
                DisableButton(Button1);
                DisableButton(Button2);
                DisableButton(Button3);
                EnableButton(Button4);
                DisableButton(Reset5);

                AppearCube(FinalCube);

                PrintDebugLog("Moving target point.");

                previousState = waitingFinalPoint;
                break;

            case finalPointPlaced:
                DisableButton(Button1);
                DisableButton(Button2);
                DisableButton(Button3);
                DisableButton(Button4);
                EnableButton(Reset5);

                DettachCube(FinalCube);

                previousState = finalPointPlaced;
                break;
        }
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
        Button.transform.Find("Text").gameObject.SetActive(true);
        //Button.GetComponent<SimpleInteractionGlowSlider>().enabled = true;
        Button.GetComponentInChildren<MeshRenderer>().material = ButonEnableMaterial;
    }

    private void DisableSlider(GameObject Button)
    {

        Button.GetComponent<InteractionSlider>().controlEnabled = false;
        Button.transform.Find("Text").gameObject.SetActive(false);
        Button.GetComponentInChildren<MeshRenderer>().material = ButonDisableMaterial;

        //Button.GetComponent<SimpleInteractionGlowSlider>().enabled = false;
    }

    public void AppearCube(GameObject cube)
    {

        // cubePrefab reset
        cube.SetActive(true);
        float initialScaleY = cube.transform.localScale.y;

        // cubePrefab positioning in 0,0 on the plane. Also animating appearence.
        cube.transform.parent = floorPlane.transform;                             //*******//PARENT: FLOOR
        cube.transform.localPosition = new Vector3(100, initialScaleY / 2, 100);
        cube.transform.localPosition = new Vector3(0, initialScaleY/2, 0);

        //Se le asigna el laser para que moverlo
        laserLineLeft.transform.parent = null;
        laserLineRight.transform.parent = null;
        laserLineLeft.SetActive(true);
        laserLineRight.SetActive(true);
        laserLineLeft.GetComponent<Electric>().transformPointB = cube.transform;
        laserLineRight.GetComponent<Electric>().transformPointB = cube.transform;

        //Animacion de entrada
        //cubePrefab.GetComponent<Animator>().enabled = true;
        //animator.SetTrigger("AnimateCube");

        // cubePrefab moving and rotating over the plane
        rtsParent.SetActive(true);
        cube.transform.parent = rtsParent.transform;                              //*******//PARENT: RTS
        ActivateRTS();
    }

    public void DettachCube(GameObject cube)
    {
        // Se desactiva RTS
        DeactivateRTS();

        cube.transform.parent = floorPlane.transform;
        PrintDebugLog("Point placed at: \tx: " + (Mathf.Round(cube.transform.localPosition.x * 10) / 10f) + "\tz: " + (Mathf.Round(cube.transform.localPosition.z * 10) / 10f));

        //Se vuelve a poner de padre a la carpeta originaria
        cube.transform.parent = CubesParent.transform;  

        //Se apaga el gameObject RTS
        rtsParent.SetActive(false);

        //AQUI FALTA CAMBIAR COLOR O ALGO CUANDO SE DESANCLA
    } 


    public void ActivateRTS()
    {
        LeapRTS.RTSenabled = true;
    }
    public void DeactivateRTS()
    {
        LeapRTS.RTSenabled = false;
    }

    void PrintDebugLog(string n)
    {
        debugLog.GetComponent<TMPro.TextMeshPro>().text = "[" + System.DateTime.UtcNow.ToString("HH:mm:ss") + "]" + " " + n + "\n\n" + debugLog.GetComponent<TMPro.TextMeshPro>().text;
    }
}
