using System;
using System.Collections;
using System.Collections.Generic;
using ProceduralTerrainGeneration.Data;
using UnityEngine;

namespace ProceduralTerrainGeneration {
	public static class BiomeMapGenerator {
		public static BiomeMap GenerateBiomeMap(int width, int height, BiomeMapSettings settings, Vector2 sampleCentre) {
			int offset = settings.smoothingRadius * 2;
			
			int[,] map = new int[width + offset, height + offset];

			float[,] values = Noise.GenerateNoiseMap (width + offset, height + offset, settings.noiseSettings, sampleCentre);
			
			for (int x = 0; x < width + offset; x++) {
				for (int y = 0; y < height + offset; y++) {
					for (int biomeIndex = 0; biomeIndex < settings.Biomes.Length; biomeIndex++) {
						if (values [x, y] <= settings.Biomes [biomeIndex].startValue) {
							map [x, y] = biomeIndex;
							break;
						}
					}
				}
			}
		
			float[,] heightMult = removeOffset (BlurHeightMultipliers (settings, map), settings.smoothingRadius);
			map = removeOffset (map, settings.smoothingRadius);
			//TODO change blur to HLSL
			return new BiomeMap (map, settings.Biomes.Length, heightMult);
		}

		public static float[,] BlurHeightMultipliers(BiomeMapSettings settings, int[,] indexes) {
		
			int blurSize = settings.smoothingRadius;
			int gridSize = indexes.GetLength (0);
		
			int kernelSize = blurSize * 2 + 1;
			int kernelExtents = (kernelSize - 1) / 2;
		
			float[,] penaltiesHorizontalPass = new float[gridSize,gridSize];
			float[,] penaltiesVerticalPass = new float[gridSize,gridSize];
			float[,] values = new float[gridSize, gridSize];
		
			for (int y = 0; y < gridSize; y++) {
				for (int x = -kernelExtents; x <= kernelExtents; x++) {
					int sampleX = Mathf.Clamp (x, 0, kernelExtents);
					penaltiesHorizontalPass [0, y] += settings.Biomes[indexes[sampleX, y]].heightMult;
				}

				for (int x = 1; x < gridSize; x++) {
					int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSize);
					int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSize-1);

					penaltiesHorizontalPass [x, y] = penaltiesHorizontalPass [x - 1, y] - settings.Biomes[indexes[removeIndex, y]].heightMult + settings.Biomes[indexes[addIndex, y]].heightMult;
				}
			}
			
			for (int x = 0; x < gridSize; x++) {
				for (int y = -kernelExtents; y <= kernelExtents; y++) {
					int sampleY = Mathf.Clamp (y, 0, kernelExtents);
					penaltiesVerticalPass [x, 0] += penaltiesHorizontalPass [x, sampleY];
				}

				float blurredValues = penaltiesVerticalPass [x, 0] / (kernelSize * kernelSize);
				values [x, 0] = blurredValues;

				for (int y = 1; y < gridSize; y++) {
					int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSize);
					int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSize-1);

					penaltiesVerticalPass [x, y] = penaltiesVerticalPass [x, y-1] - penaltiesHorizontalPass [x,removeIndex] + penaltiesHorizontalPass [x, addIndex];
					blurredValues = penaltiesVerticalPass [x, y] / (kernelSize * kernelSize);
					values [x, y] = blurredValues;
				}
			}
			return values;
		}

		static T[,] removeOffset<T> (T[,] values, int offset) {
			int width = values.GetLength (0) - offset * 2;
			int height = values.GetLength (1) - offset * 2;
			T[,] output = new T[width, height];
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					output [i, j] = values [i + offset, j + offset];
				}
			}
			return output;
		}
	}


	public struct BiomeMap {
		public readonly int[,] biomeMapIndexes;
		public readonly int numBiomes;
		public readonly float[,] heightMultMat;

		public BiomeMap(int[,] biomeMapIndexes, int numBiomes, float[,] heightMultMat) {
			this.biomeMapIndexes = biomeMapIndexes;
			this.numBiomes = numBiomes;
			this.heightMultMat = heightMultMat;
		}

	}
}