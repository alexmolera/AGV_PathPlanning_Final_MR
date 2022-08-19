using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectUser : MonoBehaviour
{

    public GameObject warning;
    public BehaviourSimulationMenu simulationScript;
    // Start is called before the first frame update
    void Start()
    {
        warning.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 16)
        {
            simulationScript.userDetected = true;
            warning.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 16)
        {

            warning.SetActive(false);
            simulationScript.userDetected = false;
            simulationScript.endUserDetection = true;

        }
    }

}
