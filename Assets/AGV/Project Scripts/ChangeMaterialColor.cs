using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialColor : MonoBehaviour
{

    public List<Material> materials;
    public List<Material> materialsCopy;    //It will be restored at the end of the program
    bool red = false;

    private void Awake()
    {
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].color = materialsCopy[i].color;
            materials[i].shader= materialsCopy[i].shader;
        }
    }
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeToRed()
    {
        if (!red)
        {
            foreach (Material mat in materials)
            {
                    Color rojo = new Color(mat.color.r+2f, mat.color.g, mat.color.b, mat.color.a);
                    mat.color = rojo;
            }
            red = true;
        }
    }

    public void ChangeToGreen()
    {
        if (red)
        {
            foreach (Material mat in materials)
            {
                    Color verde = new Color(mat.color.r-2f, mat.color.g, mat.color.b, mat.color.a);
                    mat.color = verde;
            }
            red = false;
        }
    }

    private void OnApplicationQuit()
    {
        for(int i=0; i<materials.Count; i++)
        {
            materials[i].color = materialsCopy[i].color;
            materials[i].shader = materialsCopy[i].shader;
        }
    }
}
