using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.FSM.Brick
{
    public class SpawnPointGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject spawnArea;
        public Bounds spawnPointBounds;
        [SerializeField] private bool visualizePoints = true;
        [SerializeField] private float spacingX = 1.5f;
        [SerializeField] private float spacingZ = 1.5f;

        private readonly List<Vector3> spawnPoints          = new List<Vector3>();
        private          List<Vector3> availableSpawnPoints = new List<Vector3>();
        public           bool[]        _spawnPointAvailability;

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
            Debug.Log($"Spawn area bounds: center={spawnPointBounds.center}, size={spawnPointBounds.size}");

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
                        this.spawnArea.transform.position.y,
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

            _spawnPointAvailability = new bool[spawnPoints.Count];
            for (var i = 0; i < this.spawnPoints.Count; i++)
            {
                _spawnPointAvailability[i] = true;
            }

            Debug.Log($"Generated {spawnPoints.Count} spawn points with spacing ({spacingX}, {spacingZ})");

            RefreshAvailableSpawnPoints();
        }

        public void SetSpawnPointAvailability(int index, bool isAvailable)
        {
            if (index >= 0 && index < this._spawnPointAvailability.Length)
            {
                _spawnPointAvailability[index] = isAvailable;
            }
        }

        public int GetSpawnPointIndex(Vector3 position)
        {
            for(var i = 0; i < spawnPoints.Count; i++)
            {
                if (Vector3.Distance(spawnPoints[i], position) < 0.1f)
                {
                    return i;
                }
            }
            return -1;
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
            var availableIndices = new List<int>();

            for (var i = 0; i < _spawnPointAvailability.Length; i++)
            {
                if (_spawnPointAvailability[i])
                {
                    availableIndices.Add(i);
                }
            }

            var points = new List<Vector3>();
            count = Mathf.Min(count, availableIndices.Count);

            for (var i = 0; i < count; i++)
            {
                if (availableIndices.Count == 0) break;

                var randomIndexPosition = Random.Range(0, availableIndices.Count);
                var spawnPointIndex     = availableIndices[randomIndexPosition];

                points.Add(spawnPoints[spawnPointIndex]);
                _spawnPointAvailability[spawnPointIndex] = false;
                availableIndices.RemoveAt(randomIndexPosition);
            }

            Debug.Log($"Retrieved {points.Count} random spawn points, {availableIndices.Count} remaining available");
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