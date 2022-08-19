using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Leap.Unity.Interaction;

public class BehaviourImportData : MonoBehaviour
{
    [Header("Data ")]
    public GameObject createdObjects;
    public static string[] fileNames; //El nombre del primer archivo JSON de la carpeta ExportedProgram
    
    public static int fileCount = 0;
    public GameObject objectMenu;
    public static int fileIndex = 0;

    [Header("Button")]
    public GameObject Button1;

    [Header("Text ")]
    public GameObject nameFile;
    public GameObject dateFile;
    public TMPro.TextMeshPro displayText;
    public GameObject floor;
    public GameObject createdWalls;

    public GameObject ButtonNext; //Import
    public GameObject ButtonPrev; //Dismiss
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fileIndex < (fileNames.Length - 1)) { ButtonNext.SetActive(true); }
        else ButtonNext.SetActive(false);

        if (fileIndex > 0) { ButtonPrev.SetActive(true); }
        else ButtonPrev.SetActive(false);

        if (fileCount == 0)
        {
            Button1.SetActive(false);
        }
    }

    public void GetButtonNumber(int button_id)
    {
        switch (button_id)
        {
            //Button 1: Import
            case 1:
                ImportProgramData(fileNames[fileIndex]);
                break;

            //Dismiss
            case 2:
                break;

            //Next
            case 3:
                if(fileIndex < (fileNames.Length-1))
                    fileIndex++;
                    PrintFile();
                break;

            //Prev
            case 4:
                if (fileIndex > 0)
                    fileIndex--;
                    PrintFile();
                break;
        }
    }

    public void SearchProgramData()
    {
        fileCount = 0;
        fileIndex = 0;
        string sourceDirectory = Application.streamingAssetsPath + "/ExportedData/ExportedWorkspace/";
        System.IO.Directory.CreateDirectory(sourceDirectory);

        //It gets an array of the files
        fileNames = Directory.GetFiles(sourceDirectory, "*.json");
        fileCount = fileNames.Length;

        if (fileCount == 0)
        {
            displayText.text = "Any program has been found";
            nameFile.GetComponent<TMPro.TextMeshPro>().text = "";
            dateFile.GetComponent<TMPro.TextMeshPro>().text = "";
            Button1.SetActive(false);
        }
        else
            displayText.text = "Existing program data has been found!";
            Button1.SetActive(true);

        if (fileCount > 0)
        {
            string nameToPrint = fileNames[fileIndex];
            nameToPrint = nameToPrint.Replace(Application.streamingAssetsPath + "/ExportedData/ExportedWorkspace/", "");
            nameFile.GetComponent<TMPro.TextMeshPro>().text = "\t " + nameToPrint;
            dateFile.GetComponent<TMPro.TextMeshPro>().text = "\t " + File.GetCreationTime(fileNames[fileIndex]);
        }
    }

    public void PrintFile()
    {
        if (fileCount > 0)
        {
            string nameToPrint = fileNames[fileIndex];
            nameToPrint = nameToPrint.Replace(Application.streamingAssetsPath + "/ExportedData/ExportedWorkspace/", "");
            nameFile.GetComponent<TMPro.TextMeshPro>().text = "\t " + nameToPrint;
            dateFile.GetComponent<TMPro.TextMeshPro>().text = "\t " + File.GetLastWriteTime(fileNames[fileIndex]);
        }
    }

    public void ImportProgramData(string jsonString)
    {
        //if(File.Exists)
        jsonString = File.ReadAllText(jsonString);
        var objectList = JsonHelper.FromJson<ObjectData>(jsonString);

        //Delete all the existing objects
        while (BehaviorObjectMenu.createdObstaclesNum > 0)
        {
            if(createdObjects.transform.childCount > 0)
                objectMenu.GetComponent<BehaviorObjectMenu>().DeleteCube();
        }

        //Check sure that all the objects are destroyed
        foreach (Transform child in createdObjects.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        //Be sure to destroy everything
        //while (createdObjects.transform.childCount > 0) { 
        //    Destroy(createdObjects.transform.GetChild(0).gameObject);
        //}

        //Import objects
        for (int i = 0; i < objectList.Length; i++)
        {
            if(objectList[i].tag == "Cube")
                objectMenu.GetComponent<BehaviorObjectMenu>().DettachCube(objectList[i].name, objectList[i].Position, objectList[i].Rotation, objectList[i].Scale);
            if (objectList[i].tag == "Floor")
            {
                for (int j = 0; j < 4; j++)
                {
                    createdWalls.transform.GetChild(0).transform.SetParent(floor.transform, true);
                }
                floor.transform.localScale = objectList[i].Scale;
                //for (int j = 0; j < 4; j++)
                //{
                //    floor.transform.GetChild(0).transform.parent = createdWalls.transform;
                //}
            }
        }

        //fileCount = 0;
    }
}
