using UnityEngine;

namespace ProceduralTerrainGeneration.Data {
	[CreateAssetMenu(menuName = "World Generation/Biome")]
	public class Biome : UpdatableData{
		[Range(0,1)]
		public float startValue;
		public float heightMult = 1;
		public AnimationCurve heightCurve;
		public TextureData.Layer[] biomeLayers;
		public Color biomeColour;

		public Vector2 temperatureRange;
		public Vector2 precipitationRange;
	}
}
