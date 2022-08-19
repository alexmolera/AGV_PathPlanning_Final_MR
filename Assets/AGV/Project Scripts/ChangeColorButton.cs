using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using Leap.Unity;
using UnityEngine.UI;

public class ChangeColorButton : MonoBehaviour
{
    private InteractionToggle Button;
    public MeshRenderer buttonMaterial;
    public Material Green;
    public Material Red;
    // Start is called before the first frame update
    private void Awake()
    {
        Button = gameObject.GetComponent<InteractionToggle>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Button.isToggled)
        {
            buttonMaterial.material = Green;
        }
        else
            buttonMaterial.material = Red;

    }
}
