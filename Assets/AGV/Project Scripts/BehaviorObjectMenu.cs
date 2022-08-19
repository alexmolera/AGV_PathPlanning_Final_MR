using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using Leap.Unity;
using UnityEngine.UI;


public class BehaviorObjectMenu : MonoBehaviour
{

    public static int createdObstaclesNum = 0;
    [HideInInspector] public bool programImported = false;

    [Header("Buttons and Dimension Manager")]
    public GameObject Button1;  //Place Object
    public GameObject Button2;  //Confirm object
    public GameObject Slider3;  //Width dimension
    public GameObject Slider4;  //Length dimension
    public GameObject Slider5;  //Rotation
    public GameObject Button6;  //Delete Cubes
    public GameObject Button7;
    public GameObject DimensionCubeManager;
    //public GameObject Reset5;   //Reset Button

    [Header("Text Prefab")]
    public GameObject textPrefab;   //Text where appear created objects
    public GameObject textPrefabParent;

    [Header("Create Object & Import Data")]
    public GameObject CreateObjMenu;   
    public GameObject ImportMenu;

    [Header("Cube and its Parent")]
    public GameObject cubePrefab;
    public GameObject parentCubePrefab;

    [Header("Laser")]
    public GameObject laserLineLeft;
    public GameObject laserLineRight;

    [Header("RTS, Floor and CreatedObstacles folder")]
    public GameObject rtsParent;
    public GameObject floorPlane;
    public GameObject createdObstacles;

    [Header("Materials On/Off")]
    public Material ButonEnableMaterial;
    public Material ButonDisableMaterial;
    public Material CubePlacedMaterial;

    [Header("Debug Log text")]
    public GameObject debugLog;

    [Header("States")]
    private const int waiting = 0;
    private const int moveObject = 1;
    private const int placedObject = 2;


    [Header("ID Constants")]            //For the switch function
    private const int ID_BUTTON_1 = 1;  //Start Render Button
    private const int ID_BUTTON_2 = 2;  //Confirm Button
    private const int ID_BUTTON_3 = 3;  //Dimension X Plane
    private const int ID_BUTTON_4 = 4;  //Dimension Z Plane
    private const int ID_BUTTON_5 = 5;  //Reset Button
    private const int ID_BUTTON_6 = 6;  //Delete Button
    private const int ID_BUTTON_7 = 7;  //Import Data open

    [HideInInspector] public int currentState;
    [HideInInspector] public int previousState;

    private Vector3 initialScale;
    private Quaternion initialRotation;
    private Animator animator;
    private float positionX;    //posicion X del cubo cuando se desattachea
    private float positionZ;    //posicion Z ''

    private float timeTemp;
    public GameObject handGIF;


    //Start is called before the first frame update
    void Start()
    {
        cubePrefab.SetActive(false);
        rtsParent.SetActive(false);
        DimensionCubeManager.SetActive(false);

        CreateObjMenu.SetActive(true);
        ImportMenu.SetActive(false); 

        initialScale = new Vector3(0.1f, 0.5f, 0.1f);   //Change this if the cube has different Height
        initialRotation = cubePrefab.transform.rotation;
        animator = cubePrefab.GetComponent<Animator>();
        textPrefab.transform.parent = null;

        currentState = waiting;
        previousState = waiting;
        SelectAction(currentState);
    }

    private void OnEnable()
    {
        timeTemp = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("New Animation Cube"))   //Compruba si se esta ejecutando la animacion para desactivarla porque daba problemas con poder escalar luego el cubo
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            cubePrefab.GetComponent<Animator>().enabled = false;

