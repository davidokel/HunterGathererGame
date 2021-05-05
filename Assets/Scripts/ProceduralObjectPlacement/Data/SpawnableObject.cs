using UnityEngine;

namespace ProceduralObjectPlacement.Data {
	[CreateAssetMenu(menuName = "World Generation/Spawnable Resource")]
	public class SpawnableObject : ScriptableObject {
		public bool isClusterSpawned;
		public PoissonDistributionSettings distributionSettings;
		public ClusterSettings clusterSettings;
		public GameObject[] models;
		

		public GameObject GetRandomGameObject() {
			return models [Random.Range (0, models.Length)];
		}
	}

	[System.Serializable]
	public class PoissonDistributionSettings {
		public float minimumRadius = 6;
		public int rejectionValue = 30;
		
	}
	
	[System.Serializable]
	public class ClusterSettings : PoissonDistributionSettings {
		public Vector2 clusterSize = Vector2.one;
	}
}
