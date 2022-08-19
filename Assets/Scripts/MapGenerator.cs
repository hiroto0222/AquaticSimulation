using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public enum DrawMode { NoiseMap, ColorMap, Mesh };
    public DrawMode drawMode;

    public const int mapChunkSize = 241;
    [Range(0, 6)]  // 1, 2, 4, 8, ...
    public int levelOfDetail;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] terrains;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseScale, seed, octaves, persistance, lacunarity, offset);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++) {
            for (int x = 0; x < mapChunkSize; x++) {
                float currHeight = noiseMap[x, y];
                foreach (TerrainType terrain in terrains) {
                    if (currHeight <= terrain.height) {
                        colorMap[y * mapChunkSize + x] = terrain.color;
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
            Texture2D colorTexture = TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize);
            display.DrawTexture(colorTexture);
        } else if (drawMode == DrawMode.Mesh) {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);
            Texture2D texture = TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize);
            display.DrawMesh(meshData, texture);
        }
    }

    // clamp vars
    private void OnValidate() {
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