            if (currentState == moveObject)
            {
                EnableButton(Button2);
            }
        }
        if (createdObstacles.transform.childCount > 0)
        {
            EnableButton(Button6);
        }
        else
        {
            DisableButton(Button6);
        }

        if (gameObject.activeInHierarchy && Time.time - timeTemp < 2.8)
        {
            handGIF.SetActive(true);
        }
        else
        {
            handGIF.SetActive(false);
        }
    }

    public void GetButtonNumber(int button_id)
    {
        switch (button_id)
        {
            //Button 1: Place cube. Se necesita confirmación tras posicionamiento.
            case 0:
                    currentState = waiting;
                break;

            case ID_BUTTON_1:
                if (previousState == waiting)
                {
                    currentState = moveObject;
                }
                break;

            //Button 2: confirmar cambios. Por lo tanto,el estado siguiente dependerá del anterior
            case ID_BUTTON_2:
                if (previousState == moveObject)
                {
                    currentState = waiting;
                }
                break;

            case ID_BUTTON_3:
                break;

            case ID_BUTTON_4:
                break;

            case ID_BUTTON_5:
                break;

            case ID_BUTTON_6:   //Borra el cubo y decrementa cuenta de cubos creados
                DeleteCube();
                break;

            case ID_BUTTON_7:

                break;

            default:
                Debug.Log("Error in: GetButtonNumber/switch");
                break;
        }
        SelectAction(currentState);
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
                DisableSlider(Slider5);
                DisableButton(Button6);
                EnableButton(Button7);
                DisableDimension();

                if (previousState == moveObject)
                {
                    ResetSliderValues(Slider3, Slider4, Slider5);
                    DettachCube();
                    CreateText();

                }   //En caso de que se reinicie, los valores del slider vuelven a 0.

                if (createdObstacles.transform.childCount > 0)
                {
                    EnableButton(Button6);
                }
            
                previousState = waiting;    //Esto realmente podria ir al final

                break;

            case moveObject:
                DisableButton(Button1);
                //Enable Button 2 ahora esta en el update
                EnableSlider(Slider3);
                EnableSlider(Slider4);
                EnableSlider(Slider5);
                DisableButton(Button6);
                DisableButton(Button7);
                EnableDimension();

                AppearCube();

                if (createdObstacles.transform.childCount == 0)
                {
                    PrintDebugLog("Moving cube.\nPinch with both hands to start moving the object.\nScale and rotate the object with sliders.");
                }
                else
                {
                    PrintDebugLog("Moving cube.");
                }

                previousState = moveObject;

                break;

            case placedObject:
                break;

                //******************************************************************
                //Falta añadir boton de borrar cubos
                //******************************************************************
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

    private void ResetSliderValues(GameObject Slider3, GameObject Slider4, GameObject Slider5)
    {
        Slider3.GetComponent<InteractionSlider>().HorizontalSliderValue = Slider3.GetComponent<InteractionSlider>().defaultHorizontalValue;
        Slider4.GetComponent<InteractionSlider>().HorizontalSliderValue = Slider4.GetComponent<InteractionSlider>().defaultHorizontalValue;
        Slider5.GetComponent<InteractionSlider>().VerticalSliderValue = Slider5.GetComponent<InteractionSlider>().defaultVerticalValue;
    }

    public void AppearCube()
    {

        // cubePrefab reset
        cubePrefab.SetActive(true);
        cubePrefab.transform.parent = parentCubePrefab.transform;                       //*******//PARENT: CUBE PARENT
        cubePrefab.transform.localScale = initialScale;
        cubePrefab.transform.rotation = initialRotation;

        // cubePrefab positioning in 0,0 on the plane. Also animating appearence.
        cubePrefab.transform.parent = floorPlane.transform;                             //*******//PARENT: FLOOR
        cubePrefab.transform.localPosition = new Vector3(0, initialScale.y / 2, 0);

        //Se le asigna el laser para que moverlo
        laserLineLeft.transform.parent = null;
        laserLineRight.transform.parent = null;
        laserLineLeft.GetComponent<Electric>().transformPointB = cubePrefab.transform;
        laserLineRight.GetComponent<Electric>().transformPointB = cubePrefab.transform;

        //Animacion de entrada
        cubePrefab.GetComponent<Animator>().enabled = true;
        animator.SetTrigger("AnimateCube");

        //animator.ResetTrigger("AnimateCube");                                           // Animación al aparecer

        //cubePrefab.transform.localRotation = Quaternion.Euler(0, 0, 0);               //***Comprobar esto si no funciona el angulo 0*****

        // cubePrefab moving and rotating over the plane
        rtsParent.SetActive(true);
        cubePrefab.transform.parent = rtsParent.transform;                              //*******//PARENT: RTS
        ActivateRTS();
    }

    //When the positioning is finished, an instantiated object is placed over the cube Prefab.
    public void DettachCube()
    {
        // Se desactiva RTS
        DeactivateRTS();

        // Se asigna padre: suelo
        cubePrefab.transform.parent = floorPlane.transform;
        positionX = cubePrefab.transform.localPosition.x;          //Esto se utiliza en la funcion CreateText
        positionZ = cubePrefab.transform.localPosition.z;

        // Se guardan las coordenadas respecto al suelo
        GameObject cubeInstantiated;
        //Vector3 farPosition = new Vector3(20f, 20f, 20f);   //PROBAR QUITAR ESTO
        Vector3 realPosition = cubePrefab.transform.localPosition;
        Quaternion realRotation = cubePrefab.transform.rotation;

        // Se duplica el obstaculo
        //cubePrefab.transform.position = farPosition;        //PROBAR QUITAR ESTO
        cubePrefab.SetActive(false);

        cubeInstantiated = Instantiate(cubePrefab) as GameObject;
        cubeInstantiated.transform.parent = floorPlane.transform;

        // Se posiciona donde antes estaba el cubo de plantilla
        cubeInstantiated.SetActive(true);
        cubeInstantiated.transform.localPosition = realPosition;
        cubeInstantiated.transform.rotation = realRotation;

        //Se apaga el gameObject
        rtsParent.SetActive(false);

        // Se guarda en la lista de objetos creados y cada uno tendrá un índice. Se cambia material tambien
        cubeInstantiated.transform.parent = createdObstacles.transform;
        createdObstaclesNum++;
        cubeInstantiated.name = "Obstacle " + createdObstaclesNum;
        cubeInstantiated.GetComponent<MeshRenderer>().material = CubePlacedMaterial;
    }

    public void ActivateRTS()
    {
        LeapRTS.RTSenabled = true;
    }

    public void DeactivateRTS()
    {
        LeapRTS.RTSenabled = false;
    }

    public void EnableDimension()
    {
        DimensionCubeManager.SetActive(true);
    }

    public void DisableDimension()
    {
        DimensionCubeManager.SetActive(false);
    }

    public void CreateText()
    {
        GameObject textObstacle;

        textObstacle = Instantiate(textPrefab, textPrefabParent.transform);
        textObstacle.transform.GetChild(1).GetComponent<TMPro.TextMeshPro>().text = "" + (createdObstaclesNum);                    //Obstacle Number
        textObstacle.transform.GetChild(2).GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(cubePrefab.transform.localScale.x * 10) / 10f) + " m";   //Width
        textObstacle.transform.GetChild(3).GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(cubePrefab.transform.localScale.z * 10) / 10f) + " m";   //Length

        // X and Z Components
        cubePrefab.transform.parent = floorPlane.transform; //Para obtener la componente de rotacion en base al suelo
        textObstacle.transform.GetChild(4).GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(cubePrefab.transform.localEulerAngles.y)) + "°";   //Yaw
        textObstacle.transform.GetChild(5).GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(positionX * 10) / 10f);     //X con 1 decimal
        textObstacle.transform.GetChild(6).GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(positionZ * 10) / 10f);     //Z con 1 decimal
        cubePrefab.transform.parent = parentCubePrefab.transform;   //Se vuelve a dejar como padre al que tenia al principio

        textObstacle.name = "Obstacle Text" + (createdObstaclesNum);
        PrintDebugLog("Cube placed at:" + " \tx: " + (Mathf.Round(positionX * 10) / 10f) + "\tz: " + (Mathf.Round(positionZ * 10) / 10f) + "\nWidth: " + (Mathf.Round(cubePrefab.transform.localScale.x * 10) / 10f) + " m" +"\tLenght " + (Mathf.Round(cubePrefab.transform.localScale.z * 10) / 10f) + " m" + "\tJaw: " + (Mathf.Round(cubePrefab.transform.localEulerAngles.y)) + "°");

        //Se posiciona. CUIDADO CON OFFSET, CAMBIAR SI HACE FALTA
        textObstacle.SetActive(true);
        textObstacle.transform.localPosition = new Vector3(-0.0219f, -0.1f + (-0.016f * (createdObstaclesNum - 1)), -0.003f);    //los numeros son las coordenadas iniciales del primer texto
        textObstacle.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void DeleteCube()
    {
            textPrefabParent.transform.GetChild(textPrefabParent.transform.childCount - 1).gameObject.transform.position = new Vector3(100, 100, 100);
            textPrefabParent.transform.GetChild(textPrefabParent.transform.childCount - 1).gameObject.name = "basura no borrable";
            textPrefabParent.transform.GetChild(textPrefabParent.transform.childCount - 1).gameObject.transform.parent = null; // no se por que no dejaba borrarlo asi que lo he mandado a otra galaxia y los he desvinculado del padre

            Destroy(createdObstacles.transform.GetChild(createdObstacles.transform.childCount - 1).gameObject);     

            //PrintDebugLog("Cube deleted.");

            createdObstaclesNum--;
    }

    void PrintDebugLog(string n)
    {
        debugLog.GetComponent<TMPro.TextMeshPro>().text = "[" + System.DateTime.UtcNow.ToString("HH:mm:ss") + "]" + " " + n + "\n\n" + debugLog.GetComponent<TMPro.TextMeshPro>().text;
    }

    //IMPORT DATA METHODS. POLIMORFISM IN DETACHCUBE() AND CREATETEXT()
    public void DettachCube(string _name, Vector3 _pos, Vector3 _rot, Vector3 _scale)
    {
        // Se desactiva RTS
        DeactivateRTS();

        // Se guardan las coordenadas respecto al suelo
        GameObject cubeInstantiated;
        
        // Se duplica el obstaculo
        //cubePrefab.transform.position = farPosition;        //PROBAR QUITAR ESTO
        cubePrefab.SetActive(false);
        cubeInstantiated = Instantiate(cubePrefab) as GameObject;
        cubeInstantiated.GetComponent<Animator>().enabled = false;
        cubeInstantiated.transform.parent = createdObstacles.transform;

        // Se posiciona donde antes estaba el cubo de plantilla
        cubeInstantiated.SetActive(true);
        cubeInstantiated.transform.localPosition = _pos;
        cubeInstantiated.transform.localEulerAngles = _rot;
        cubeInstantiated.transform.localScale = _scale;

        //Se apaga el laser
        rtsParent.SetActive(false);

        // Se guarda en la lista de objetos creados y cada uno tendrá un índice. Se cambia material tambien   
        createdObstaclesNum++;

        // Se llama a CreateText()
        CreateText(cubeInstantiated, _scale);

        cubeInstantiated.name = "Obstacle " + createdObstaclesNum;
        cubeInstantiated.GetComponent<MeshRenderer>().material = CubePlacedMaterial;
    }
    public void CreateText(GameObject cube, Vector3 _scale)
    {
        GameObject textObstacle;

        textObstacle = Instantiate(textPrefab, textPrefabParent.transform);
        textObstacle.transform.GetChild(1).GetComponent<TMPro.TextMeshPro>().text = "" + (createdObstaclesNum);                    //Obstacle Number
        textObstacle.transform.GetChild(2).GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(_scale[0] * 10) / 10f) + " m";   //Width
        textObstacle.transform.GetChild(3).GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(_scale[2] * 10) / 10f) + " m";   //Length

        // X and Z Components
        cube.transform.parent = floorPlane.transform; //Para obtener la componente de rotacion en base al suelo
        textObstacle.transform.GetChild(4).GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(cube.transform.localEulerAngles.y)) + "°";   //Yaw
        textObstacle.transform.GetChild(5).GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(cube.transform.localPosition.x * 10) / 10f);     //X con 1 decimal
        textObstacle.transform.GetChild(6).GetComponent<TMPro.TextMeshPro>().text = "" + (Mathf.Round(cube.transform.localPosition.z * 10) / 10f);     //Z con 1 decimal

        textObstacle.name = "Obstacle Text" + (createdObstaclesNum);
        PrintDebugLog("Cube placed at:" + " \tx: " + (Mathf.Round(cube.transform.localPosition.x * 10) / 10f) + "\tz: " + (Mathf.Round(cube.transform.localPosition.z * 10) / 10f) + "\nWidth: " + (Mathf.Round(cube.transform.localScale.x * 10) / 10f) + " m" + "\tLenght " + (Mathf.Round(cube.transform.localScale.z * 10) / 10f) + " m" + "\tJaw: " + (Mathf.Round(cube.transform.localEulerAngles.y)) + "°");

        //Se vuelve a dejar en created objects
        cube.transform.parent = createdObstacles.transform;

        //Se posiciona. CUIDADO CON OFFSET, CAMBIAR SI HACE FALTA
        textObstacle.SetActive(true);
        textObstacle.transform.localPosition = new Vector3(-0.0219f, -0.1f + (-0.016f * (createdObstaclesNum - 1)), -0.003f);    //los numeros son las coordenadas iniciales del primer texto
        textObstacle.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public void ProgramImported()
    {
        programImported = true;
    }
}



