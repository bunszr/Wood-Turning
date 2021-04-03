using System.Collections.Generic;
using UnityEngine;

public class PlaneWood : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField, HideInInspector] Mesh mesh;
    public Texture2D texture2D;

    [SerializeField] int segmentCountX = 40;
    [SerializeField] Vector2 planeSize;
    [SerializeField, Range(0, 1)] float[] changeableTexturePercent = new float[1];
    [SerializeField, HideInInspector] Vector3[] vertices;
    [SerializeField, HideInInspector] Vector2[] uvs;
    [SerializeField, HideInInspector] int[] triangles;

    float startedBottomVertexPosY;

    float xUvStep;
    int[] comprableUvIndies;

    public int SegmentCountX => segmentCountX;
    public Vector2 PlaneSize => planeSize;

    public Vector3 BottomVertex(int bottomIndex) => vertices[bottomIndex];
    public int TopIndex(int bottomIndex) => bottomIndex + segmentCountX;
    public Vector3 TopVertex(int bottomIndex) => vertices[bottomIndex + segmentCountX];
    public float xUv(int bottomIndex) => uvs[bottomIndex].x;

    public float PercentYPosStartedToOrgin(int bottomIndex) => Mathf.InverseLerp(startedBottomVertexPosY, 0, vertices[bottomIndex].y);

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        comprableUvIndies = new int[segmentCountX];
        startedBottomVertexPosY = planeSize.y * -.5f;
        GetComponent<MeshRenderer>().material.mainTexture = texture2D;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void UpdateUv(int bottomIndex, ref bool uvIndexIsChange, ref bool hasUpdateUv, ref float uvValueX)
    {
        if (comprableUvIndies[bottomIndex] < changeableTexturePercent.Length - 1)
        {
            float currVertexPercent = PercentYPosStartedToOrgin(bottomIndex);
            if (currVertexPercent > changeableTexturePercent[comprableUvIndies[bottomIndex]])
            {
                comprableUvIndies[bottomIndex]++;
                uvs[bottomIndex] += new Vector2(xUvStep, 0);
                uvs[bottomIndex + segmentCountX] += new Vector2(xUvStep, 0);
                uvValueX = uvs[bottomIndex].x;
                uvIndexIsChange = true;
                hasUpdateUv = true;
            }
        }
        else
            uvIndexIsChange = false;
    }
    public void UpdateMeshUvs()
    {
        mesh.uv = uvs;
    }


    public void UpdatePairVertexY(int bottomIndex, float newBottomPosY)
    {
        vertices[bottomIndex].y = newBottomPosY;
        vertices[bottomIndex + segmentCountX].y = Mathf.Abs(newBottomPosY);
    }
    public void UpdateMeshVertices()
    {
        mesh.vertices = vertices;
    }

    public void Init()
    {
        if (mesh == null)
        {
            mesh = meshFilter.sharedMesh = new Mesh();
        }
        mesh.name = "Plane Wood";
        CreatePlaneWood();
    }

    public void CreatePlaneWood()
    {
        segmentCountX = Mathf.Clamp(Mathf.RoundToInt(segmentCountX / 2) * 2, 2, int.MaxValue);
        xUvStep = 1f / changeableTexturePercent.Length;

        vertices = new Vector3[segmentCountX * 2];
        uvs = new Vector2[vertices.Length];
        triangles = new int[(segmentCountX - 1) * 6];
        int triangleIndex = 0;
        float xStepPos = planeSize.x / (segmentCountX - 1);
        for (int i = 0; i < segmentCountX; i++)
        {
            vertices[i] = new Vector3(planeSize.x * -.5f + i * xStepPos, planeSize.y * -.5f, 0);
            vertices[i + segmentCountX] = new Vector3(planeSize.x * -.5f + i * xStepPos, planeSize.y * .5f, 0);

            float xUv = i / (float)(segmentCountX - 1) * xUvStep;
            uvs[i] = new Vector2(xUv, 0);
            uvs[i + segmentCountX] = new Vector2(xUv, 1);

            if (i < segmentCountX - 1)
            {
                triangles[triangleIndex] = i;
                triangles[triangleIndex + 1] = i + segmentCountX;
                triangles[triangleIndex + 2] = i + 1;

                triangles[triangleIndex + 3] = i + 1;
                triangles[triangleIndex + 4] = i + segmentCountX;
                triangles[triangleIndex + 5] = i + segmentCountX + 1;

                triangleIndex += 6;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}