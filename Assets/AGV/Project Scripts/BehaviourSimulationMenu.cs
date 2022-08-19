using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using Leap.Unity;
using UnityEngine.UI;

public class BehaviourSimulationMenu : MonoBehaviour
{

    [Header("Buttons")]
    public GameObject Button1;  //Place Starting Point
    public GameObject Button2;  //Confirm Starting Point
    public GameObject Button3;  //Start
    public GameObject Button4;  //Reset
    public GameObject Speed5;   //Speed
    public GameObject Button6;  //Pause
    public GameObject Button7;  //Export
    public GameObject increaseButton;
    public GameObject decreaseButton;

    [Header("Managers and Cubes")]
    public GameObject ManagerScript;
    public GameObject MovementScript;
    public GameObject TimerScript;
    public GameObject CreatedObstacles;
    public GameObject StartPoint;
    public GameObject FinalPoint;

    [Header("Materials On/Off")]
    public Material ButonEnableMaterial;
    public Material ButonDisableMaterial;

    [Header("Robot AGV")]
    public GameObject Robot;

    [Header("Info Display")]
    public GameObject speedText;
    public GameObject NoPathFound;
    public TMPro.TextMeshPro calculationTime;

    [Header("Materials Cube On/Off")]
    public Material CubeOffMaterial;
    public Material cubeONMaterial;

    [Header("States")]
    private const int waiting = 0;                  // Waiting for appearing starting point
    private const int startManhattan = 1;           // waiting for placing starting point
    private const int startEuclidean = 2;
    private const int startMovement = 3;
    private const int pause = 4;
    private const int reset = 5;

    [Header("ID Constants")]    //For the switch function
    private const int ID_BUTTON_1 = 1;  //Start Manhattan
    private const int ID_BUTTON_2 = 2;  //Start Euclidean
    private const int ID_BUTTON_3 = 3;  //Start Movement Button
    private const int ID_BUTTON_4 = 4;  //Reset
    private const int ID_BUTTON_6 = 6;  //Pause

    public static bool movementEnabled = false;

    [HideInInspector] public int currentState;
    [HideInInspector] public int previousState;
    private float increaseValue = 0.01f;   //El valor que incrementa la velocidad
    private float lastSpeed = 0;                //El valor de la ultima velocidad antes de pausar
    private float lastUserSpeed = 0;
    private float lastCalculationTime;          //Last calculation time 
    [HideInInspector] public bool userDetected = false;
    [HideInInspector] public bool endUserDetection = false;
    private float waitTimeUntilNoPath = 0;

    // Start is called before the first frame update
    void Start()
    {
        ManagerScript.SetActive(false);
        MovementScript.SetActive(false);
        ManagerScript.GetComponent<PathFindingDiagonal>().enabled = false;
        ManagerScript.GetComponent<PathFindingManhattan>().enabled = false;
        calculationTime.enabled = false;    //calculation time is disabled

        currentState = waiting;
        previousState = waiting;
        SelectAction(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        //***DEBUG POR MEDIO DE BOTONES***//
        //Hay que desactivar animator de los cubos para poder escalarlos,
        //colocar start y final point a mano y pulsar los botones 1,2,3
        //en la ventana del juego
        if (userDetected && currentState==startMovement)
        {
            MovementScript.GetComponent<RobotMovement>().LinearSpeed = MovementScript.GetComponent<RobotMovement>().LinearSpeed/1.01f;
        } 
        else if(!userDetected && currentState==startMovement)
        {
            if (endUserDetection)
            {
                MovementScript.GetComponent<RobotMovement>().LinearSpeed = lastUserSpeed;
                endUserDetection = false;
            }
            lastUserSpeed = MovementScript.GetComponent<RobotMovement>().LinearSpeed;
        }

        if (Input.GetKey("1"))
        {
            GetButtonNumber(1);
        }
        if (Input.GetKey("2"))
        {
            GetButtonNumber(2);
        }
        if (Input.GetKey("3"))
        {
            GetButtonNumber(3);
        }
        if (Input.GetKey("4"))
        {
            GetButtonNumber(4);
        }
        //********************************//

        //Posiciona el robot en el starting node
        //////////if (currentState == startRenderingMap && ManagerScript.GetComponent<PathFinding>().pathFinded)
        {
            /////////////Robot.transform.position = ManagerScript.GetComponent<PathFinding>().GridReference.FinalPath[0].vPosition;
        }

        //Aparece la velocidad en el panel
        if(currentState == startMovement || currentState == startManhattan || currentState == startEuclidean)
        {
            speedText.GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(MovementScript.GetComponent<RobotMovement>().LinearSpeed * 1000) / 10f);
        }

        else if (currentState == pause)
        {
            speedText.GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(lastSpeed * 1000) / 10f);  
        }

