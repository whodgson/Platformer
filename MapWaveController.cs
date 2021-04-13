using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapWaveController : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] base_vertices;
    private Vector3[] world_vertices;

    public float wave_scale = 0.1f;
    public float wave_speed = 1.0f;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        // The local space vertices (pre-transformation).

        if (base_vertices == null)
            base_vertices = mesh.vertices;

        // The world space vertices (for use in the SIN function).

        if (world_vertices == null)
        {
            world_vertices = base_vertices.Clone() as Vector3[];

            for (int i = 0; i < world_vertices.Length; i++)
            {
                Vector3 world_vertex = world_vertices[i];
                world_vertex = this.transform.TransformPoint(world_vertex);
                world_vertices[i] = world_vertex;
            }
        }

        // Apply the wave effect from the base vertices.

        Vector3[] vertices = new Vector3[base_vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = base_vertices[i];
            vertex.y += Mathf.Sin
                (Time.time 
                * wave_speed 
                + world_vertices[i].x 
                + world_vertices[i].y 
                + world_vertices[i].z) * wave_scale;
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
    }
}
