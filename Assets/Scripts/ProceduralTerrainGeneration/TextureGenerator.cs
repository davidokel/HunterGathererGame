using UnityEngine;
using System.Collections;
using ProceduralTerrainGeneration;

public static class TextureGenerator {

	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height) {
		Texture2D texture = new Texture2D (width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply ();
		return texture;
	}

	public static Texture2D TextureFromBiomeMap(BiomeMap biomeMap) {
		int width = biomeMap.biomeMapIndexes.GetLength (0);
		int height = biomeMap.biomeMapIndexes.GetLength (1);
		
		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				int val = biomeMap.biomeMapIndexes [x, y];
				colourMap [y * width + x] = new Color(val/(float)biomeMap.numBiomes,val/(float)biomeMap.numBiomes,val/(float)biomeMap.numBiomes);
			}
		}

		return TextureFromColourMap(colourMap,width,height);
	}
	
	public static Texture2D TextureFromHeightMap(HeightMap heightMap) {
		int width = heightMap.values.GetLength (0);
		int height = heightMap.values.GetLength (1);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue,heightMap.maxValue,heightMap.values [x, y]));
			}
		}

		return TextureFromColourMap (colourMap, width, height);
	}

}
