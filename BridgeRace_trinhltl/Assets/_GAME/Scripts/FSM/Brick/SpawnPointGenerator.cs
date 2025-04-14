using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.FSM.Brick
{
    public class SpawnPointGenerator : MonoBehaviour
    {
        [SerializeField] private int numberOfSpawnPoints = 100;
        [SerializeField] private float minDistanceBetweenPoints = 1.5f;
        [SerializeField] private GameObject spawnArea;
        public Bounds spawnPointBounds;
        [SerializeField] private bool visualizePoints = true;
        [SerializeField] private float spacingX = 1.5f;
        [SerializeField] private float spacingZ = 1.5f;

        private List<Vector3> spawnPoints = new List<Vector3>();
        private List<Vector3> availableSpawnPoints = new List<Vector3>();

        private void Awake()
        {
            if (this.spawnArea.GetComponentInChildren<Renderer>())
            {
                this.spawnPointBounds = this.spawnArea.GetComponentInChildren<Renderer>().bounds;
            }
            else if (this.spawnArea.GetComponentInChildren<Collider>())
            {
                spawnPointBounds = this.spawnArea.GetComponentInChildren<Collider>().bounds;
            }

            GenerateSpawnPoints();
        }

        public void GenerateSpawnPoints()
        {
            spawnPoints.Clear();

            var pointsX = Mathf.FloorToInt(spawnPointBounds.size.x / spacingX);
            var pointsZ = Mathf.FloorToInt(spawnPointBounds.size.z / spacingZ);

            pointsX = Mathf.Max(pointsX, 5);
            pointsZ = Mathf.Max(pointsZ, 5);

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
                    var searchRadius = Mathf.Max(spacingX, spacingZ) * 0.5f;
                    if (NavMesh.SamplePosition(pointPosition, out hit, searchRadius, NavMesh.AllAreas))
                    {
                        var spawnPoint = hit.position + Vector3.up * 0.1f;
                        spawnPoints.Add(spawnPoint);
                        Debug.DrawLine(pointPosition, spawnPoint, Color.green, 10f);
                    }
                    else
                    {
                        Debug.DrawLine(pointPosition, pointPosition + Vector3.up, Color.red, 10f);
                    }
                }
            }

            Debug.Log($"Generated {spawnPoints.Count} spawn points with spacing ({spacingX}, {spacingZ})");

            RefreshAvailableSpawnPoints();
        }

        public void RefreshAvailableSpawnPoints()
        {
            availableSpawnPoints = new List<Vector3>(spawnPoints);
            Debug.Log($"Refreshed available spawn points: {availableSpawnPoints.Count}");
        }

        public List<Vector3> GetSpawnPoints()
        {
            return spawnPoints;
        }

        public List<Vector3> GetRandomSpawnPoints(int count)
        {
            if (availableSpawnPoints.Count == 0)
            {
                RefreshAvailableSpawnPoints();
                Debug.Log("Refreshed available spawn points pool");
            }

            count = Mathf.Min(count, availableSpawnPoints.Count);

            var points = new List<Vector3>();

            for (var i = 0; i < count; i++)
            {
                if (availableSpawnPoints.Count == 0) break;

                var randomIndex = Random.Range(0, availableSpawnPoints.Count);
                points.Add(availableSpawnPoints[randomIndex]);
                availableSpawnPoints.RemoveAt(randomIndex);
            }

            Debug.Log($"Retrieved {points.Count} random spawn points, {availableSpawnPoints.Count} remaining");
            return points;
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
    }
}