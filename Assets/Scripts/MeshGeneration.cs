using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGeneration : MonoBehaviour
{
    public Vector3[] vertices; //Vector array that contains all the vertices of the mesh, from now on referenced as grid.
    public int[] triangles; //Integer array that holds all the points of the triangles that have to be drawn for the mesh to be rendered
    public int gridX = 50; //Holds the grid size (x)
    public int gridZ = 50; //Holds the grid size (z)
    public float steep = 5; //determines how tall and steep a mountain can be
    public float perlinCoefficient = 0.1f; //Perlin noise coefficient, determines how "noisy" the terrain will be, the higher the value the more noisy the outcome
    Mesh mesh; //Contains the mesh gameobject
    MeshCollider meshCollider; //contains the mesh's collider

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh(); //Calls constructor method and initializes the mesh class
        GetComponent<MeshFilter>().mesh = mesh; //gets the mesh from the mesh generator object
        meshCollider = GetComponent<MeshCollider>(); //assigns the mesh collider on the mesh generator gameobject to the mesh collider variable

        CreateShape(); 
        UpdateMesh();
    }

    void LateUpdate()
    {
        CreateShape(); 
        UpdateMesh();
    }

    //Creates Vertices and Triangles for the mesh
    public void CreateShape()
    {
        vertices = new Vector3[(gridX + 1) * (gridZ + 1)]; //sets size of the array to the number of points required for a grid of apropriate size

        int i = 0; //counter variable
        
        for(int z = 0;z <= gridZ;z++) //whilst z position on grid is less than the z size of grid run the following
        {
            for(int x = 0;x <= gridX;x++) //whilst x position on grid is less than the x size of grid run the following
            {
                float y = Mathf.PerlinNoise(x * perlinCoefficient, z * perlinCoefficient) * steep; //Perlin noise is a way of generating pseudorandom numbers with a smooth gradient, this allows for the generated terrain to look more natural as there  are no sudden uneven points on the generated texture, i use this fact to assign a y value to each vertex while still making sure that the terrain looks good
                vertices[i] = new Vector3(x,y,z); //sets where the vertex should be in 3D space
                i++;
            }
        }

        triangles = new int[gridX * gridZ * 6]; //Defines the size of the triangle array to that of 6 * the number of squares in the grid, this is because for each square in the grid 2 triangles need to be formed and each trianle needs 3 points to be defined
        int tris = 0; //counts which triangle in the triangle array the program is on, increases in multiples of 6
        int vert = 0; //counts which vertex the program is on

        for(int z = 0;z < gridZ;z++)//whilst z position on grid is less than the z size of grid run the following
        {
            for(int x = 0;x < gridX;x++)//whilst x position on grid is less than the x size of grid run the following
            {
                /*
                In unity for a triangle has only one of its two faces rendered, the side that is rendered is decided by what direction the triangle was drawn in,
                in this case for the upper face to be rendered the triangle needs to be drawn clockwise
                0       1


                2       3
                in the above example a triangle would be drawn with the points 0,3,2 (in that order) and the other triangle would be 0,1,3 (in that order)
                below is the algorithm that does this
                */
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

    //updates the mesh visuals
    public void UpdateMesh()
    {
        mesh.Clear(); //clears whatever was previosly in the mesh object
        mesh.vertices = vertices; //sets the mesh vertices
        mesh.triangles = triangles; //sets the mesh triangles
        mesh.RecalculateNormals(); //recalculates the normals, in other words it recalculates the way light is meant to bounce off the object
        mesh.RecalculateTangents();
        meshCollider.sharedMesh = mesh; //assigns the mesh that is generated to the collider so that the collision can be calculated
        mesh.Optimize();
    }
}
