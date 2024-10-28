using UnityEngine;
using System.Collections.Generic;

public class IslandCell
{
    public int index;          
    public float humidity;     
    public float temperature;  
    public float height;       
    public Color color;        

    public IslandCell(int index, float height, float humidity, float temperature, Color color)
    {
        this.index = index;
        this.height = height;
        this.humidity = humidity;
        this.temperature = temperature;
        this.color = color;
    }
}

public class IslandMap : MonoBehaviour
{
    private List<IslandCell> islandCells; 
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh terrainMesh;

    private LowPolyTerrain terrainGenerator;

    public Gradient humidityGradient;   
    public Gradient temperatureGradient; 

    void Start()
    {
        terrainGenerator = GetComponent<LowPolyTerrain>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        GenerateIslandMap();
    }

    
    public void GenerateIslandMap()
    {
        islandCells = new List<IslandCell>();
        terrainMesh = meshFilter.mesh;

        
        Vector3[] vertices = terrainMesh.vertices;
        int[] triangles = terrainMesh.triangles;
        Color[] colors = terrainMesh.colors;

        
        for (int i = 0; i < triangles.Length; i += 6)
        {
            
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];
            Vector3 v3 = vertices[triangles[i + 3]];
            Vector3 v4 = vertices[triangles[i + 4]];
            Vector3 v5 = vertices[triangles[i + 5]];

            
            float avgHeight = (v0.y + v1.y + v2.y + v3.y + v4.y + v5.y) / 6f;

            
            float humidity = Random.Range(0f, 1f);
            float temperature = Random.Range(0f, 1f);

            
            Color initialColor = EvaluateColor(avgHeight, humidity, temperature);

            
            IslandCell cell = new IslandCell(i / 6, avgHeight, humidity, temperature, initialColor);
            islandCells.Add(cell);
        }

        UpdateTerrainColors();
    }

    
    private Color EvaluateColor(float height, float humidity, float temperature)
    {
        
        Color humidityColor = humidityGradient.Evaluate(humidity);
        Color temperatureColor = temperatureGradient.Evaluate(temperature);

        
        Color finalColor = Color.Lerp(humidityColor, temperatureColor, 0.5f);

        
        finalColor *= Mathf.InverseLerp(terrainGenerator.minNoiseHeight, terrainGenerator.maxNoiseHeight, height);

        return finalColor;
    }

    
    public void UpdateTerrainColors()
    {
        Color[] colors = terrainMesh.colors;

        foreach (IslandCell cell in islandCells)
        {
            
            for (int j = 0; j < 6; j++)
            {
                colors[cell.index * 6 + j] = cell.color;
            }
        }

        
        terrainMesh.colors = colors;
    }

    
    public void UpdateCellProperties(int cellIndex, float newHumidity, float newTemperature)
    {
        if (cellIndex < 0 || cellIndex >= islandCells.Count)
        {
            Debug.LogError("Index de cellule invalide.");
            return;
        }

        IslandCell cell = islandCells[cellIndex];
        cell.humidity = newHumidity;
        cell.temperature = newTemperature;

        
        cell.color = EvaluateColor(cell.height, newHumidity, newTemperature);

        
        UpdateTerrainColors();
    }
}