        // 1. Si no hay camino
        //Se pone la senal de alerta y se indica
        if (currentState == startEuclidean)
        {
            //Esto es para no activar el boton de spped de primeras y que espere un poco a ver si hay camino
            if ((Time.time - waitTimeUntilNoPath) < 0.7)
            {
                DisableButton(Button3);
                Robot.SetActive(false); 
                //DisableButton(Button7);
            }

            //Si hay camino lo habilita
            else
            {
                EnableButton(Button3);
                Robot.SetActive(true);
                //EnableButton(Button7);
            }

            if (ManagerScript.GetComponent<PathFindingDiagonal>().enabled && (Time.time - waitTimeUntilNoPath) > 0.7)  //Tiempo de margen hasta que no encuentra camino
            {
                NoPathFound.SetActive(true);
                DisableButton(Button1);
                DisableButton(Button2);
                DisableButton(Button3);
                EnableButton(Button4);
                DisableButton(Speed5);
                DisableButton(Button6);
                DisableButton(Button7);
                increaseButton.SetActive(false);
                decreaseButton.SetActive(false);

                Robot.SetActive(false);
            }

            if (!ManagerScript.GetComponent<PathFindingDiagonal>().enabled && !calculationTime.enabled)    //Si ha encontrado camino, el script se apaga, por lo que aqui se comprueba si lo ha encontrado
            {
                calculationTime.text = "" + ((int)(Time.time * 1000) - (int)lastCalculationTime);
                calculationTime.enabled = true;
            }
        }

        if (currentState == startManhattan)
        {
            //Esto es para no activar el boton de spped de primeras y que espere un poco a ver si hay camino
            if ((Time.time - waitTimeUntilNoPath) < 0.7)
            {
                DisableButton(Button3);
                Robot.SetActive(false);
                //DisableButton(Button7);
            }
                
            else
            {
                EnableButton(Button3);
                Robot.SetActive(true);
                //EnableButton(Button7);
            }
                

            if  (ManagerScript.GetComponent<PathFindingManhattan>().enabled && (Time.time - waitTimeUntilNoPath) > 0.7)
            {
                NoPathFound.SetActive(true);
                DisableButton(Button1);
                DisableButton(Button2);
                DisableButton(Button3);
                EnableButton(Button4);
                DisableButton(Speed5);
                DisableButton(Button6);
                DisableButton(Button7);
                increaseButton.SetActive(false);
                decreaseButton.SetActive(false);

                Robot.SetActive(false);
            }

            if (!ManagerScript.GetComponent<PathFindingManhattan>().enabled && !calculationTime.enabled)    //Si ha encontrado camino, el script se apaga, por lo que aqui se comprueba si lo ha encontrado
            {
                calculationTime.text = "" + ((int)(Time.time * 1000) - (int)lastCalculationTime);
                calculationTime.enabled = true;
            }
        }

        if(currentState != waiting)
        {
            StartPoint.GetComponent<MeshRenderer>().enabled = false;
            FinalPoint.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            StartPoint.GetComponent<MeshRenderer>().enabled = true;
            FinalPoint.GetComponent<MeshRenderer>().enabled = true;
            StartPoint.GetComponent<MeshRenderer>().material = cubeONMaterial;
            FinalPoint.GetComponent<MeshRenderer>().material = cubeONMaterial;
        }
        
