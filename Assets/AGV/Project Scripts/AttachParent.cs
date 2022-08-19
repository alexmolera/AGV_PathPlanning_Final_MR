using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachParent : MonoBehaviour
{
    //Sirve para attachear HAND MENU a Palm UI Pivot Anchor nada mas empezar.
    //De este modo, el panel de control queda mas organizado y todos los menus se encuentran en el mismo sitio

    public GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        menu.transform.parent = gameObject.transform;   //Se atachea al current gameObject
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
