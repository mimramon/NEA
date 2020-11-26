using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGeneration : MonoBehaviour
{
    public Vector3[] vertices;
    public int[] triangles;
    public int gridX = 50;
    public int gridZ = 50;
    Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
    }

    // Update is called once per frame
    public void CreateShape()
    {
        vertices = new Vector3[(gridX + 1) * (gridZ + 1)];

        int i = 0;
        
        for(int z = 0;z <= gridZ;z++)
        {
            for(int x = 0;x <= gridX;x++)
            {
                float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2;
                vertices[i] = new Vector3(x,y,z);
                i++;
            }
        }

        triangles = new int[gridX * gridZ * 6];
        int tris = 0;
        int vert = 0;

        for(int z = 0;z < gridZ;z++)
        {
            for(int x = 0;x < gridX;x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + gridX + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + gridX + 1;
                triangles[tris + 5] = vert + gridX + 2;

                vert++;
                tris +=6 ;

            }
            vert++;
        }
    }

    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
