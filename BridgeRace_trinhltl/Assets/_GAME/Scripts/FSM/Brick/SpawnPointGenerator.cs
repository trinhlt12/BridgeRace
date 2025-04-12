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
            if (this.spawnArea.GetComponent<Renderer>())
            {
                this.spawnPointBounds = this.spawnArea.GetComponent<Renderer>().bounds;
            }else if (this.spawnArea.GetComponent<Collider>())
            {
                spawnPointBounds = this.spawnArea.GetComponent<Collider>().bounds;
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
            this.spawnPoints.Clear();
            var attemps = 0;
            var maxAttempts = numberOfSpawnPoints * 10;



            while (this.spawnPoints.Count < this.numberOfSpawnPoints && attemps < maxAttempts)
            {
                var randomPos = GetRandomPositionInBounds();
                NavMeshHit hit;

                if (NavMesh.SamplePosition(randomPos, out hit, 1.0f, NavMesh.AllAreas))
                {
                    if (!IsNearOtherPoints(hit.position, this.spawnPoints, this.minDistanceBetweenPoints))
                    {
                        var spawnPoint = hit.position + Vector3.up * 0.1f;
                        this.spawnPoints.Add(spawnPoint);
                    }
                }

                attemps++;
            }

            Debug.Log($"Generated {spawnPoints.Count} spawn points after {attemps} attempts");
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