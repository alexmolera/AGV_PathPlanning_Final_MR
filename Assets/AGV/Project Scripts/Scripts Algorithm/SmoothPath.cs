using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothPath
{ //Such the node class

	public List<Node> lookPoints;
	public Line[] turnBoundaries;
	public int finishLineIndex;
	public int slowDownIndex;


	public SmoothPath(List<Node> waypoints, Vector3 startPos, float turnDst, float stoppingDst)
	{
		lookPoints = waypoints; //Renaming waypoints
		turnBoundaries = new Line[lookPoints.Count];
		finishLineIndex = turnBoundaries.Length - 1;

		Vector2 previousPoint = V3ToV2(startPos);
		for (int i = 0; i < lookPoints.Count; i++)
		{
			Vector2 currentPoint = V3ToV2(lookPoints[i].vPosition);//Current point turned into a 2D vector
			Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
			//For the last point, the turn boundary point will b eonto that line, with no turning distance
			Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;
			//The turn boundary is a perpendicular line from the one the robot will start to rotate with a certain speed and turn distance
			//The turn distanc eis substracted just in case the distance between both points is smaller than the turn distance
			turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);
			previousPoint = turnBoundaryPoint;
		}

		float dstFromEndPoint = 0;
		for (int i = lookPoints.Count - 1; i > 0; i--)
		{
			dstFromEndPoint += Vector3.Distance(lookPoints[i].vPosition, lookPoints[i - 1].vPosition);
			if (dstFromEndPoint > stoppingDst)
			{
				//From the endnode, the index where the robot will have to slow down will be stored
				slowDownIndex = i; 
				break;
			}
		}
	}

	Vector2 V3ToV2(Vector3 v3)
	{
		return new Vector2(v3.x, v3.z);
	}

	public void DrawWithGizmos()
	{

		Gizmos.color = Color.black;
		foreach (Node p in lookPoints)
		{
			Gizmos.DrawCube(p.vPosition + Vector3.up, Vector3.one);
		}

		Gizmos.color = Color.white;
		foreach (Line l in turnBoundaries)
		{
			l.DrawWithGizmos(10);
		}

	}

}
