using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class BehaviourClipboard : MonoBehaviour
{

    public GameObject clipboard_1_1;
    public GameObject clipboard_1_2;
    public GameObject clipboard_2;
    public GameObject clipboard_3;

    public GameObject buttonNext;
    public GameObject buttonPrev;

    [Header("Materials On/Off")]
    public Material ButonEnableMaterial;
    public Material ButonDisableMaterial;

    [HideInInspector] public int currentState;
    [HideInInspector] public int previousState;

    [Header("ID Constants")]    //For the switch function
    private const int ID_BUTTON_1 = 1;  //Next Button
    private const int ID_BUTTON_2 = 2;  //Prev Button

    [Header("States")]
    private const int clipboard_1_1_state = 0;
    private const int clipboard_1_2_state = 1;
    private const int clipboard_2_state = 2;
    private const int clipboard_3_state = 3;



    // Start is called before the first frame update
    void Start()
    {
        currentState = clipboard_1_1_state;
        previousState = clipboard_1_1_state;

        SelectAction(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetButtonNumber(int button_id)
    {
        switch (button_id)
        {
            //Button 1: comienza a trackear el marker. Se necesita confirmación para dejar el suelo estático y dejar de trackear.
            case ID_BUTTON_1:
                switch (previousState)
                {
                    case clipboard_1_1_state:
                        currentState = clipboard_1_2_state;
                    break;
                    case clipboard_1_2_state:
                        currentState = clipboard_2_state;
                        break;
                    case clipboard_2_state:
                        currentState = clipboard_3_state;
                        break;
                    case clipboard_3_state:
                        //Nothing
                        break;
                }
                break;

            //Button 2: confirmar cambios. Por lo tanto,´el estado siguiente dependerá del anterior
            case ID_BUTTON_2:
                switch (previousState)
                {
                    case clipboard_1_1_state:
                        //Nothing
                        break;
                    case clipboard_1_2_state:
                        currentState = clipboard_1_1_state;
                        break;
                    case clipboard_2_state:
                        currentState = clipboard_1_2_state;
                        break;
                    case clipboard_3_state:
                        currentState = clipboard_2_state;
                        break;
                }
                break;
        }
        SelectAction(currentState); //
    }

    private void SelectAction(int currentState)
    {
        switch (currentState)
        {
            case clipboard_1_1_state:
                DisableButton(buttonPrev);
                EnableButton(buttonNext);

                clipboard_1_1.SetActive(true);
                clipboard_1_2.SetActive(false);
                clipboard_2.SetActive(false);
                clipboard_3.SetActive(false);

                previousState = clipboard_1_1_state;    //Esto realmente podria ir al final

                break;

            case clipboard_1_2_state:
                EnableButton(buttonPrev);
                EnableButton(buttonNext);

                clipboard_1_1.SetActive(false);
                clipboard_1_2.SetActive(true);
                clipboard_2.SetActive(false);
                clipboard_3.SetActive(false);

                previousState = clipboard_1_2_state;

                break;

            case clipboard_2_state:
                EnableButton(buttonPrev);
                EnableButton(buttonNext);

                clipboard_1_1.SetActive(false);
                clipboard_1_2.SetActive(false);
                clipboard_2.SetActive(true);
                clipboard_3.SetActive(false);

                previousState = clipboard_2_state;

                break;

            case clipboard_3_state:
                EnableButton(buttonPrev);
                DisableButton(buttonNext);

                clipboard_1_1.SetActive(false);
                clipboard_1_2.SetActive(false);
                clipboard_2.SetActive(false);
                clipboard_3.SetActive(true);

                previousState = clipboard_3_state;
                break;


                //******************************************************************
                //Falta añadir boton de restart o de error
                //******************************************************************
        }
    }

    private void DisableButton(GameObject Button)
    {

        Button.GetComponent<InteractionButton>().controlEnabled = false;
        //Button.transform.Find("Text").gameObject.SetActive(false);
        //Button.GetComponent<SimpleInteractionGlow>().enabled = false;
        Button.GetComponentInChildren<MeshRenderer>().material = ButonDisableMaterial;

    }

    private void EnableButton(GameObject Button)
    {
        Button.GetComponent<InteractionButton>().controlEnabled = true;
        //Button.transform.Find("Text").gameObject.SetActive(true);
        //Button.GetComponent<SimpleInteractionGlow>().enabled = true;
        Button.GetComponentInChildren<MeshRenderer>().material = ButonEnableMaterial;
    }
}
