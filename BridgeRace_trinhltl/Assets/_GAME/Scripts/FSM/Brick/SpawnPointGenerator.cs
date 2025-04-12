namespace _GAME.Scripts.FSM.Brick
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using Random = UnityEngine.Random;

    public class SpawnPointGenerator : MonoBehaviour
    {
        [SerializeField] private int        numberOfSpawnPoints      = 100;
        [SerializeField] private float      minDistanceBetweenPoints = 1f;
        [SerializeField] private GameObject spawnArea;
        public Bounds     spawnPointBounds;
        [SerializeField] private bool       visualizePoints = true;

        private List<Vector3> spawnPoints = new List<Vector3>();

        private void Awake()
        {
            if (this.spawnArea.GetComponentInChildren<Renderer>())
            {
                this.spawnPointBounds = this.spawnArea.GetComponentInChildren<Renderer>().bounds;
            }else if (this.spawnArea.GetComponentInChildren<Collider>())
            {
                spawnPointBounds = this.spawnArea.GetComponentInChildren<Collider>().bounds;
            }

            GenerateSpawnPoints();
        }

        private Vector3 GetRandomPositionInBounds()
        {
            return new Vector3(
                Random.Range(spawnPointBounds.min.x, spawnPointBounds.max.x),
                0,
                Random.Range(spawnPointBounds.min.z, spawnPointBounds.max.z)
            );
        }

        private bool IsNearOtherPoints(Vector3 position, List<Vector3> points, float minDistance)
        {
            foreach (var point in points)
            {
                if (Vector3.Distance(position, point) < minDistance)
                {
                    return true;
                }
            }
            return false;
        }

        public void GenerateSpawnPoints()
        {
            spawnPoints.Clear();

            var spacingX = 1f;
            var spacingZ = 1f;

            var pointsX = Mathf.FloorToInt(spawnPointBounds.size.x / spacingX);
            var pointsZ = Mathf.FloorToInt(spawnPointBounds.size.z / spacingZ);

            var startPoint = new Vector3(
                spawnPointBounds.min.x + spacingX / 2,
                0,
                spawnPointBounds.min.z + spacingZ / 2
            );

            for (var x = 0; x < pointsX; x++)
            {
                for (var z = 0; z < pointsZ; z++)
                {
                    var pointPosition = new Vector3(
                        startPoint.x + x * spacingX,
                        0,
                        startPoint.z + z * spacingZ
                    );

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(pointPosition, out hit, 1.0f, NavMesh.AllAreas))
                    {
                        var spawnPoint = hit.position + Vector3.up * 0.1f;
                        spawnPoints.Add(spawnPoint);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!this.visualizePoints || this.spawnPoints == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;

            Gizmos.DrawWireCube(this.spawnPointBounds.center, this.spawnPointBounds.size);

            Gizmos.color = Color.red;

            foreach (var point in this.spawnPoints)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }
        }

        public List<Vector3> GetSpawnPoints()
        {
            return this.spawnPoints;
        }

        public List<Vector3> GetRandomSpawnPoints(int count)
        {
            if (count >= this.spawnPoints.Count)
            {
                return new List<Vector3>(this.spawnPoints);
            }

            var points = new List<Vector3>();
            var indices = new List<int>();

            for(var i = 0; i < this.spawnPoints.Count; i++)
            {
                indices.Add(i);
            }

            for (var i = 0; i < count; i++)
            {
                var randomIndex = Random.Range(0, indices.Count);
                points.Add(this.spawnPoints[indices[randomIndex]]);
                indices.RemoveAt(randomIndex);
            }
            return points;
        }
    }
}