using System;
using System.Collections.Generic;
using UnityEngine;
using _GAME.Scripts.Floor;

namespace _GAME.Scripts.FSM.Brick
{
    using Floor = _GAME.Scripts.Floor.Floor;

    public class BrickSpawner : MonoBehaviour
    {
        [SerializeField] private int maxBricksPerColor = 25;
        [SerializeField] private int minBricksPerColor = 5;
        public static BrickSpawner Instance { get; private set; }

        private BrickPoolManager _brickPoolManager;
        private Floor _currentFloor;

        public Dictionary<BrickColor, List<Brick>> _activeBricks = new Dictionary<BrickColor, List<Brick>>();

        private Dictionary<Brick, int> _brickToSpawnPointIndex = new Dictionary<Brick, int>();

        private SpawnPointGenerator _currentSpawnPointGenerator;

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
        }

        public void SetCurrentFloor(Floor floor)
        {
            _currentFloor = floor;

            _currentSpawnPointGenerator = floor.GetSpawnPointGenerator();
            if (this._currentSpawnPointGenerator == null)
            {
                Debug.LogError("No spawnpoint generator found");
            }
        }

        public void SpawnBricksForCharacters(List<Character> characters)
        {
            ClearAllBricks();

            var colorsToSpawn = new HashSet<BrickColor>();
            foreach (var character in characters)
            {
                if (!character.characterColor.Equals(BrickColor.Grey))
                {
                    colorsToSpawn.Add(character.characterColor);
                }
            }

            if (colorsToSpawn.Count == 0)
            {
                Debug.Log("No characters with valid colors on floor, skipping brick spawn");
                return;
            }

            var spawnPointsPerColor = AllocateSpawnPointsForColors(colorsToSpawn);

            foreach (var colorEntry in spawnPointsPerColor)
            {
                SpawnBricksOfColorAtPoints(colorEntry.Key, colorEntry.Value);
            }

            LogActiveBrickCounts();
        }

        private Dictionary<BrickColor, List<Vector3>> AllocateSpawnPointsForColors(HashSet<BrickColor> colors)
        {
            var result = new Dictionary<BrickColor, List<Vector3>>();
            var allAvailablePoints = new List<Vector3>(this._currentSpawnPointGenerator.GetSpawnPoints());

            if (colors.Count == 0)
                return result;

            var pointsPerColor = Mathf.Max(minBricksPerColor, allAvailablePoints.Count / colors.Count);
            pointsPerColor = Mathf.Min(pointsPerColor, maxBricksPerColor);

            foreach (var color in colors)
            {
                result[color] = new List<Vector3>();
            }

            var colorList = new List<BrickColor>(colors);
            var colorIndex = 0;

            while (allAvailablePoints.Count > 0 && colorList.Count > 0)
            {
                var currentColor = colorList[colorIndex];

                if (result[currentColor].Count < pointsPerColor)
                {
                    var randomIndex = UnityEngine.Random.Range(0, allAvailablePoints.Count);
                    result[currentColor].Add(allAvailablePoints[randomIndex]);
                    allAvailablePoints.RemoveAt(randomIndex);
                }
                else
                {
                    colorList.RemoveAt(colorIndex);
                    if (colorList.Count == 0)
                        break;
                    colorIndex = colorIndex % colorList.Count;
                    continue;
                }

                colorIndex = (colorIndex + 1) % colorList.Count;
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
                return;
            }

            foreach (var point in points)
            {
                var spawnPointIndex = this._currentSpawnPointGenerator.GetSpawnPointIndex(point);

                if (spawnPointIndex == -1 || !this._currentSpawnPointGenerator._spawnPointAvailability[spawnPointIndex])
                {
                    continue;
                }

                var brick = _brickPoolManager.SpawnBrick(color, point + Vector3.down * 0.15f);

                if (brick != null)
                {
                    brick.Initialize(color, brickMaterial);
                    brick.transform.SetParent(this._currentFloor._brickParent);

                    _activeBricks[color].Add(brick);

                    this._brickToSpawnPointIndex[brick] = spawnPointIndex;
                    this._currentSpawnPointGenerator.SetSpawnPointAvailability(spawnPointIndex, false);
                }
            }
        }

        private void ClearAllBricks()
        {
            var bricksCopy = new Dictionary<BrickColor, List<Brick>>();

            foreach (BrickColor color in Enum.GetValues(typeof(BrickColor)))
            {
                if (_activeBricks.TryGetValue(color, out var bricks))
                {
                    bricksCopy[color] = new List<Brick>(bricks);
                }
            }

            foreach (var colorBrickPair in bricksCopy)
            {
                foreach (var brick in colorBrickPair.Value)
                {
                    RemoveBrick(brick);
                }
            }
        }

        public void ActivateAllBricks(bool enabled)
        {
            if (enabled == true) return;

            ClearAllBricks();
            LogActiveBrickCounts();
            Debug.Log("All bricks have been deactivated");
        }

        private void LogActiveBrickCounts()
        {
            var brickCounts = "Active bricks: ";
            foreach (BrickColor color in Enum.GetValues(typeof(BrickColor)))
            {
                brickCounts += $"{color}: {_activeBricks[color].Count}, ";
            }
            Debug.Log(brickCounts);
        }

        public void RemoveBrick(Brick brick)
        {
            if (brick == null) return;

            if (_activeBricks.ContainsKey(brick.Color))
            {
                _activeBricks[brick.Color].Remove(brick);
            }

            if (_brickToSpawnPointIndex.ContainsKey(brick))
            {
                int spawnPointIndex = _brickToSpawnPointIndex[brick];
                this._currentSpawnPointGenerator.SetSpawnPointAvailability(spawnPointIndex, true);
                _brickToSpawnPointIndex.Remove(brick);
            }

            brick.ReturnToPool();
        }

        public void RespawnBrick(Brick brick)
        {
            if (brick == null) return;

            RemoveBrick(brick);

            var newPoints = this._currentSpawnPointGenerator.GetRandomSpawnPoints(1);
            if (newPoints.Count > 0)
            {
                var newPosition = newPoints[0];
                var newSpawnPointIndex = this._currentSpawnPointGenerator.GetSpawnPointIndex(newPosition);

                brick.transform.position = newPosition + Vector3.down * 0.15f;

                brick.transform.SetParent(this._currentFloor._brickParent);
                brick.transform.localRotation = Quaternion.identity;
                var brickVisual = brick.transform.GetChild(0);
                if (brickVisual != null)
                {
                    brickVisual.localRotation = Quaternion.identity;
                    brickVisual.localPosition = Vector3.zero;
                }
                brick.gameObject.GetComponent<Collider>().enabled = true;
                brick.gameObject.SetActive(true);

                this._activeBricks[brick.Color].Add(brick);

                _brickToSpawnPointIndex[brick] = newSpawnPointIndex;
                this._currentSpawnPointGenerator.SetSpawnPointAvailability(newSpawnPointIndex, false);
            }
            else
            {
                brick.ReturnToPool();
            }
        }
    }
}