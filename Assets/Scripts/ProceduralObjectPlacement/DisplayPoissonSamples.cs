using System.Collections.Generic;
using UnityEngine;

namespace ProceduralObjectPlacement {
   public class DisplayPoissonSamples : MonoBehaviour {
      public float radius = 1;
      public float displayRadius = 1;
      public Vector2 gridSize = Vector2.one;
      public int rejectionValue = 30;

      private List<Vector2> points;

      private void OnValidate() {
         points = PoissonDiscSampling.GeneratePoints (radius, gridSize, new Vector2(0,0) , rejectionValue);
      }

      private void OnDrawGizmos() {
         Gizmos.DrawWireCube(new Vector3(gridSize.x / 2, 0, gridSize.y / 2),new Vector3(gridSize.x, 0, gridSize.y));
         if (points != null) {
            foreach (var point in points) {
               Gizmos.DrawSphere(new Vector3(point.x, 0, point.y),displayRadius);
            }
         }
      }
   }
}
