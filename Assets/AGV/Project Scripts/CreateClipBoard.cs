using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateClipBoard : MonoBehaviour
{
    public GameObject clipboard;
    public Transform trans;
    public Vector3 offSetPos = new Vector3(-0.025f, -0.1795f, 0.0738f);
    public Quaternion offSetRot = new Quaternion(-79.248f, 57.918f, 52.085f, 1f);
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        //DEBUG DE POSICIONAMIENTO
        //clipboard.transform.rotation = trans.rotation * offSetRot;
        //clipboard.transform.position = trans.position - offSetPos;
    }

    public void Appear()
    {
        clipboard.SetActive(true);
        clipboard.transform.parent = trans.transform;

        clipboard.transform.localPosition = offSetPos;
        clipboard.transform.localRotation = offSetRot;

        clipboard.transform.parent = null;
    }

    public void Disappear()
    {
        clipboard.SetActive(false);
    }
}
