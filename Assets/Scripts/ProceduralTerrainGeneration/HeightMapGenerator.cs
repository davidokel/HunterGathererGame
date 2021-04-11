using System.Collections;
using System.Collections.Generic;
using ProceduralTerrainGeneration;
using UnityEngine;

public static class HeightMapGenerator {

	public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre) {
		float[,] values = Noise.GenerateNoiseMap (width, height, settings.noiseSettings, sampleCentre);
		AnimationCurve heightCurve_threadsafe = new AnimationCurve (settings.heightCurve.keys);
		float minValue = float.MaxValue;
		float maxValue = float.MinValue;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				values [i, j] *= heightCurve_threadsafe.Evaluate (values [i, j]) * settings.heightMultiplier;

				if (values [i, j] > maxValue) {
					maxValue = values [i, j];
				}
				if (values [i, j] < minValue) {
					minValue = values [i, j];
				}
			}
		}

		return new HeightMap (values, minValue, maxValue);
	}

	public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre, BiomeMapSettings biomeMapSettings) {
		float[,] values = Noise.GenerateNoiseMap (width, height, settings.noiseSettings, sampleCentre);
		BiomeMap biomeMap = BiomeMapGenerator.GenerateBiomeMap (width, height, biomeMapSettings, sampleCentre);
		AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);
		AnimationCurve[] biomeCurves_threadsafe = new AnimationCurve[biomeMap.numBiomes];
		for (int i = 0; i < biomeMap.numBiomes; i++) {
			biomeCurves_threadsafe[i] = new AnimationCurve(biomeMapSettings.Biomes[i].heightCurve.keys);
		}
		
		float minValue = float.MaxValue;
		float maxValue = float.MinValue;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				int biomeIndex = biomeMap.biomeMapIndexes [i, j];
				float biomeScalar = GetSmoothedMean(biomeMap, biomeMapSettings, new Vector2(i, j)); 
				values [i, j] *= heightCurve_threadsafe.Evaluate (values [i, j]) * settings.heightMultiplier * biomeScalar;
				values [i, j] *= biomeCurves_threadsafe[biomeIndex].Evaluate (values [i, j]);
				if (values [i, j] > maxValue) {
					maxValue = values [i, j];
				}
				if (values [i, j] < minValue) {
					minValue = values [i, j];
				}
			}
		}

		return new HeightMap (values, minValue, maxValue, biomeMap);
	}

	private static float GetSmoothedMean(BiomeMap map, BiomeMapSettings settings, Vector2 coords) {
		int x = (int) coords.x;
		int y = (int) coords.y;
		
		if (settings.smoothingRadius == 0 || map.borderCoords[x,y] == 0) {
			return settings.Biomes [map.biomeMapIndexes [x, y]].heightMult;
		} 
		
		int width = map.biomeMapIndexes.GetLength (0);
		int height = map.biomeMapIndexes.GetLength (1);
		int radNegX = (x - settings.smoothingRadius < 0) ? x : settings.smoothingRadius;
		int radPosX = (x + settings.smoothingRadius > width) ? width - x : settings.smoothingRadius;
		int radNegY = (y - settings.smoothingRadius < 0) ? y : settings.smoothingRadius;
		int radPosY = (y + settings.smoothingRadius > height) ? height - y : settings.smoothingRadius;

		float sum = 0;
		int count = 0;
		for (int i = x - radNegX; i < x + radPosX; i++) {
			for (int j = y - radNegY; j < y + radPosY; j++) {
				sum += settings.Biomes [map.biomeMapIndexes [i, j]].heightMult;
				count++;
			}
		}
		return sum / count;
	}
}

public struct HeightMap {
	public readonly float[,] values;
	public readonly float minValue;
	public readonly float maxValue;
	public BiomeMap biomeMap;
	
	

	public HeightMap (float[,] values, float minValue, float maxValue, BiomeMap biomeMap)
	{
		this.values = values;
		this.minValue = minValue;
		this.maxValue = maxValue;
		this.biomeMap = biomeMap;
	}

	public HeightMap(float[,] values, float minValue, float maxValue)
	{
		this.values = values;
		this.minValue = minValue;
		this.maxValue = maxValue;
		this.biomeMap = new BiomeMap();
	}
}

