using System.Collections.Generic;
using UnityEngine;

public class CircleBrush : AbsBrush
{
    Vector3[] brushPoints;
    Vector3[] pointDir;

    protected override void Awake()
    {
        base.Awake();
        brushPoints = Utility.GetBrushPoints(10, colider, transform.position);
        pointDir = Utility.GetPointDir(brushPoints, transform.position);
        Vector3 maxPoint = brushPoints.GetMaxBrushPointFromArray();
        MaxBrushPointY = transform.position.y - maxPoint.y;
        Destroy(colider);
    }

    public override void UpdateCuttingEdgePoints()
    {
        for (int i = 0; i < brushPoints.Length; i++)
        {
            brushPoints[i] = transform.position + pointDir[i];
        }
        Left = brushPoints[0];
        Right = brushPoints[brushPoints.Length - 1];
    }

    public override void SetIntersectPairIndex(ref List<BrushVertexInfo> brushVertexInfos)
    {
        int[] interpolatedTwoIndies = Utility.GetInterpolationTwoIndies(planeWood, Left, Right);
        int newIntersectPointIndex = 0;
        for (int i = interpolatedTwoIndies[0]; i < interpolatedTwoIndies[1]; i++)
        {
            for (int brushIndex = newIntersectPointIndex; brushIndex < brushPoints.Length; brushIndex++) // intersectPointIndex = ipIndex
            {
                if (brushPoints[brushIndex].y > planeWood.BottomVertex(i).y)
                {
                    if (planeWood.BottomVertex(i).x < brushPoints[brushIndex].x && brushPoints[brushIndex].x < planeWood.BottomVertex(i + 1).x)
                    {
                        brushVertexInfos.Add(new BrushVertexInfo(i, brushPoints[brushIndex].y));
                        if (i + 1 == planeWood.SegmentCountX - 1)
                            brushVertexInfos.Add(new BrushVertexInfo(i + 1, brushPoints[brushIndex + 1 < brushPoints.Length ? brushIndex + 1 : brushIndex].y));
                        newIntersectPointIndex = brushIndex + 1;
                        break;
                    }
                }
            }
            // Debug.DrawLine(planeWood.BottomVertex(i), planeWood.BottomVertex(i) + Vector3.down * 6);
        }
    }

    protected override void OnDrawGizmos()
    {
        if (!isGizmo)
            return;

        base.OnDrawGizmos();

        if (brushPoints == null)
            return;

        for (int i = 0; i < brushPoints.Length; i++)
        {
            Gizmos.DrawSphere(brushPoints[i], .01f);
        }
    }
}