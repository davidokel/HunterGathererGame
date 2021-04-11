using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralTerrainGeneration
{
	public static class BiomeMapGenerator
	{
		public static BiomeMap GenerateBiomeMap(int width, int height, BiomeMapSettings settings,Vector2 sampleCentre) {
			float[,] values = Noise.GenerateNoiseMap (width, height, settings.noiseSettings, sampleCentre);
			int[,] map = new int[width, height];
			int[,] edgeCoords = new int[width, height];
		
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					for (int biomeIndex = 0; biomeIndex < settings.Biomes.Length; biomeIndex++) {
						if (values[x,y] <= settings.Biomes[biomeIndex].startValue) {
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
								Console.WriteLine(e);
							}
						}
				}
			}

			return new BiomeMap (map,settings.Biomes.Length,edgeCoords);
		}
	}

	public struct BiomeMap {
		public readonly int[,] biomeMapIndexes;
		public readonly int numBiomes;
		public readonly int[,] borderCoords;

		public BiomeMap(int[,] biomeMapIndexes, int numBiomes, int[,] borderCoords) {
			this.biomeMapIndexes = biomeMapIndexes;
			this.numBiomes = numBiomes;
			this.borderCoords = borderCoords;
		}

	}
}