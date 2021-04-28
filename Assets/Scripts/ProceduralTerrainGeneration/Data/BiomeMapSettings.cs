using UnityEngine;

namespace ProceduralTerrainGeneration.Data {
    [CreateAssetMenu(menuName = "Terrain Data/Biome Map Settings")]
    public class BiomeMapSettings : UpdatableData
    {
        public NoiseSettings noiseSettings;

        [Range (0, 10)]
        public int smoothingRadius;
        public Biome[] Biomes;

        public float minHeight {
            get {
                float height = float.MaxValue;
                float bHeight;
                foreach (var biome in Biomes) {
                    bHeight = biome.heightCurve.Evaluate (0) * biome.heightMult;
                    if (bHeight < height)
                        height = bHeight;
                }
                return height;
            }
        }
    
        public float maxHeight {
            get {
                float height = float.MinValue;
                float bHeight;
                foreach (var biome in Biomes) {
                    bHeight = biome.heightCurve.Evaluate (1) * biome.heightMult;
                    if (bHeight > height)
                        height = bHeight;
                }
                return height;
            }
        } 
    
#if UNITY_EDITOR
        protected override void OnValidate() {
            noiseSettings.ValidateValues ();
            base.OnValidate ();
        }
#endif
    
    
    }
}

