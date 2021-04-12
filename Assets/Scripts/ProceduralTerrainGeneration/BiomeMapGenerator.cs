using System;
using System.Collections;
using System.Collections.Generic;
using ProceduralTerrainGeneration;
using UnityEngine;

public static class BiomeMapGenerator {
	public static BiomeMap GenerateBiomeMap(int width, int height, BiomeMapSettings settings, Vector2 sampleCentre) {
		float[,] values = Noise.GenerateNoiseMap (width, height, settings.noiseSettings, sampleCentre);
		int[,] map = new int[width, height];
		int[,] edgeCoords = new int[width, height];


		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				for (int biomeIndex = 0; biomeIndex < settings.Biomes.Length; biomeIndex++) {
					if (values [x, y] <= settings.Biomes [biomeIndex].startValue) {
						map [x, y] = biomeIndex;
						break;
					}
				}
			}
		}

		
		for (int x = 1; x < width; x++)
		{
			for (int y = 1; y < height; y++)
			{
				if (map[x, y] != map[x - 1, y - 1])
					for (int i = -settings.smoothingRadius; i < settings.smoothingRadius; i++)
					{
						try
						{
							edgeCoords[x + i, y + i] = 1;
						}
						catch (Exception e)
						{
							Debug.unityLogger.Log(e);
						}
					}
			}
		}
		
		float[,] heightMult =
			MatrixConvolution (map, GaussianBlurKernel (settings.smoothingRadius, settings.smoothingWeight), edgeCoords);


		return new BiomeMap (map, settings.Biomes.Length, edgeCoords, heightMult);
	}


	public static float[,] GaussianBlurKernel(int length, float weight) {
		float[,] kernel = new float[length, length];
		float kernelSum = 0;
		int foff = (length - 1) / 2;
		float distance = 0;
		float constant = 1f / (2 * (float)Math.PI * weight * weight);
		for (int y = -foff; y <= foff; y++) {
			for (int x = -foff; x <= foff; x++) {
				distance = ((y * y) + (x * x)) / (2 * weight * weight);
				kernel [y + foff, x + foff] = constant * (float)Math.Exp (-distance);
				kernelSum += kernel [y + foff, x + foff];
			}
		}
		for (int y = 0; y < length; y++) {
			for (int x = 0; x < length; x++) {
				kernel [y, x] = kernel [y, x] / kernelSum;
			}
		}
		return kernel;
	}

	public static float[,] MatrixConvolution(int[,] values, float[,] kernel, int[,] selectedCoords) {
		int size = values.GetLength (0);
		int kernelMidPoint = kernel.GetLength (0) / 2 - 1;
		float[,] output = new float[size, size];

		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				if (selectedCoords [x, y] != 0) {
					float accumulator = 0;

					for (int i = -kernelMidPoint; i < kernelMidPoint; i++) {
						for (int j = -kernelMidPoint; j < kernelMidPoint; j++) {
							try {
								accumulator += kernel [kernelMidPoint + i, kernelMidPoint + j] * values [x + i, y + j];
							}
							catch (Exception e) {
								Console.WriteLine (e);
							}
						}
					}
					output [x, y] = accumulator;
				} else {
					output [x, y] = values [x, y];
				}
			}
		}
		return output;
	}
}


public struct BiomeMap {
	public readonly int[,] biomeMapIndexes;
	public readonly int numBiomes;
	public readonly float[,] heightMultMat;

	public BiomeMap(int[,] biomeMapIndexes, int numBiomes, int[,] borderCoords, float[,] heightMultMat) {
		this.biomeMapIndexes = biomeMapIndexes;
		this.numBiomes = numBiomes;
		this.heightMultMat = heightMultMat;
	}

}