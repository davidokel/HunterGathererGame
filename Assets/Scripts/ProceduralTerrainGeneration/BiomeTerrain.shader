Shader "Custom/BiomeTerrain" {
    Properties {
		testTexture("Texture", 2D) = "white"{}
    }
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxNumBiomes = 10;
		const static int maxLayerCount = 8;
		const static int maxMapSize = 2304;
		const static float epsilon = 1E-4;

		int numBiomes;
		int mapSize;
		float3 biomeColours[maxNumBiomes];
		float3 tileCentre;
		uniform int biomeMap[maxMapSize];
		float offset;

		float minHeight;
		float maxHeight;

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		float inverseLerp(float a, float b, float value) {
			return saturate((value-a)/(b-a));
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			int biomeX = round(IN.worldPos.x - tileCentre.x - offset);
			int biomeY = round(IN.worldPos.z - tileCentre.z - offset);
			
			int biome = biomeMap[23 * mapSize + 23];
			
			o.Albedo = biomeColours[biome];

		}
        ENDCG
    }
    FallBack "Diffuse"
}