        if(currentState == startMovement || currentState == pause)
        {
            if (!MovementScript.GetComponent<RobotMovement>().followingPath)
            {
                EnableButton(Button7);
            }
            else
            {
                DisableButton(Button7);
            }
        }
    }

    public void GetButtonNumber(int button_id)
    {
        switch (button_id)
        {
            case 0:
                currentState = waiting;
                break;

            //Button 1: 
            case ID_BUTTON_1:            
                    currentState = startManhattan;               
                break;

            //Button 2: 
            case ID_BUTTON_2:
                    currentState = startEuclidean;  
                break;
            //Moving
            case ID_BUTTON_3: 
                    currentState = startMovement;
                break;
            //Reset
            case ID_BUTTON_4:
                currentState = reset;
                break;

            case ID_BUTTON_6:
                if(previousState == startMovement )
                {
                    currentState = pause;
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
                EnableButton(Button2);
                DisableButton(Button3);
                DisableButton(Button4);
                DisableButton(Speed5);
                DisableButton(Button6);
                DisableButton(Button7);
                increaseButton.SetActive(false);
                decreaseButton.SetActive(false);
                NoPathFound.SetActive(false);
                calculationTime.enabled = false;    //calculation time is disabled

                ManagerScript.SetActive(false);
                ManagerScript.GetComponent<PathFindingDiagonal>().enabled = false;
                ManagerScript.GetComponent<PathFindingManhattan>().enabled = false;
                MovementScript.SetActive(false);
                TimerScript.GetComponent<Timer>().ResetTime();  //TODO LO RELACIONADO CON EL TIMER DEBERIA HACERSE EN ESTE SCRIPT
                Robot.SetActive(false);

                //Green Color on 
                StartPoint.GetComponent<DetectBoxStart>().enabled = true;
                FinalPoint.GetComponent<DetectBoxFinal>().enabled = true;
                StartPoint.GetComponent<MeshRenderer>().material = cubeONMaterial;
                FinalPoint.GetComponent<MeshRenderer>().material = cubeONMaterial;

                waitTimeUntilNoPath =0;

                previousState = waiting;

                break;

            case startManhattan:
                DisableButton(Button1);
                EnableButton(Button2);
                EnableButton(Button3);
                EnableButton(Button4);
                EnableButton(Speed5);
                DisableButton(Button6);
                DisableButton(Button7);
                increaseButton.SetActive(true);
                decreaseButton.SetActive(true);

                //Guarda valor de tiempo
                lastCalculationTime = Time.time * 1000;
                calculationTime.enabled = false;    //calculation time is disabled

                //El Manager script crea el grid y el camino de NODOS
                ManagerScript.SetActive(true);                      //Aqui renderiza el mapa y crea el grid
                ManagerScript.GetComponent<PathFindingDiagonal>().enabled = false;
                ManagerScript.GetComponent<PathFindingManhattan>().enabled = true;
                ManagerScript.GetComponent<Grid>().MakeGrid();      //Crea Grid

                //Activa Robot 
                Robot.SetActive(true);

                //Activa contador para comprobar si no hay camino
                waitTimeUntilNoPath = Time.time;

                //Activa Movement
                MovementScript.GetComponent<RobotMovement>().LinearSpeed = MovementScript.GetComponent<RobotMovement>().defaultSpeed;
                MovementScript.SetActive(true);

                previousState = startManhattan;

                break;

            case startEuclidean:
                EnableButton(Button1);
                DisableButton(Button2);
                EnableButton(Button3);
                EnableButton(Button4);
                EnableButton(Speed5);
                DisableButton(Button6);
                DisableButton(Button7);
                increaseButton.SetActive(true);
                decreaseButton.SetActive(true);

                //Guarda valor de tiempo
                lastCalculationTime = Time.time * 1000;
                calculationTime.enabled = false;    //calculation time is disabled

                //El Manager script crea el grid y el camino de NODOS
                ManagerScript.SetActive(true);                      //Aqui renderiza el mapa y crea el grid
                ManagerScript.GetComponent<PathFindingDiagonal>().enabled = true;
                ManagerScript.GetComponent<PathFindingManhattan>().enabled = false;
                ManagerScript.GetComponent<Grid>().MakeGrid();      //Crea Grid

                //Activa Robot
                Robot.SetActive(true);

                //Activa contador para comprobar si no hay camino
                waitTimeUntilNoPath = Time.time;

                //Activa Movement
                MovementScript.GetComponent<RobotMovement>().LinearSpeed = MovementScript.GetComponent<RobotMovement>().defaultSpeed;
                MovementScript.SetActive(true);

                previousState = startEuclidean;

                break;

            case startMovement:
                DisableButton(Button1);
                DisableButton(Button2);
                DisableButton(Button3);
                EnableButton(Button4);
                EnableButton(Speed5);
                EnableButton(Button6);
                DisableButton(Button7);
                increaseButton.SetActive(true);
                decreaseButton.SetActive(true);

                // Si viene de pausa se ejecutan estas rutinas
                if(previousState == pause)
                {
                    MovementScript.GetComponent<RobotMovement>().LinearSpeed = lastSpeed;
                    //If the robot is stopped an pause is pressed there´s no time to count
                    if (MovementScript.GetComponent<RobotMovement>().followingPath)
                    {
                        TimerScript.GetComponent<Timer>().ResumeCountTime();
                    }
                    
                }
                else
                {
                    MovementScript.GetComponent<RobotMovement>().startMovement();
                    TimerScript.GetComponent<Timer>().StartCountTime();
                }
                
                previousState = startMovement;
                break;

            case pause:
                DisableButton(Button1);
                DisableButton(Button2);
                EnableButton(Button3);
                EnableButton(Button4);
                EnableButton(Speed5);
                DisableButton(Button6);
                DisableButton(Button7);
                increaseButton.SetActive(true);
                decreaseButton.SetActive(true);

                lastSpeed = MovementScript.GetComponent<RobotMovement>().LinearSpeed;
                MovementScript.GetComponent<RobotMovement>().LinearSpeed = 0;
                TimerScript.GetComponent<Timer>().StopCountTime();

                previousState = pause;

                break;

            case reset:

                // Resetea todos los gameobjects y hace desaparecer robot
                ManagerScript.SetActive(false);
                MovementScript.SetActive(false);
                TimerScript.GetComponent<Timer>().ResetTime();
                Robot.SetActive(false);

                previousState = waiting;

                //PROBAR: GetButtonNumber(2);

                // Y hace de nuevo el renderizado automaticamente, coloca al robot en 0. etc. 
                GetButtonNumber(0);

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
     

    void AppearRobot()
    {
        //Los puntos se hacen invisibles
        StartPoint.GetComponent<MeshRenderer>().material = CubeOffMaterial;
        FinalPoint.GetComponent<MeshRenderer>().material = CubeOffMaterial;

        Robot.SetActive(true);
        //Robot.transform.position = StartPoint.transform.position;
        //Robot.transform.position = new Vector3  ()
    }

    public void IncreaseSpeed()
    {
        //Dependiendo del estado, a veces se cambia la velocidad en parado o en movimiento
        if (previousState == pause)
        {
            if ((lastSpeed + increaseValue) < 0.21)
            {
                lastSpeed += increaseValue;
            }
        }

        else if (previousState != pause && (MovementScript.GetComponent<RobotMovement>().LinearSpeed + increaseValue) < 0.21)
        {
            MovementScript.GetComponent<RobotMovement>().LinearSpeed += increaseValue;
        }
        // The angular speed is updated at the same time as the linear one
        // MovementScript.GetComponent<RobotMovement>().RotSpeedRad = (float)(-0.625 * Mathf.Pow(MovementScript.GetComponent<RobotMovement>().LinearSpeed,2) + 5.625 * MovementScript.GetComponent<RobotMovement>().LinearSpeed -0.05);
    }

    public void DecreaseSpeed()
    {
        if (previousState == pause)
        {
            if ((lastSpeed - increaseValue) > 0.00)
            {
                lastSpeed -= increaseValue;
            }
        }

        else if (previousState != pause && (MovementScript.GetComponent<RobotMovement>().LinearSpeed - increaseValue) > 0.00)
        {
            MovementScript.GetComponent<RobotMovement>().LinearSpeed -= increaseValue;
        }
        // The angular speed is updated at the same time as the linear one
        // MovementScript.GetComponent<RobotMovement>().RotSpeedRad = (float)(-0.625 * Mathf.Pow(MovementScript.GetComponent<RobotMovement>().LinearSpeed, 2) + 5.625 * MovementScript.GetComponent<RobotMovement>().LinearSpeed - 0.05);
    }
}