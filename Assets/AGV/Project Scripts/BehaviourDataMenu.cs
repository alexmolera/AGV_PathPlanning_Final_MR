using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Leap.Unity.Interaction;

public class BehaviourDataMenu : MonoBehaviour
{
    [Header("Export Button")]
    public GameObject exportButton;

    [Header("Snapshot Text & Camera")]
    public GameObject snapText;
    public GameObject dataText;
    public GameObject waypointsText;
    public GameObject noRoadText;
    public GameObject snapCamera;

    public GameObject Button1;
    public GameObject Button2;
    public GameObject Button3;
    public BehaviourSimulationMenu simulationScript;

    [Header("Data ")]
    public GameObject createdObjects;

    public GameObject floorObjects;
    public GameObject floor;
    public GameObject LinesParent;

    private float timeAppear = 2;
    private float startTime = 0;

    public bool prueba=false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        snapText.SetActive(false);

        System.IO.File.Delete(Application.streamingAssetsPath + "/ExportedData/chartjs/jsonPoints.json");
        System.IO.File.Delete(Application.streamingAssetsPath + "/ExportedData/chartjs/jsonPoints2.json");
        System.Diagnostics.Process process = System.Diagnostics.Process.Start("cmd.exe", "/c http-server " + Application.streamingAssetsPath.Replace(@"/", @"\") + "\\ExportedData\\chartjs" + " -a localhost -p 80");
        //System.Diagnostics.Process process = System.Diagnostics.Process.Start("cmd.exe", "/c http-server " + "C:\\Users\\Public\\PublicVarjo\\AGV PathPlanning\\AGV PathPlanning Scripting_Data\\StreamingAssets\\ExportedData\\chartjs" + " -a localhost -p 80");

    }

    // Update is called once per frame
    void Update()
    {
        //*****************DEBUG EXPORTAR DATOS********************
        if (Input.GetKeyDown(KeyCode.P))
        {
            ExportWaypoints();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            //ImportProgramData();
        }
        //*****************DEBUG EXPORTAR IMAGEN**********************
        if (Input.GetKeyDown(KeyCode.L))
        {
            snapCamera.SetActive(true);
            snapCamera.GetComponent<SnapshotCamera>().takeSnap = true;
        }

        if (exportButton.GetComponent<InteractionButton>().controlEnabled)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        if((Time.time-startTime) > timeAppear)
        {
            snapText.SetActive(false);
            dataText.SetActive(false);
            waypointsText.SetActive(false);
            noRoadText.SetActive(false);
            startTime = 0;
        }

        if (prueba)
        {
            
            Application.OpenURL("http://localhost:80");

            prueba = false;
        }

        if(simulationScript.currentState == 0)
        {
            Button1.SetActive(true);
            Button2.SetActive(true);
            Button3.SetActive(true);
        }
    }

    //This functions are directly called from buttons, not the same as the other menus
    public void ExportSnapshot()
    {
        snapCamera.SetActive(true);
        snapCamera.GetComponent<SnapshotCamera>().takeSnap = true;

        snapText.SetActive(true);
        startTime = Time.time;
    }

    public void ExportProgramData()
    {
        //ExportData.ObjectData[] objectList = new ExportData.ObjectData[createdObjects.transform.childCount];
        var objectList = new List<ObjectData>();
        
        for (int i = 0; i < createdObjects.transform.childCount; i++)
        {
            //Constructor Arguments
            objectList.Add(new ObjectData
                (
                createdObjects.transform.GetChild(i).gameObject.tag,
                createdObjects.transform.GetChild(i).gameObject.name,
                createdObjects.transform.GetChild(i).gameObject.transform.localPosition,
                createdObjects.transform.GetChild(i).gameObject.transform.localEulerAngles,
                createdObjects.transform.GetChild(i).gameObject.transform.localScale
                ));
        }
        //Insert Floor
        objectList.Add(new ObjectData
        (
        floor.tag,
        floor.name,
        Vector3.zero,
        Vector3.zero,
        floor.transform.localScale
        ));

        string json = JsonHelper.ToJson(objectList.ToArray(), true);                            //the array is transformed to a json format string

        //Source directoy
        string sourceDirectory = Application.streamingAssetsPath + "/ExportedData/ExportedWorkspace";
        System.IO.Directory.CreateDirectory(sourceDirectory);                                    //Check and create the directory

        //It gets an array of the files
        string[] fileNames = Directory.GetFiles(sourceDirectory, "*.json");
        int fileCount = fileNames.Length+1;

        File.WriteAllText(sourceDirectory + "/Program"+fileCount+".json", json);                 //the string is saved in
        Debug.Log("Program exported in: " + sourceDirectory);
        Debug.Log("http-server " + Application.streamingAssetsPath.Replace(@"/", @"\") + "\\ExportedData\\chartjs" + " -a localhost -p 80 & pause");

        dataText.SetActive(true);
        startTime = Time.time;
    }

    public void ExportWaypoints()
    {
        //GET THE 2 LINES
        if (LinesParent.transform.childCount == 2)
        {
            //ESTIMATED LINE
            var EstimatedPointList = new List<WaypointsData>(); //Create list
            LineRenderer estimatedLines = LinesParent.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
            int lineCount = LinesParent.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<LineRenderer>().positionCount;
            for(int i = 0; i < lineCount; i++)
            {
                GameObject point = new GameObject("" + i);
                point.transform.position = estimatedLines.GetPosition(i);
                point.transform.parent = floorObjects.transform;
                EstimatedPointList.Add(new WaypointsData(point.transform.localPosition, 1));                    //It saves the local position of the point 
                Destroy(point);
            }
            //REAL LINE
            LineRenderer RealLines = LinesParent.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<LineRenderer>();
            lineCount = LinesParent.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<LineRenderer>().positionCount;
            for (int i = 0; i < lineCount; i++)
            {
                GameObject point = new GameObject("" + i);
                point.transform.position = RealLines.GetPosition(i);
                point.transform.parent = floorObjects.transform;
                EstimatedPointList.Add(new WaypointsData(point.transform.localPosition, 2));                    //It saves the local position of the point 
                Destroy(point);
            }

            string Estimatedjson    = JsonHelper.ToJson(EstimatedPointList.ToArray(), true);                             //the array is transformed to a json format string

            //Source directoy
            string EstimatedsourceDirectory = Application.streamingAssetsPath + "/ExportedData/EstimatedWaypoints";
            System.IO.Directory.CreateDirectory(EstimatedsourceDirectory);                                                   //Check and create the directory

            //ESTIMATED WRITE
            string[] fileNames1 = Directory.GetFiles(EstimatedsourceDirectory, "*.json");
            int fileCount1 = fileNames1.Length + 1;
            File.WriteAllText(EstimatedsourceDirectory + "/Program" + fileCount1 + ".json", Estimatedjson);                   //the string is saved in
            Debug.Log("Program exported in: " + EstimatedsourceDirectory);

            File.WriteAllText(Application.streamingAssetsPath + "/ExportedData/chartjs/jsonPoints.json", Estimatedjson);

            /*
            System.Diagnostics.ProcessStartInfo commandInfo = new System.Diagnostics.ProcessStartInfo();
            commandInfo.WorkingDirectory = Application.dataPath.Replace(@"/", @"\") + "\\ExportedData\\chartjs\\"; ///@"c:\";
            commandInfo.FileName = "hyper.exe";
            commandInfo.CreateNoWindow = true;
            commandInfo.Arguments = "/c http-server " + Application.dataPath.Replace(@"/", @"\") + "\\ExportedData\\chartjs" + " -a localhost -p 80";

            Debug.Log("/c http-server " + Application.dataPath.Replace(@"/", @"\") + "\\ExportedData\\chartjs" + " -a localhost -p 80");
            Debug.Log(Application.dataPath);

            //System.Diagnostics.Process process = System.Diagnostics.Process.Start(commandInfo);
            //System.Diagnostics.Process process = System.Diagnostics.Process.Start("cmd.exe", "http-server " + Application.dataPath.Replace(@"/", @"\") + "\\ExportedData\\chartjs" + " -a localhost -p 80") ;
            */

            Application.OpenURL("http://localhost:80");

            waypointsText.SetActive(true);
            startTime = Time.time;
        }
        else
        {
            noRoadText.SetActive(true);
        }

    }
    
}
