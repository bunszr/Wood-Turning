using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    static readonly int[] interpolationTwoIndies = new int[2];

    public static Vector3 To0YZ(this Vector3 v)
    {
        v.x = 0;
        return v;
    }

    public static Vector2 GetMousePos(Camera cam)
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Plane p = new Plane(Vector3.forward, Vector3.zero);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float enter;
        if (p.Raycast(ray, out enter))
        {
            mousePos = ray.GetPoint(enter);
        }
        return mousePos;
    }

    public static int[] GetInterpolationTwoIndies(PlaneWood planeWood, Vector2 leftPoint, Vector2 rigthPoint)
    {
        float interpolatedPercentA = Mathf.InverseLerp(planeWood.BottomVertex(0).x, planeWood.BottomVertex(planeWood.SegmentCountX - 1).x, leftPoint.x);
        float interpolatedPercentB = Mathf.InverseLerp(planeWood.BottomVertex(0).x, planeWood.BottomVertex(planeWood.SegmentCountX - 1).x, rigthPoint.x);
        
        interpolationTwoIndies[0] = Mathf.FloorToInt(interpolatedPercentA * (planeWood.SegmentCountX - 1));
        interpolationTwoIndies[1] = Mathf.CeilToInt(interpolatedPercentB * (planeWood.SegmentCountX - 1));
        return interpolationTwoIndies;
    }

    public static Vector3[] GetBrushPoints(int stepSize, Collider2D colider, Vector3 position)
    {
        List<Vector3> point = new List<Vector3>();
        Vector2 left = new Vector2(colider.bounds.min.x + .001f, colider.bounds.center.y);
        Vector2 right = new Vector2(colider.bounds.max.x, colider.bounds.center.y);
        float space = (right.x - left.x - .001f) / (float)stepSize;
        LayerMask mask = 1 << 8;
        int i = 0;
        while (true)
        {
            Vector2 newDir = Vector2.right * space * i;
            Vector2 rayPoint = left + Vector2.up + newDir;
            Debug.DrawRay(rayPoint, Vector2.down * 2, Color.red, 3);
            RaycastHit2D hit2D = Physics2D.Raycast(rayPoint, Vector2.down, 2, mask);

            if (hit2D)
                point.Add(hit2D.point);
            else
                break;
            i++;
        }
        return point.ToArray();
    }

    public static Vector3 GetMaxBrushPointFromArray(this Vector3[] pointArray)
    {
        float compareDst = Mathf.NegativeInfinity;
        int bestIndex = 0;
        for (int i = 0; i < pointArray.Length; i++)
        {
            if (pointArray[i].y > compareDst)
            {
                compareDst = pointArray[i].y;
                bestIndex = i;
            }
        }
        return pointArray[bestIndex];
    }

    public static Vector3[] GetPointDir(Vector3[] point, Vector3 position)
    {
        Vector3[] pointDir = new Vector3[point.Length];
        for (int i = 0; i < point.Length; i++)
        {
            pointDir[i] = point[i] - position;
        }
        return pointDir;
    }
}