using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RobotMovement : MonoBehaviour
{
    //Pathfinding pf;//For referencing the grid class
    public Grid gridref;
    public PathFindingManhattan manhattan;
    public PathFindingDiagonal diagonal;

    public GameObject GameManager;
    public GameObject robot;
    public GameObject timer;
    public float LinearSpeed = 0.1f;
    [HideInInspector] public float defaultSpeed;    //Es una copia de la primera velocidad linear
    public float RotSpeedRad = 0.5f;
    public List<Node> waypoints = new List<Node>();
    SmoothPath sPath;
    private bool startMovementBool;
    private float turnDst = 0.1f;
    private float stoppingDst = 0.2f;
    private float speedPercent;
    [HideInInspector] public bool followingPath;
    private int pathIndex;
    private float distanceCovered;
    public GameObject distanceData;
    public GameObject Floor;
    private int index;
    GameObject smooth;
    GameObject SmoothLine;

    public Material LineMat;
    public Material SLineMat;
    public float TimeTemp=0;

    // public LayerMask WallMask;
    // private Vector3 Velocity;

    // Use this for initialization

    private void Awake()//When the program starts
    {
        defaultSpeed = LinearSpeed;
    }

    private void Start()
    {

    }
    private void OnEnable()
    {
        startMovementBool = false;
        followingPath = true;
        pathIndex = 1;
        speedPercent = 1;
        distanceCovered = 0;
        index = 1;

        //RotSpeedRad = (float)(-0.625 * Mathf.Pow(LinearSpeed, 2) + 5.625 * LinearSpeed - 0.05);
    }

    private void Update()
    {
        if (startMovementBool)
        {
            PathFollowing(sPath);
        } 
    }


    public void startMovement()
    {
        startMovementBool = true;
    }

    public void AppearRoadManhattan()
    {
        distanceCovered = 0;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }

        robot.transform.position = manhattan.FinalPath[0].vPosition;
        sPath = new SmoothPath(manhattan.FinalPath, manhattan.FinalPath[0].vPosition, turnDst, stoppingDst);
        robot.transform.LookAt(manhattan.FinalPath[1].vPosition);

        //Line
        GameObject trazado = new GameObject("Trazado");
        trazado.transform.parent = gameObject.transform;
        GameObject line = new GameObject("Line Road"); line.layer = 14;
        line.transform.parent = trazado.transform;
        line.AddComponent<LineRenderer>();
        line.GetComponent<LineRenderer>().startColor = Color.red;
        line.GetComponent<LineRenderer>().endColor = Color.red;
        line.GetComponent<LineRenderer>().startWidth = 0.01f;
        line.GetComponent<LineRenderer>().endWidth = 0.01f;
        line.GetComponent<LineRenderer>().positionCount = sPath.lookPoints.Count;
        line.GetComponent<LineRenderer>().useWorldSpace = true;
        line.GetComponent<LineRenderer>().material = LineMat;

        //Curved Line
        smooth = new GameObject("SmoothLine");
        smooth.transform.parent = gameObject.transform;
        SmoothLine = new GameObject("SLine"); SmoothLine.layer = 14;
        SmoothLine.transform.parent = smooth.transform;
        SmoothLine.AddComponent<LineRenderer>();
        SmoothLine.GetComponent<LineRenderer>().startColor = Color.green;
        SmoothLine.GetComponent<LineRenderer>().endColor = Color.green;
        SmoothLine.GetComponent<LineRenderer>().startWidth = 0.01f;
        SmoothLine.GetComponent<LineRenderer>().endWidth = 0.01f;
        SmoothLine.GetComponent<LineRenderer>().positionCount = 1;
        SmoothLine.GetComponent<LineRenderer>().useWorldSpace = true;
        SmoothLine.GetComponent<LineRenderer>().material = SLineMat;
        SmoothLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(robot.transform.position.x, Floor.transform.position.y - 0.03f, robot.transform.position.z));

        for (int i = 0; i < sPath.lookPoints.Count; i++)
        {
            //For drawing line in the world space, provide the x,y,z values of each point
            line.GetComponent<LineRenderer>().SetPosition(i, new Vector3(sPath.lookPoints[i].vPosition.x, Floor.transform.position.y-0.05f, sPath.lookPoints[i].vPosition.z));
            if (i > 0)
            {
                //We want the position in metres
                distanceCovered += (sPath.lookPoints[i].vPosition - sPath.lookPoints[i - 1].vPosition).magnitude * 100;
            }
        }
        string DistanceString = string.Format("{00}", (int)distanceCovered);
        distanceData.GetComponent<TMPro.TextMeshPro>().text = DistanceString.ToString();
    }

    public void AppearRoadDiagonal()
    {
        distanceCovered = 0;

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }

        robot.transform.position = diagonal.FinalPath[0].vPosition;
        sPath = new SmoothPath(diagonal.FinalPath, diagonal.FinalPath[0].vPosition, turnDst, stoppingDst);
        robot.transform.LookAt(diagonal.FinalPath[1].vPosition);

        //Estimated Line
        GameObject trazado = new GameObject("Trace");
        trazado.transform.parent = gameObject.transform;
        GameObject line = new GameObject("Line Road"); line.layer = 14;
        line.transform.parent = trazado.transform;
        line.AddComponent<LineRenderer>();
        line.GetComponent<LineRenderer>().startColor = Color.red;
        line.GetComponent<LineRenderer>().endColor = Color.red;
        line.GetComponent<LineRenderer>().startWidth = 0.01f;
        line.GetComponent<LineRenderer>().endWidth = 0.01f;
        line.GetComponent<LineRenderer>().positionCount = sPath.lookPoints.Count;
        line.GetComponent<LineRenderer>().useWorldSpace = true;
        line.GetComponent<LineRenderer>().material = LineMat;

        //Curved Lined
        smooth = new GameObject("SmoothLine");
        smooth.transform.parent = gameObject.transform;
        SmoothLine = new GameObject("SLine"); SmoothLine.layer = 14;
        SmoothLine.transform.parent = smooth.transform;
        SmoothLine.AddComponent<LineRenderer>();
        SmoothLine.GetComponent<LineRenderer>().startColor = Color.green;
        SmoothLine.GetComponent<LineRenderer>().endColor = Color.green;
        SmoothLine.GetComponent<LineRenderer>().startWidth = 0.01f;
        SmoothLine.GetComponent<LineRenderer>().endWidth = 0.01f;
        SmoothLine.GetComponent<LineRenderer>().positionCount = 1;
        SmoothLine.GetComponent<LineRenderer>().useWorldSpace = true;
        SmoothLine.GetComponent<LineRenderer>().material = SLineMat;
        SmoothLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(robot.transform.position.x, Floor.transform.position.y - 0.03f, robot.transform.position.z));


        for (int i = 0; i < sPath.lookPoints.Count; i++)
        {
            //For drawing line in the world space, provide the x,y,z values of each point
            line.GetComponent<LineRenderer>().SetPosition(i, new Vector3(sPath.lookPoints[i].vPosition.x, Floor.transform.position.y - 0.05f, sPath.lookPoints[i].vPosition.z));
            if (i > 0)
            {
                //We want the position in metres
                distanceCovered += (sPath.lookPoints[i].vPosition - sPath.lookPoints[i - 1].vPosition).magnitude * 100;
            }
        }
        string DistanceString = string.Format("{00}", (int)distanceCovered);
        distanceData.GetComponent<TMPro.TextMeshPro>().text = DistanceString.ToString();
    }
    public void PathFollowing(SmoothPath path)
    {
        if (followingPath && pathIndex < sPath.lookPoints.Count)
        {
            //It turns the vector into a 2d vector
            Vector2 pos2D = new Vector2(robot.transform.position.x, robot.transform.position.z);

            //If the turning line has been corsses the next index is set
            if (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    //The distance covered is displayed on screen

                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                // if the index is higher that the slow down index
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                    //a reduction percentage is set for slowing down the speed
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    if (speedPercent < 0.05)
                    {
                        followingPath = false;
                    }
                }
            }
            else
            {
                timer.GetComponent<Timer>().StopCountTime();
            }

            //Rotation is 8 more times than linear speed, but it changes when it aproaches to the end
            RotSpeedRad = LinearSpeed * 14;

            Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex].vPosition - robot.transform.position);
            robot.transform.rotation = Quaternion.Lerp(robot.transform.rotation, targetRotation, Time.deltaTime * RotSpeedRad * speedPercent);
            robot.transform.Translate(Vector3.forward * Time.deltaTime * LinearSpeed * speedPercent);

            //Draw Smooth Line
            if (gameObject.transform.childCount >= 2 && (Time.time - TimeTemp > 0.3))
            {
                SmoothLine.GetComponent<LineRenderer>().positionCount++;
                SmoothLine.GetComponent<LineRenderer>().SetPosition(index, new Vector3(robot.transform.position.x, Floor.transform.position.y - 0.03f, robot.transform.position.z));
                index++;

                TimeTemp = Time.time;
            }
        }

        else
        {
            timer.GetComponent<Timer>().StopCountTime();
        }

    }
}


