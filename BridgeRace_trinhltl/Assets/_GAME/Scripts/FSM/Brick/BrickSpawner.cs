using System;
using System.Collections.Generic;
using UnityEngine;

namespace _GAME.Scripts.FSM.Brick
{
    public class BrickSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnPointGenerator _spawnPointGenerator;
        [SerializeField] private int maxBricksPerColor = 25;
        [SerializeField] private int bricksToSpawnPerBatch = 5;
        [SerializeField] private int minBricksPerColor = 5;

        private BrickPoolManager _brickPoolManager;
        private Dictionary<BrickColor, List<Brick>> _activeBricks = new Dictionary<BrickColor, List<Brick>>();

        private void Awake()
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

        public void SpawnAllBricks()
        {
            var allPoints = _spawnPointGenerator.GetSpawnPoints();
            var totalColors = Enum.GetValues(typeof(BrickColor)).Length;

            Debug.Log($"Total spawn points: {allPoints.Count}, Total colors: {totalColors}");

            if (allPoints.Count < totalColors * minBricksPerColor)
            {
                Debug.LogWarning($"Not enough spawn points ({allPoints.Count}) for all colors ({totalColors}). " +
                    $"Consider reducing spacing or increasing spawn area size!");
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
            var result = new Dictionary<BrickColor, List<Vector3>>();
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
            var colors = (BrickColor[])Enum.GetValues(typeof(BrickColor));

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
                var brick = _brickPoolManager.SpawnBrick(color, point + Vector3.down * 0.15f);
                if (brick != null)
                {
                    brick.Initialize(color, brickMaterial);
                    _activeBricks[color].Add(brick);
                }
            }
        }

        private void SpawnBricksOfColor(BrickColor color, int count)
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
        }

        public void RespawnBricks()
        {
            var spawnPointsPerColor = AllocateSpawnPointsPerColor();

            foreach (BrickColor color in Enum.GetValues(typeof(BrickColor)))
            {
                if (spawnPointsPerColor.ContainsKey(color) && spawnPointsPerColor[color].Count > 0)
                {
                    int spawnCount = Mathf.Min(spawnPointsPerColor[color].Count,
                                               maxBricksPerColor - _activeBricks[color].Count);

                    if (spawnCount > 0)
                    {
                        List<Vector3> pointsToUse = spawnPointsPerColor[color].GetRange(0, spawnCount);
                        SpawnBricksOfColorAtPoints(color, pointsToUse);
                    }
                }
            }

            LogActiveBrickCounts();
        }

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
            }
            brick.ReturnToPool();
        }
    }
}