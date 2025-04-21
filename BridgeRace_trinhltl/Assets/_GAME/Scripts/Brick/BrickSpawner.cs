using System;
using System.Collections.Generic;
using UnityEngine;
using _GAME.Scripts.Floor;

namespace _GAME.Scripts.FSM.Brick
{
    using _GAME.Scripts.Character;
    using Floor = _GAME.Scripts.Floor.Floor;

    public class BrickSpawner : MonoBehaviour
    {
        [SerializeField] private int maxBricksPerColor = 25;
        [SerializeField] private int minBricksPerColor = 5;
        public static BrickSpawner Instance { get; private set; }

        private BrickPoolManager _brickPoolManager;
        private Floor _currentFloor;

        public Dictionary<BrickColor, List<Brick>> _activeBricks = new Dictionary<BrickColor, List<Brick>>();

        public delegate void BricksSpawnedDelegate(BrickColor color, int count);
        public event BricksSpawnedDelegate OnBricksSpawned;

        public Dictionary<Brick, int> _brickToSpawnPointIndex = new Dictionary<Brick, int>();

        public SpawnPointGenerator _currentSpawnPointGenerator;

        private Dictionary<Floor, Dictionary<BrickColor, List<Brick>>> _floorBricks =
            new Dictionary<Floor, Dictionary<BrickColor, List<Brick>>>();

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

            Debug.Log($"Initialized _activeBricks with {_activeBricks.Count} colors");

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
            _currentFloor               = floor;
            _currentSpawnPointGenerator = floor.GetSpawnPointGenerator();

            if (!_floorBricks.ContainsKey(floor))
            {
                _floorBricks[floor] = new Dictionary<BrickColor, List<Brick>>();
                foreach (BrickColor color in Enum.GetValues(typeof(BrickColor)))
                {
                    _floorBricks[floor][color] = new List<Brick>();
                }
            }

            if (_currentSpawnPointGenerator == null)
            {
                Debug.LogError("No spawnpoint generator found");
            }
        }

        public void SpawnBricksForCharacters(List<Character> characters)
        {
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
        }

        public void SpawnBricksForCharacter(Character character)
        {
            if (character.characterColor == BrickColor.Grey)
                return;

            var singleCharList = new List<Character> { character };
            SpawnBricksForCharacters(singleCharList);
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

                    if (!_floorBricks.ContainsKey(_currentFloor))
                    {
                        _floorBricks[_currentFloor] = new Dictionary<BrickColor, List<Brick>>();
                    }

                    if (!_floorBricks[_currentFloor].ContainsKey(color))
                    {
                        _floorBricks[_currentFloor][color] = new List<Brick>();
                    }

                    _floorBricks[_currentFloor][color].Add(brick);

                    this._brickToSpawnPointIndex[brick] = spawnPointIndex;
                    this._currentSpawnPointGenerator.SetSpawnPointAvailability(spawnPointIndex, false);
                    OnBricksSpawned?.Invoke(color, _activeBricks[color].Count);
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
        }

        public void RemoveBrick(Brick brick)
        {
            if (brick == null) return;

            if (_activeBricks.ContainsKey(brick.Color))
            {
                _activeBricks[brick.Color].Remove(brick);
            }

            foreach (var floorEntry in _floorBricks)
            {
                if (floorEntry.Value.TryGetValue(brick.Color, out var bricks))
                {
                    bricks.Remove(brick);
                }
            }

            if (_brickToSpawnPointIndex.ContainsKey(brick))
            {
                int spawnPointIndex = _brickToSpawnPointIndex[brick];
                if (_currentSpawnPointGenerator != null)
                {
                    _currentSpawnPointGenerator.SetSpawnPointAvailability(spawnPointIndex, true);
                }
                _brickToSpawnPointIndex.Remove(brick);
            }

            brick.ReturnToPool();
        }

        public void RemoveBricksByColor(Floor floor, BrickColor color)
        {
            if (floor == null) return;

            if (_floorBricks.TryGetValue(floor, out var colorBricks) &&
                colorBricks.TryGetValue(color, out var bricks))
            {
                var bricksCopy = new List<Brick>(bricks);

                foreach (var brick in bricksCopy)
                {
                    if (brick != null)
                    {
                        _activeBricks[color].Remove(brick);

                        if (_brickToSpawnPointIndex.ContainsKey(brick))
                        {
                            int spawnPointIndex = _brickToSpawnPointIndex[brick];
                            if (floor.GetSpawnPointGenerator() != null)
                            {
                                floor.GetSpawnPointGenerator().SetSpawnPointAvailability(spawnPointIndex, true);
                            }
                            _brickToSpawnPointIndex.Remove(brick);
                        }

                        brick.ReturnToPool();
                    }
                }

                bricks.Clear();
            }
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

        public List<Brick> GetActiveBricksByColor(BrickColor color)
        {
            if (this._activeBricks.TryGetValue(color, out var bricks))
            {
                return bricks;
            }
            else
            {
                Debug.LogWarning($"No active bricks found for color: {color}");
                return new List<Brick>();
            }
        }
    }
}