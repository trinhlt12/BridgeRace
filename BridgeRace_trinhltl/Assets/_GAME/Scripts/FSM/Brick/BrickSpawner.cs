using System;
using System.Collections.Generic;
using UnityEngine;

namespace _GAME.Scripts.FSM.Brick
{
    internal class Vector3Comparer : IEqualityComparer<Vector3>
    {
        public bool Equals(Vector3 a, Vector3 b)
        {
            // Consider positions equal if they are very close
            return Vector3.Distance(a, b) < 0.1f;
        }

        public int GetHashCode(Vector3 position)
        {
            // Create a reasonable hash code for Vector3
            return Mathf.RoundToInt(position.x * 100) ^ Mathf.RoundToInt(position.y * 100) << 2 ^ Mathf.RoundToInt(position.z * 100) << 4;
        }
    }

    public class BrickSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnPointGenerator _spawnPointGenerator;
        [SerializeField] private int                 maxBricksPerColor     = 25;
        [SerializeField] private int                 bricksToSpawnPerBatch = 5;
        [SerializeField] private int                 minBricksPerColor     = 5;

        public static BrickSpawner Instance { get; private set; }

        private BrickPoolManager                    _brickPoolManager;
        private Dictionary<BrickColor, List<Brick>> _activeBricks      = new Dictionary<BrickColor, List<Brick>>();
        private HashSet<Vector3>                    _occupiedPositions = new HashSet<Vector3>(new Vector3Comparer());

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            this.OnInit();
        }

        private void OnInit()
        {
            foreach (var color in Enum.GetValues(typeof(BrickColor)))
            {
                _activeBricks[(BrickColor)color] = new List<Brick>();
            }
        }

        private void Start()
        {
            if (BrickPoolManager.Instance == null)
            {
                Debug.LogError("BrickPoolManager.Instance is null! Make sure it's initialized before BrickSpawner.");
                return;
            }

            _brickPoolManager = BrickPoolManager.Instance;

            SpawnAllBricks();
        }

        public bool IsPositionOccupied(Vector3 position)
        {
            return _occupiedPositions.Contains(position);
        }

        public void SpawnAllBricks()
        {
            var allPoints   = _spawnPointGenerator.GetSpawnPoints();
            var totalColors = Enum.GetValues(typeof(BrickColor)).Length;

            Debug.Log($"Total spawn points: {allPoints.Count}, Total colors: {totalColors}");

            if (allPoints.Count < totalColors * minBricksPerColor)
            {
                Debug.LogWarning($"Not enough spawn points ({allPoints.Count}) for all colors ({totalColors}). " + $"Consider reducing spacing or increasing spawn area size!");
            }

            var spawnPointsPerColor = AllocateSpawnPointsPerColor();

            foreach (BrickColor color in Enum.GetValues(typeof(BrickColor)))
            {
                if (spawnPointsPerColor.ContainsKey(color) && spawnPointsPerColor[color].Count > 0)
                {
                    if (color.Equals(BrickColor.Grey))
                    {
                        continue; // Skip spawning grey bricks
                    }
                    SpawnBricksOfColorAtPoints(color, spawnPointsPerColor[color]);
                }
            }

            LogActiveBrickCounts();
        }

        private Dictionary<BrickColor, List<Vector3>> AllocateSpawnPointsPerColor()
        {
            var result             = new Dictionary<BrickColor, List<Vector3>>();
            var allAvailablePoints = new List<Vector3>(_spawnPointGenerator.GetSpawnPoints());

            var colorCount = Enum.GetValues(typeof(BrickColor)).Length - 1; // Exclude Grey

            var pointsPerColor = Mathf.Max(minBricksPerColor, allAvailablePoints.Count / colorCount);

            foreach (BrickColor color in Enum.GetValues(typeof(BrickColor)))
            {
                if (color.Equals(BrickColor.Grey))
                {
                    continue;
                }
                result[color] = new List<Vector3>();
            }

            var colorIndex = 0;
            var colors     = (BrickColor[])Enum.GetValues(typeof(BrickColor));

            while (allAvailablePoints.Count > 0 && colorIndex < colorCount)
            {
                var currentColor = colors[colorIndex];

                if (result[currentColor].Count < maxBricksPerColor)
                {
                    var randomIndex = UnityEngine.Random.Range(0, allAvailablePoints.Count);
                    result[currentColor].Add(allAvailablePoints[randomIndex]);
                    allAvailablePoints.RemoveAt(randomIndex);
                }

                colorIndex = (colorIndex + 1) % colorCount;
            }

            return result;
        }

        private void SpawnBricksOfColorAtPoints(BrickColor color, List<Vector3> points)
        {
            Material brickMaterial = null;
            if (MaterialManager.Instance != null)
            {
                brickMaterial = MaterialManager.Instance.GetMaterial(color);
            }
            else
            {
                Debug.LogWarning("MaterialManager.Instance is null");
            }

            foreach (var point in points)
            {
                if (IsPositionOccupied(point))
                {
                    return;
                }
                var brick = _brickPoolManager.SpawnBrick(color, point + Vector3.down * 0.15f);
                if (brick != null)
                {
                    brick.Initialize(color, brickMaterial);
                    _activeBricks[color].Add(brick);
                }
            }
        }

        /*private void SpawnBricksOfColor(BrickColor color, int count)
        {
            if (_brickPoolManager == null)
            {
                Debug.LogError("_brickPoolManager is null in SpawnBricksOfColor");
                return;
            }

            if (_spawnPointGenerator == null)
            {
                Debug.LogError("_spawnPointGenerator is null in SpawnBricksOfColor");
                return;
            }

            _spawnPointGenerator.RefreshAvailableSpawnPoints();

            var spawnPoints = _spawnPointGenerator.GetRandomSpawnPoints(count);
            Debug.Log($"Spawning {spawnPoints.Count} bricks of color {color}");

            Material brickMaterial = null;
            if (MaterialManager.Instance != null)
            {
                brickMaterial = MaterialManager.Instance.GetMaterial(color);
            }
            else
            {
                Debug.LogWarning("MaterialManager.Instance is null");
            }

            foreach (var spawnPoint in spawnPoints)
            {
                var brick = _brickPoolManager.SpawnBrick(color, spawnPoint + Vector3.down * 0.15f);
                if (brick != null)
                {
                    brick.Initialize(color, brickMaterial);
                    _activeBricks[color].Add(brick);
                }
            }
        }*/

        private void LogActiveBrickCounts()
        {
            string brickCounts = "Active bricks: ";
            foreach (BrickColor color in Enum.GetValues(typeof(BrickColor)))
            {
                brickCounts += $"{color}: {_activeBricks[color].Count}, ";
            }
            Debug.Log(brickCounts);
        }

        public void RemoveBrick(Brick brick)
        {
            if (_activeBricks.ContainsKey(brick.Color))
            {
                _activeBricks[brick.Color].Remove(brick);
                _occupiedPositions.Remove(brick.transform.position);
            }
            brick.ReturnToPool();
        }

        private Vector3 FindEmptySpawnPoint()
        {
            var allSpawnPoints       = _spawnPointGenerator.GetSpawnPoints();
            var availableSpawnPoints = new List<Vector3>();

            foreach (var point in allSpawnPoints)
            {
                if (!IsPositionOccupied(point))
                {
                    availableSpawnPoints.Add(point);
                }
            }

            if (availableSpawnPoints.Count > 0)
            {
                var randomIndex = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
                return availableSpawnPoints[randomIndex];
            }

            return Vector3.zero; // No available spawn points
        }

        public void RespawnBrick(Brick brick)
        {
            if (_activeBricks.ContainsKey(brick.Color))
            {
                _activeBricks[brick.Color].Remove(brick);
                _occupiedPositions.Remove(brick.transform.position);
            }

            brick.gameObject.GetComponent<Collider>().enabled = true;

            var newPosition = FindEmptySpawnPoint();

            if (newPosition != Vector3.zero)
            {
                brick.transform.position = newPosition;
                this._occupiedPositions.Add(newPosition);
                this._activeBricks[brick.Color].Add(brick);
                brick.gameObject.SetActive(true);
            }
            else
            {
                brick.ReturnToPool();
            }
        }
    }
}