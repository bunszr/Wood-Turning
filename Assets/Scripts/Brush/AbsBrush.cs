using System.Collections.Generic;
using UnityEngine;

public abstract class AbsBrush : MonoBehaviour
{
    public Vector2 Left { get; protected set; }
    public Vector2 Right { get; protected set; }

    protected PlaneWood planeWood;
    protected Collider2D colider;
    [SerializeField] protected bool isGizmo = false;

    [SerializeField] Bounds intersectBounds;

    public float MaxBrushPointY {get; protected set; }

    protected virtual void Awake()
    {
        planeWood = FindObjectOfType<PlaneWood>();
        colider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        UpdateCuttingEdgePoints();
        intersectBounds.center = transform.position;
    }


    public abstract void UpdateCuttingEdgePoints();
    public abstract void SetIntersectPairIndex(ref List<BrushVertexInfo> brushVertexInfos);

    protected virtual void OnDrawGizmos()
    {
        if (!isGizmo)
            return;
        
        if (!Application.isPlaying)
            intersectBounds.center = transform.position;

        Gizmos.DrawWireCube(intersectBounds.center, intersectBounds.size);
    }

    
}