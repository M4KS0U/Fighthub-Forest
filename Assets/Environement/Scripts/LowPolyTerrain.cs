using UnityEngine;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Topology;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

public class LowPolyTerrain : MonoBehaviour
{
    public int width = 100;
    public int depth = 100;
    public float scale = 20f;
    public float heightScale = 10f;

    [Range(1, 8)] public int octaves = 4;
    [Range(0, 8)] public int octavesSkip = 0;
    [Range(0f, 1f)] public float persistence = 0.5f;
    public float lacunarity = 2f;
    public Vector2 offset;
    public float seed = 0f;
    public float dampening = 1f;

    private TriangleNet.Mesh mesh;
    private UnityEngine.Mesh terrainMesh;
    private List<float> heights = new List<float>();
    public float minNoiseHeight;
    public float maxNoiseHeight;

    private List<float> colorList = new List<float>();
    public float minColor;
    public float maxColor;

    public Gradient heightGradient;
    public Gradient colorGradient;

    private Polygon polygon;
    public Material material;

    [Range(1f, 50f)] public float colorScale = 10;

    public Vector2 islandCenter;
    public float islandSize = 10f;

    public void Initiate()
    {
        InitializeTerrain();
        ShapeTerrain();
        GenerateMesh();
    }

    private void InitializeTerrain()
    {
        
        heights = new List<float>();
        polygon = new Polygon();
        
        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                polygon.Add(new Vertex(x, z));
            }
        }

        ConstraintOptions constraints = new ConstraintOptions();
        constraints.ConformingDelaunay = true;

        mesh = polygon.Triangulate(constraints) as TriangleNet.Mesh;
    }

    private Vector3 GetVertexPosition(Triangle currentTriangle, int vertexIndex)
    {
        Vector3 vertex = new Vector3((float) currentTriangle.vertices[vertexIndex].x,heights[currentTriangle.vertices[vertexIndex].id],(float)currentTriangle.vertices[vertexIndex].y);

        vertex.x += (Mathf.PerlinNoise((vertex.x + offset.x) * 10, (vertex.z + offset.y) * 10) - 0.5f);
        vertex.z += (Mathf.PerlinNoise((vertex.x + offset.x) * 10, (vertex.z + offset.y) * 10) - 0.5f);
        return vertex;
    }

    private void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();

        IEnumerator<Triangle> triangleEnum = mesh.Triangles.GetEnumerator();

        for (int i = 0; i < mesh.Triangles.Count; i++)
        {
            if (!triangleEnum.MoveNext())
            {
                break;
            }

            Triangle currentTriangle = triangleEnum.Current;

            Vector3 v0 = GetVertexPosition(currentTriangle, 2);
            Vector3 v1 = GetVertexPosition(currentTriangle, 1);
            Vector3 v2 = GetVertexPosition(currentTriangle, 0);

            triangles.Add(vertices.Count);
            triangles.Add(vertices.Count + 1);
            triangles.Add(vertices.Count + 2);

            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);

            var normal = Vector3.Cross(v1 - v0, v2 - v0);

            var triangleColor = EvaluateColor(currentTriangle);
            
            for(int x = 0; x < 3; x++)
            {
                normals.Add(normal);
                uvs.Add(Vector3.zero);
                colors.Add(triangleColor);
            }
        }

        terrainMesh = new UnityEngine.Mesh();
        terrainMesh.vertices = vertices.ToArray();
        terrainMesh.uv = uvs.ToArray();
        terrainMesh.triangles = triangles.ToArray();
        terrainMesh.colors = colors.ToArray();
        terrainMesh.normals = normals.ToArray();
        terrainMesh.RecalculateBounds();

        transform.GetComponent<MeshFilter>().mesh = terrainMesh;
        transform.GetComponent<MeshCollider>().sharedMesh = terrainMesh;
        transform.GetComponent<MeshRenderer>().material = material;
        
    }

    private Color EvaluateColor(Triangle triangle)
    {
        var currentHeight = heights[triangle.vertices[0].id] + heights[triangle.vertices[1].id] + heights[triangle.vertices[2].id];
        currentHeight /= 3f;
        var color = colorList[triangle.vertices[0].id] + colorList[triangle.vertices[1].id] + colorList[triangle.vertices[2].id];
        color /= 3f;


        //currentHeight = (currentHeight < 0f) ? currentHeight / heightScale * 10f : currentHeight / heightScale;

        var gradientVal = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, currentHeight);
        Color triangleColor = heightGradient.Evaluate(gradientVal);
        var colorVal = Mathf.InverseLerp(minColor, maxColor, color);
        // triangleColor = colorGradient.Evaluate(colorVal);
        triangleColor = (triangleColor + colorGradient.Evaluate(colorVal)) / 2;

        //triangleColor = ApplyPerlinNoiseToColor(triangleColor, variation);
        return triangleColor;
    }

    void ShapeTerrain()
    {
        minNoiseHeight = float.PositiveInfinity;
        maxNoiseHeight = float.NegativeInfinity;

        heights.Clear();
        colorList.Clear();

        for (int i = 0; i < mesh.vertices.Count; i++)
        {
            float amplitude = 1f;
            float frequency = 1f;
            float noiseHeight = 0f;

            for (int o = 0; o < octavesSkip; o++)
            {
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            for (int o = octavesSkip; o < octaves; o++)
            {
                float xValue = ((float)mesh.vertices[i].x + offset.x) / scale * frequency;
                float zValue = ((float)mesh.vertices[i].y + offset.y) / scale * frequency;

                float perlinValue = Mathf.PerlinNoise(xValue + seed, zValue + seed) * 2 - 1;
                perlinValue *= dampening;

                noiseHeight += perlinValue * amplitude;

                amplitude *= persistence;
                frequency *= lacunarity;
            }

            noiseHeight = (noiseHeight + 1.3f) * FallOff((float)mesh.vertices[i].x + offset.x, (float)mesh.vertices[i].y + offset.y);
            noiseHeight = (noiseHeight - 1.3f);
            noiseHeight *= heightScale;
            if (noiseHeight > maxNoiseHeight) {
                maxNoiseHeight = noiseHeight;
            } else if (noiseHeight < minNoiseHeight) {
                minNoiseHeight = noiseHeight;
            }

            float xClr = ((float)mesh.vertices[i].x + offset.x) / colorScale;
            float zClr = ((float)mesh.vertices[i].y + offset.y) / colorScale;

            float color = (Mathf.PerlinNoise(xClr + seed, zClr + seed)) * 2 - 1;

            if (color > maxColor) {
                maxColor = color;
            } else if (color < minColor) {
                minColor = color;
            }
            colorList.Add(color);
            // float flatLevel = -0.2f;
            // noiseHeight = (noiseHeight < flatLevel) ? (noiseHeight - flatLevel) / 10f + flatLevel : noiseHeight;

            heights.Add(noiseHeight);
        }
    }

    private float FallOff(float x, float z)
    {
        float distanceX = x - islandCenter.x;
        float distanceZ = z - islandCenter.y;

        float distanceFromCenter = Mathf.Sqrt(distanceX * distanceX + distanceZ * distanceZ);

        float normalizedDistance = distanceFromCenter / islandSize;

        if (normalizedDistance < 0.6f) {
            return 1.0f;
        }

        float falloff = Mathf.Clamp01(1.0f - (normalizedDistance - 0.6f) / 0.4f);

        return falloff;

    }

}
