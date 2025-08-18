using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {

    Mesh mesh;
    MeshCollider meshCollider;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    [Min(1)]
    public int terrainSize = 100;

    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;
    public float scale = 10f;
    public float height = 1f;

    public Gradient gradient;

    public Vector2 noiseOffset = Vector2.zero;

    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        meshCollider = GetComponent<MeshCollider>();
    }

    void Update() {
        CreateShape();
        UpdateMesh();
    }

    float GenerateOctavePerlinNoise(float x, float z) {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < octaves; i++) {
            float sampleX = (x + noiseOffset.x) / scale * frequency;
            float sampleZ = (z + noiseOffset.y) / scale * frequency;
            float perlin = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;

            noiseHeight += perlin * amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return noiseHeight;
    }

    void CreateShape() {
        vertices = new Vector3[(terrainSize + 1) * (terrainSize + 1)];

        for (int i = 0, z = 0; z <= terrainSize; z++) {
            for (int x = 0; x <= terrainSize; x++) {
                float y = GenerateOctavePerlinNoise(x, z) * height;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[terrainSize * terrainSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < terrainSize; z++) {
            for (int x = 0; x < terrainSize; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + terrainSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + terrainSize + 1;
                triangles[tris + 5] = vert + terrainSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= terrainSize; z++) {
            for (int x = 0; x <= terrainSize; x++) {
                float normalizedHeight = Mathf.InverseLerp(-height, height, vertices[i].y);
                colors[i] = gradient.Evaluate(normalizedHeight);
                i++;
            }
        }
    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }
}