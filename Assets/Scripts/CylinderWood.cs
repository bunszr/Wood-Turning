using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CylinderWood : MonoBehaviour
{
    MeshFilter meshFilter;
    PlaneWood planeWood;

    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;
    Vector3[] normals;

    public int pipeSegmentCount = 7;
    float stepAngleSize;

    public float turnSpeed = 75;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        planeWood = FindObjectOfType<PlaneWood>();

        CreateMesh();
        GetComponent<MeshRenderer>().material.mainTexture = planeWood.texture2D;
    }

    public void CreateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        Mesh mesh = new Mesh();
        mesh.name = "Cylinder";

        stepAngleSize = 360f / (pipeSegmentCount - 1);

        for (int i = 0; i < planeWood.SegmentCountX; i++)
        {
            SetLateralSurface(planeWood.BottomVertex(i).y, i, vertices, uvs, normals, triangles);
        }
        SetCover(0, vertices, uvs, triangles, normals, true);
        SetCover((planeWood.SegmentCountX - 1) * pipeSegmentCount, vertices, uvs, triangles, normals, false);


        this.vertices = vertices.ToArray();
        this.uvs = uvs.ToArray();
        this.triangles = triangles.ToArray();
        this.normals = normals.ToArray();

        mesh.vertices = this.vertices;
        mesh.triangles = this.triangles;
        mesh.uv = this.uvs;
        mesh.normals = this.normals;
        // mesh.RecalculateNormals(); // Bunu kullanırsak eğer yanal yüzeylerdeki aynı konumda olan vertex normalleri farklı olacağı için kötü bir görüntü ortaya çıkar ondan dolayı normalleride kendimiz belirliyoruz.
        mesh.RecalculateBounds();
        meshFilter.mesh = mesh;
    }

    public void SetLateralSurface(float radius, int xSegmentIndex, List<Vector3> vertices, List<Vector2> uvs, List<Vector3> normals, List<int> triangles)
    {
        for (int i = 0; i < pipeSegmentCount; i++)
        {
            float angle = i * stepAngleSize;
            float y = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            vertices.Add(new Vector3(planeWood.BottomVertex(xSegmentIndex).x, y, z));
            normals.Add(new Vector3(0, y, z));
            uvs.Add(new Vector2(planeWood.xUv(xSegmentIndex), i / (float)(pipeSegmentCount - 1)));

            if (xSegmentIndex < planeWood.SegmentCountX - 1)
            {
                triangles.Add(xSegmentIndex * pipeSegmentCount + i);
                triangles.Add(xSegmentIndex * pipeSegmentCount + i + 1);
                triangles.Add((xSegmentIndex + 1) * pipeSegmentCount + i);

                triangles.Add((xSegmentIndex + 1) * pipeSegmentCount + i);
                triangles.Add(xSegmentIndex * pipeSegmentCount + i + 1);
                triangles.Add((xSegmentIndex + 1) * pipeSegmentCount + i + 1);
            }
        }
    }

    void SetCover(int pairVertexIndex, List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, List<Vector3> normals, bool isLeftCover)
    {
        int startVertIndex = vertices.Count;
        for (int i = 0; i < pipeSegmentCount; i++)
        {
            vertices.Add(vertices[i + pairVertexIndex]);
            normals.Add(pairVertexIndex == 0 ? Vector3.left : Vector3.right);

            float xUv = Mathf.Cos(i * stepAngleSize * Mathf.Deg2Rad) * .25f + .75f;
            float yUv = Mathf.Sin(i * stepAngleSize * Mathf.Deg2Rad) * .25f + .5f;
            uvs.Add(new Vector2(xUv, yUv));

            if (i < pipeSegmentCount - 2)
            {
                triangles.Add(isLeftCover ? startVertIndex + i + 2 : startVertIndex);
                triangles.Add(startVertIndex + i + 1);
                triangles.Add(isLeftCover ? startVertIndex : startVertIndex + i + 2);
            }
        }
    }

    public void UpdateVertexY(int bottomIndex, float radius)
    {
        int startVertexIndex = bottomIndex * pipeSegmentCount;
        for (int y = 0; y < pipeSegmentCount; y++)
        {
            Vector3 normal = vertices[startVertexIndex + y].To0YZ().normalized;
            vertices[startVertexIndex + y] = new Vector3(vertices[startVertexIndex + y].x, 0, 0) + normal * radius;
        }

        if (bottomIndex == 0)
        {
            UpdateCoverVerticesY(planeWood.SegmentCountX * pipeSegmentCount, radius);
        }
        else if (bottomIndex == planeWood.SegmentCountX - 1)
        {
            UpdateCoverVerticesY((planeWood.SegmentCountX + 1) * pipeSegmentCount, radius);
        }
    }

    void UpdateCoverVerticesY(int startVertexIndex, float radius)
    {
        for (int y = 0; y < pipeSegmentCount; y++)
        {
            Vector3 normal = vertices[startVertexIndex + y].To0YZ().normalized;
            vertices[startVertexIndex + y] = new Vector3(vertices[startVertexIndex + y].x, 0, 0) + normal * radius;
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * turnSpeed);
    }

    public void UpdateUv(int bottomIndex, bool uvIsChange, float uvValueX)
    {
        if (uvIsChange)
        {
            int startUvIndexX = bottomIndex * pipeSegmentCount;
            for (int y = 0; y < pipeSegmentCount; y++)
            {
                uvs[startUvIndexX + y] = new Vector2(uvValueX, uvs[startUvIndexX + y].y);
            }
        }
    }

    public void UpdateMeshVertices()
    {
        meshFilter.mesh.vertices = vertices;
        // meshFilter.mesh.RecalculateNormals();
    }

    public void UpdateMeshUvs()
    {
        meshFilter.mesh.uv = uvs;
    }
}