using System;
using System.Collections.Generic;
using ProceduralObjectPlacement.Data;
using ProceduralTerrainGeneration;
using ProceduralTerrainGeneration.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralObjectPlacement {
	public class ObjectSpawner : MonoBehaviour {
		public LayerMask groundMask;
		public float rayCastHeight;
		public SpawnableObject[] spawnableObjects;

		private Vector2 areaStart;
		private Vector2 areaSize;

		private TerrainGenerator _terrainGenerator;
		private Biome[] _biomes;

		private void Start() {
			_terrainGenerator = (TerrainGenerator) transform.GetComponent(typeof(TerrainGenerator));
			_biomes = _terrainGenerator.biomeMapSettings.Biomes;
			
			var bounds = GetComponent<Collider> ().bounds;
			var position = transform.position;
			float xCoord = position.x - bounds.size.x / 2;
			float zCoord = position.z - bounds.size.z / 2;
			areaStart = new Vector2 (xCoord, zCoord);
			areaSize = new Vector2 (bounds.size.x, bounds.size.z);
		}

		private void Update() {
			if (Input.GetKeyDown (KeyCode.LeftAlt)) {
				for (int i = transform.childCount - 1; i >= 0; i--) {
					Destroy (transform.GetChild (i).gameObject);
				}
				
				if (spawnableObjects != null && spawnableObjects.Length != 0) {
					foreach (var spawnable in spawnableObjects) {
						List<Vector2> points = PoissonDiscSampling.GeneratePoints (spawnable.distributionSettings.minimumRadius, areaSize, areaStart ,spawnable.distributionSettings.rejectionValue);
						if (spawnable.isClusterSpawned) {
							List<Vector2> clusterPoints = new List<Vector2> ();
							foreach (var point in points) {
								List<Vector2> clusterPointsIndex = PoissonDiscSampling.GeneratePoints (spawnable.clusterSettings.minimumRadius, spawnable.clusterSettings.clusterSize, point, spawnable.clusterSettings.rejectionValue);
								clusterPoints.AddRange(clusterPointsIndex);
							}
							points.AddRange(clusterPoints);
						}
						SpawnObjects (spawnable, points);
					}
				}
			}
		}

		public void SpawnObjects(SpawnableObject spawnable, List<Vector2> points) {
			foreach (var point in points) {
				Ray ray = new Ray (PointAtHeight(point, transform.position.y + rayCastHeight), Vector3.down);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, rayCastHeight * 2, groundMask.value) && BiomeAllowsSpawn(point, spawnable.tag)) {
					float worldHeight = hitInfo.point.y;
					GameObject newObject = Instantiate (spawnable.GetRandomGameObject (), transform, true);
					Vector3 rotation = new Vector3 (Random.Range (0, 5f), Random.Range (0, 360f), Random.Range (0, 5f));
					newObject.transform.position = PointAtHeight (point, worldHeight);
					newObject.transform.eulerAngles = rotation;
				}
			}
			
		}

		public bool BiomeAllowsSpawn(Vector2 point, string resourceTag) {
			TerrainChunk chunk = _terrainGenerator.GetChunkFromDictionary (point);
			int biomeIndex = chunk.GetBiomeIndex (point);

			return _biomes [biomeIndex].resourceTagsList.Contains (resourceTag);
		}

		public Vector3 PointAtHeight(Vector2 vector, float height) {
			return new Vector3 (vector.x, height, vector.y);
		}
	}
}
