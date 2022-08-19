using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWalls : MonoBehaviour
{

    public GameObject CreatedWalls;
    public GameObject Plane;
    public GameObject coordenatesImage;

    public Material OffMaterial;
    public Material OnMaterial;

    private bool wallsCreated;
    // Start is called before the first frame update
    void Start()
    {
        wallsCreated = false;
        coordenatesImage.SetActive(false);

        Plane.transform.GetChild(0).GetComponent<MeshRenderer>().material = OffMaterial;
        Plane.transform.GetChild(1).GetComponent<MeshRenderer>().material = OffMaterial;
        Plane.transform.GetChild(2).GetComponent<MeshRenderer>().material = OffMaterial;
        Plane.transform.GetChild(3).GetComponent<MeshRenderer>().material = OffMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
   
    }

    public void CreateWalls()
    {
        if (!wallsCreated)
        {
            List<GameObject> squadlist = new List<GameObject>();
            for  (int i = 0; i<Plane.transform.childCount; i++)
            {
                if(Plane.transform.GetChild(i).tag == "Wall")
                {
                    squadlist.Add(Plane.transform.GetChild(i).gameObject);
                }
            }

            for (int i = 0; i <squadlist.Count; i++)
            {
                squadlist[i].GetComponent<MeshRenderer>().material = OnMaterial;
                squadlist[i].transform.SetParent(CreatedWalls.transform, true);
            }
            wallsCreated = true;
            coordenatesImage.SetActive(true);
        }
    }

    public void ResetWalls()
    {
        if (wallsCreated)
        {
            for (int i = 0; i <= 3; i++)
            {
                CreatedWalls.transform.GetChild(0).GetComponent<MeshRenderer>().material = OffMaterial;
                CreatedWalls.transform.GetChild(0).transform.SetParent(Plane.transform, true);
            }
        }
        wallsCreated = false;

        coordenatesImage.SetActive(false);

    }
}
