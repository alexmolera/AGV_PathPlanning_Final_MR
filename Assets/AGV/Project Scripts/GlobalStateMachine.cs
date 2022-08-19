using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using Leap.Unity;
using UnityEngine.UI;


public class GlobalStateMachine : MonoBehaviour
{
    [Header("Menus")]
    public GameObject floorMenu;
    public GameObject objectMenu;
    public GameObject targetMenu;
    public GameObject simulationMenu;
    public GameObject importButton;

    [Header("Switch Between Import & Objects")]
    public GameObject cubePlacementmenu;
    public GameObject ImportDataMenu;

    [Header("Algorithm")]
    public GameObject manager;
    public GameObject movement;

    [Header("Buttons")]
    public GameObject Button1;
    public GameObject Button2;
    public GameObject Button3;
    public GameObject Button4;

    [Header("Camera")]
    public GameObject SnapCamera;

    [Header("Materials")]
    public Material OnButtonMaterial;
    public Material OffButtonMaterial;
    public Material PressedButtonMaterial;

    [Header("States")]
    private const int floorPlacing = 1;
    private const int objectsPlacing = 2;
    private const int targetsPlacing = 3;
    private const int simulating = 4;

    [Header("Private variables")]
    [HideInInspector] public int currentState;
    [HideInInspector] public int previousState;



    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);

        currentState = floorPlacing;
        previousState = 0;

        EnableButton(Button1);
        DisableButton(Button2);
        DisableButton(Button3);
        DisableButton(Button4);

        //At start it search for a file to import
        ImportDataMenu.GetComponent<BehaviourImportData>().SearchProgramData();
    }

    private void OnEnable()
    {
        currentState = floorPlacing;
        previousState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //STATE 1
        if (currentState == floorPlacing)
        {
            if (previousState == 0)
            {
                EnableButton(Button1);
                DisableButton(Button2);
                DisableButton(Button3);
                DisableButton(Button4);
            }
            //Si ya se ha colocado el suelo, se activan el siguiente estado
            if (floorMenu.GetComponent<BehaviourFloorMenu>().previousState == 3)    //ACCEPTDIMENSION   (Cuidado con esto, porque el numero del estado se ha tenido que poner a mano=
            {
                currentState = objectsPlacing;
            }
        }

        //STATE 2
        if (currentState == objectsPlacing)
        {
            //Si el anterior era el del floor, activa los botones y se ejecuta una unica vez
            if (previousState == floorPlacing)
            {
                DisableButton(Button1);
                EnableButton(Button2);
                EnableButton(Button3);
                DisableButton(Button4);
            }

            //Si no se esta moviendo objeto, se puede ir al boton 3
            if (objectMenu.GetComponent<BehaviorObjectMenu>().previousState == 0)       //WAITING
            {
                EnableButton(Button3);
            }
            //Sino, no se puede ir
            else if (objectMenu.GetComponent<BehaviorObjectMenu>().previousState == 1)  //MOVE OBJECT
            {
                DisableButton(Button3);
            }
            //Si se pulsa el boton 3, se pasa al siguiente estado
            if (Button3.GetComponent<InteractionButton>().isPressed)
            {
                currentState = targetsPlacing;
            }
        }

        //STATE 3
        if (currentState == targetsPlacing)
        {
            if (previousState == objectsPlacing)
            {
                DisableButton(Button1);
                EnableButton(Button2);
                EnableButton(Button3);
                DisableButton(Button4);
            }

            //Si no se ha comenzado a colocar los targets, se puede colocar algun objeto
            if (targetMenu.GetComponent<BehaviourTargetMenu>().previousState == 0 || targetMenu.GetComponent<BehaviourTargetMenu>().previousState == 2)   //WAITING or STARTPOINTPLACED
            {
                EnableButton(Button2);
            }
            //Si se ha comenzado a colocar los targets, se puede retornar
            else
            {
                DisableButton(Button2);
            }

            //Cuando se haya colocado el ultimo punto se puede avanzar
            if (targetMenu.GetComponent<BehaviourTargetMenu>().previousState == 4)   //Final Point Placed
            {
                currentState = simulating;
            }
        }

        //STATE 4
        if (currentState == simulating)
        {
            if (previousState == targetsPlacing)
            {
                DisableButton(Button1);
                EnableButton(Button2);
                EnableButton(Button3);
                EnableButton(Button4);
            }

            if (simulationMenu.GetComponent<BehaviourSimulationMenu>().previousState == 0)
            {
                EnableButton(Button2);
                EnableButton(Button3);
            }
            else
            {
                DisableButton(Button2);
                DisableButton(Button3);
            }

        }

        //CASOS DE RESETEO

        // 1. Si se pulsa reset del target menu 
        if (targetMenu.GetComponent<BehaviourTargetMenu>().Reset5.GetComponent<InteractionButton>().isPressed )
        {
            //Se pasa al estado 0 de simulationMenu
            simulationMenu.GetComponent<BehaviourSimulationMenu>().GetButtonNumber(0);
            
            if (movement.transform.childCount > 0)
            {
                Destroy(movement.transform.GetChild(0).gameObject); //Se borran las lineas dibujadas
            }
            currentState = targetsPlacing;
            DisableButton(Button1);
            EnableButton(Button2);
            EnableButton(Button3);
            DisableButton(Button4);
        }

        // 2. Si se borra un objeto o se crea otro nuevo o se importa un programa nuevo, se resetea el mapa y tambien vuelve al estado 0.
        // (EN UN FUTURO SE PODRÁ PONER QUE EL DIBUJO DEL MAPA SEA DINAMICO Y NO HAGA FALTA DARLE A RENDERIZAR MAPA PARA QUE HAGA ALGO)
        if (objectMenu.GetComponent<BehaviorObjectMenu>().Button1.GetComponent<InteractionButton>().isPressed || objectMenu.GetComponent<BehaviorObjectMenu>().programImported)
        {
            objectMenu.GetComponent<BehaviorObjectMenu>().programImported = false; //It resets not to enter another time
            //Se pasa al estado 0 todos los menus
            targetMenu.GetComponent<BehaviourTargetMenu>().GetButtonNumber(0);
            simulationMenu.GetComponent<BehaviourSimulationMenu>().GetButtonNumber(0);
            
            if (movement.transform.childCount > 0)
            {
                Destroy(movement.transform.GetChild(0).gameObject); //Se borran las lineas dibujadas
            }
            // Se pone manualmente los botones
            currentState = objectsPlacing;
            DisableButton(Button1);
            EnableButton(Button2);
            EnableButton(Button3);
            DisableButton(Button4);
        }

        

        previousState = currentState;
    }
    private void DisableButton(GameObject Button)
    {

        Button.GetComponent<InteractionToggle>().controlEnabled = false;
        //Button.transform.Find("Text").gameObject.SetActive(false);            //QUITAR EL TEXTO NO ES NECESARIO AQUI
        //Button.GetComponent<SimpleInteractionGlow>().enabled = false;
        Button.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = OffButtonMaterial;

        for (int i = 1; i < Button.transform.GetChild(0).transform.childCount; i++)
        {
            Button.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void EnableButton(GameObject Button)
    {
        Button.GetComponent<InteractionToggle>().controlEnabled = true;
        //Button.transform.Find("Text").gameObject.SetActive(true);
        //Button.GetComponent<SimpleInteractionGlow>().enabled = true;
        Button.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material = OnButtonMaterial;

        for (int i = 1; i < Button.transform.GetChild(0).transform.childCount; i++)
        {
            Button.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void checkPressed(GameObject Button)
    {
        if (Button.GetComponent<InteractionButton>().isPressed)
        {
            Button.GetComponentInChildren<MeshRenderer>().material = PressedButtonMaterial;
        }
        else
        {
            Button.GetComponentInChildren<MeshRenderer>().material = OnButtonMaterial;
        }
    }
}
