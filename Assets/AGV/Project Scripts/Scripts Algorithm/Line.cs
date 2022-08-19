using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{

	const float verticalLineGradient = 1e5f;

	float gradient;
	float y_intercept;
	Vector2 pointOnLine_1;
	Vector2 pointOnLine_2;

	float gradientPerpendicular;

	bool approachSide;

	public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
	{
		float dx = pointOnLine.x - pointPerpendicularToLine.x;//distance from the x point to the perpendicular one
		float dy = pointOnLine.y - pointPerpendicularToLine.y;//distance from the y point to the perpendicular one
		//The perpendicular gradient is calculated
		//In case it is 0, the value would be infinite, that is why a high value is set

		if (dx == 0)
		{
			gradientPerpendicular = verticalLineGradient;
		}
		else
		{
			gradientPerpendicular = dy / dx;
		}
		//As it happened before, to avoid the infinte value from an horizontal line
		if (gradientPerpendicular == 0)
		{
			gradient = verticalLineGradient;
		}
		else
		{
			gradient = -1 / gradientPerpendicular;
		}
		//Y=mx+c m=gradient c=y intercept
		//c=y-mx
		y_intercept = pointOnLine.y - gradient * pointOnLine.x;
		pointOnLine_1 = pointOnLine;
		pointOnLine_2 = pointOnLine + new Vector2(1, gradient);
		approachSide = false;
		approachSide = GetSide(pointPerpendicularToLine);
	}

	bool GetSide(Vector2 p)
	{
		//If the point is in one side it's true, if false, it's in the other side 
		return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
	}

	public bool HasCrossedLine(Vector2 p)
	{
		//It determines if the point is in the other point of the line 
		return GetSide(p) != approachSide;//If the point crosses the line it's true
	}

	public float DistanceFromPoint(Vector2 p)
	{
		float yInterceptPerpendicular = p.y - gradientPerpendicular * p.x; //c=y-mx
		float intersectX = (yInterceptPerpendicular - y_intercept) / (gradient - gradientPerpendicular);//x=(c2-c1/m1-m2)
		float intersectY = gradient * intersectX + y_intercept;//y=mx+n
		return Vector2.Distance(p, new Vector2(intersectX, intersectY));
	}

	public void DrawWithGizmos(float length)
	{
		Vector3 lineDir = new Vector3(1, 0, gradient).normalized;
		Vector3 lineCentre = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
		Gizmos.DrawLine(lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
	}

}

