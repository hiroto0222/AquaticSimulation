using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public enum DrawMode { NoiseMap, ColorMap };
    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public TerrainType[] terrains;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, seed, octaves, persistance, lacunarity, offset);
        Color[] colorMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                float currHeight = noiseMap[x, y];
                foreach (TerrainType terrain in terrains) {
                    if (currHeight <= terrain.height) {
                        colorMap[y * mapWidth + x] = terrain.color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap) {
            Texture2D noiseTexture = TextureGenerator.TextureFromNoiseMap(noiseMap);
            display.DrawTexture(noiseTexture);
        } else if (drawMode == DrawMode.ColorMap) {
            Texture2D colorTexture = TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight);
            display.DrawTexture(colorTexture);
        }
    }

    // clamp vars
    private void OnValidate() {
        if (mapWidth < 1) mapWidth = 1;
        if (mapHeight < 1) mapHeight = 1;
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}