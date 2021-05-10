using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralTerrainGeneration.Data {
	[CreateAssetMenu(menuName = "World Generation/Biome")]
	public class Biome : UpdatableData{
		public float heightMult = 1;
		public AnimationCurve heightCurve;
		public TextureData.Layer[] biomeLayers;
		public Color biomeColour;

		public List<string> resourceTagsList;

		public bool isOcean;

		public Vector2 maxAndMinTemperature;
		public Vector2 maxAndMinPrecipitation;
	}
}
