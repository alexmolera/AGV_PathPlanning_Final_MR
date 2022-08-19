using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectBoxFinal : MonoBehaviour
{
    public Material OnMaterial;
    public Material OffMaterial;

    public GameObject warning;

    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 10 ||  other.gameObject.layer == 12)
        {
            BehaviourTargetMenu.placeFinalAllowed = false;
            gameObject.GetComponent<MeshRenderer>().material = OffMaterial;
        }
        else
        {
            BehaviourTargetMenu.placeFinalAllowed = true;
            gameObject.GetComponent<MeshRenderer>().material = OnMaterial;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == 10 || other.gameObject.layer == 12)
        {
            BehaviourTargetMenu.placeFinalAllowed = true;
            gameObject.GetComponent<MeshRenderer>().material = OnMaterial;
        }
        gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10 || other.gameObject.layer == 12)
        {
            BehaviourTargetMenu.placeFinalAllowed = false;
            gameObject.GetComponent<MeshRenderer>().material = OffMaterial;
            warning.SetActive(true);
        }
        else
        {
            BehaviourTargetMenu.placeFinalAllowed = true;
            gameObject.GetComponent<MeshRenderer>().material = OnMaterial;
            warning.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10 || other.gameObject.layer == 12)
        {
            BehaviourTargetMenu.placeFinalAllowed = true;
            warning.SetActive(false);
            gameObject.GetComponent<MeshRenderer>().material = OnMaterial;
        }
    }
}
