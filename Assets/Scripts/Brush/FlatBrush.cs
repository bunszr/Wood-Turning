using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlatBrush : AbsBrush
{
    Vector3 leftDir;
    Vector3 rightDir;

    public override void UpdateCuttingEdgePoints()
    {
        Left = transform.position + leftDir;
        Right = transform.position + rightDir;
    }

    protected override void Awake()
    {
        base.Awake();
        Left = new Vector2(colider.bounds.min.x, colider.bounds.max.y);
        Right = colider.bounds.max;
        leftDir = Left - (Vector2)transform.position;
        rightDir = Right - (Vector2)transform.position;
        Vector3 maxPoint = colider.bounds.max;
        MaxBrushPointY = transform.position.y - maxPoint.y;
        Destroy(colider);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SetIntersectPairIndex(ref List<BrushVertexInfo> brushVertexInfos)
    {
        int[] interpolatedTwoIndies = Utility.GetInterpolationTwoIndies(planeWood, Left, Right);

        for (int i = interpolatedTwoIndies[0]; i < interpolatedTwoIndies[1]; i++)
        {
            if (Left.y > planeWood.BottomVertex(i).y)
            {
                if (Left.x < planeWood.BottomVertex(i).x && planeWood.BottomVertex(i).x < Right.x)
                {
                    brushVertexInfos.Add(new BrushVertexInfo(i, Left.y));
                    if (i + 1 == planeWood.SegmentCountX - 1)
                        brushVertexInfos.Add(new BrushVertexInfo(i + 1, Left.y));
                }
            }
        }
    }
}

