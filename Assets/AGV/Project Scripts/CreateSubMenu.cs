using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class CreateSubMenu : MonoBehaviour
{
    [Header("Submenu")]
    public GameObject submenu;
    public GameObject buttonParent;

    [Header("Move Button")]
    public GameObject moveButton;
    public Material OffMaterial;
    public Material OnMaterial;

    public Vector3 offSetPos = new Vector3(-0.025f, -0.1795f, 0.0738f);
    public Quaternion offSetRot = new Quaternion(-79.248f, 57.918f, 52.085f, 1f);

    private bool submenuAttached;
    // Start is called before the first frame update
    void Start()
    {
        submenu.SetActive(false);
    }

    void Update()
    {
        //De este modo, en cuanto se coja el menu, se desvincula con el menu principal//
        if (submenu.GetComponent<InteractionBehaviour>().isGrasped)                 //Se ilumina al agarrar menu
        {
            Leap.Unity.LeapRTS.RTSenabled = false;                              //Deactivate RTS
            moveButton.GetComponent<MeshRenderer>().material = OnMaterial;
            if (submenuAttached)                                                //Desvincula del padre
            {
                submenu.transform.parent = null;
                submenuAttached = false;
            }
        }
        else
        {
            Leap.Unity.LeapRTS.RTSenabled = true;                               //Activate RTS
            moveButton.GetComponent<MeshRenderer>().material = OffMaterial;
        }

        //DEBUG DE POSICIONAMIENTO
        //submenu.transform.localPosition = offSetPos;
        //submenu.transform.localRotation = offSetRot;

    }

    public void Appear()
    {
        submenu.SetActive(true);
        submenu.transform.parent = buttonParent.transform;
        
           if (!submenuAttached) 
           {
                submenu.transform.localPosition = offSetPos;
                submenu.transform.localRotation = offSetRot;
                submenuAttached = true;
           }
            

    }

    public void Disappear()
    {
        submenu.SetActive(false);
        submenuAttached = false;
    }
}
