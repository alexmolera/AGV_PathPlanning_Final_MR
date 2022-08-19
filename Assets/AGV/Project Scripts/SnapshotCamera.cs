using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Leap.Unity.Interaction;
using Leap.Unity;


[RequireComponent(typeof(Camera))]
public class SnapshotCamera : MonoBehaviour
{
    //public InteractionButton button;

    public GameObject debugLog;
    public string FolderName;

    public GameObject floor;
    public GameObject obstacles;
    public GameObject targetPoint1;
    public GameObject targetPoint2;

    [Header("Objets to consider")]
    public Material cubeMaterial;
    public Material floorMaterial;
    public Material target1Material;
    public Material target2Material;

    [Header("Previous attributes of the objects")]
    public Material lastCubeMaterial;
    private Material lastFloorMaterial;
    private Material lastTarget1Material;
    private Material lastTarget2Material;
    private float lastPosition;

    Camera snapCam;
    int resWidth = 540;
    int resHeight = 540;
    private int i = 1;
    private float maxScale;

    [HideInInspector] public bool takeSnap = false; 

    // Start is called before the first frame update
    void Awake()
    {
        snapCam = GetComponent<Camera>();
        if (snapCam.targetTexture == null)
        {
            snapCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
        }
        else
        {
            resWidth = snapCam.targetTexture.width;
            resHeight = snapCam.targetTexture.height;
        }
        snapCam.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetSnap()
    {
        takeSnap = true;
    }

    public void LateUpdate()
    {
        if (takeSnap)
        {
            //Get Materials
            lastFloorMaterial = floor.GetComponent<MeshRenderer>().material;
            lastTarget1Material = targetPoint1.GetComponent<MeshRenderer>().material;
            lastTarget2Material = targetPoint2.GetComponent<MeshRenderer>().material;
            //Change Materials a
            floor.GetComponent<MeshRenderer>().material = floorMaterial;
            targetPoint1.GetComponent<MeshRenderer>().material = target1Material;
            targetPoint2.GetComponent<MeshRenderer>().material = target2Material;
            for(i=0; i<obstacles.transform.childCount; i++)
            {
                obstacles.transform.GetChild(i).GetComponent<MeshRenderer>().material = cubeMaterial;
            }

            //Set Camera size
            maxScale = Mathf.Max(floor.transform.localScale.x, floor.transform.localScale.z);
            snapCam.orthographicSize = 5.3f * maxScale + 0.1f;

            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            snapCam.targetTexture = rt;
            Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            snapCam.Render();
            
            RenderTexture.active = rt;
            //RenderTexture.active = snapCam.targetTexture;

            snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            snapCam.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            byte[] bytes = snapshot.EncodeToPNG();     
            string fileName = SnapShotName();

            System.IO.Directory.CreateDirectory(Application.streamingAssetsPath + "/" + FolderName);   //Check and create the directory
            System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/" + FolderName + "/" + fileName + ".png", bytes);

            Debug.Log("Snapshot " + i + " taken. Saved in: " + Application.streamingAssetsPath + "/" + FolderName);
            PrintDebugLog("Snapshot "+i+" taken. Saved in: " + Application.streamingAssetsPath + "/" + FolderName);
            i++;

            snapCam.gameObject.SetActive(false);
            takeSnap = false;

            //Change Materials 
            floor.GetComponent<MeshRenderer>().material = lastFloorMaterial;
            targetPoint1.GetComponent<MeshRenderer>().material = lastTarget1Material;
            targetPoint2.GetComponent<MeshRenderer>().material = lastTarget2Material;
            for (i = 0; i < obstacles.transform.childCount; i++)
            {
                obstacles.transform.GetChild(i).GetComponent<MeshRenderer>().material = lastCubeMaterial;
            }
        }
    }

    string SnapShotName()
    {
        return string.Format("Snap_{2}", resWidth, resHeight, System.DateTime.Now.ToString("HH_mm_ss"));

        //return string.Format("{0}/snapshots/_snap{1}x{2}_{3}.png", Application.dataPath, resWidth, resHeight, System.DateTime.Now.ToString("HH:mm:ss"));
    }

    void PrintDebugLog(string n)
    {
        debugLog.GetComponent<TMPro.TextMeshPro>().text = "[" + System.DateTime.UtcNow.ToString("HH:mm:ss") + "]" + " " + n + "\n\n" + debugLog.GetComponent<TMPro.TextMeshPro>().text;
    }
}
