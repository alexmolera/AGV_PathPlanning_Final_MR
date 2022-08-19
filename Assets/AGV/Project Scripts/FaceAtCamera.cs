using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAtCamera : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject mainCamera;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(mainCamera.transform);
    }
}
