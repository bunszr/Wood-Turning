using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurningState : MonoBehaviour
{
    PlaneWood planeWood;
    public AbsBrush currBrush;
    public AbsBrush[] brushes;

    List<BrushVertexInfo> brushVertexInfos = new List<BrushVertexInfo>();

    CylinderWood cylinderWood;
    Camera cam;

    private void Start()
    {
        cylinderWood = FindObjectOfType<CylinderWood>();
        planeWood = FindObjectOfType<PlaneWood>();
        cam = Camera.main;
    }

    private void Update()
    {
        Vector2 brushPos = Utility.GetMousePos(cam);

        if (Input.GetMouseButton(0))
        {
            brushVertexInfos.Clear();
            currBrush.SetIntersectPairIndex(ref brushVertexInfos);
            if (brushVertexInfos.Count != 0)
            {
                float uvValueX = 0;
                bool hasUpdateUv = false;
                for (int wrongIndex = 0; wrongIndex < brushVertexInfos.Count; wrongIndex++)
                {
                    bool uvIndexIsChange = false;
                    int bottomI = brushVertexInfos[wrongIndex].bottomIndex;

                    planeWood.UpdatePairVertexY(bottomI, brushVertexInfos[wrongIndex].newVertexY);
                    planeWood.UpdateUv(bottomI, ref uvIndexIsChange, ref hasUpdateUv, ref uvValueX);

                    cylinderWood.UpdateVertexY(bottomI, Mathf.Abs(brushVertexInfos[wrongIndex].newVertexY));
                    cylinderWood.UpdateUv(bottomI, uvIndexIsChange, uvValueX);

                }
                if (hasUpdateUv)
                {
                    planeWood.UpdateMeshUvs();
                    cylinderWood.UpdateMeshUvs();
                }
                planeWood.UpdateMeshVertices();
                cylinderWood.UpdateMeshVertices();
            }
        }
        brushPos.y = Mathf.Clamp(brushPos.y, Mathf.NegativeInfinity, currBrush.MaxBrushPointY);
        currBrush.transform.position = brushPos;
    }

    public void SetBrush()
    {
        int index = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        currBrush.gameObject.SetActive(false);
        currBrush = brushes[index];
        currBrush.gameObject.SetActive(true);
    }
}
