namespace _GAME.Scripts.Level
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using _GAME.Scripts.Character;
    using _GAME.Scripts.Floor;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    public class LevelManager : MonoBehaviour
    {
        private List<Transform> _startPoints;
        [SerializeField] private Transform       _finishLine;
        [SerializeField] private List<LevelData> levelDataList;

        public static LevelManager Instance { get; private set; }

        private GameObject _currentLevelInstance;
        private int        _currentLevelIndex = -1;
        public  int        CurrentLevelIndex => _currentLevelIndex;

        public event Action<int> OnLevelLoaded;
        public event Action      OnLevelStarted;
        public event Action      OnLevelUnLoaded;

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

            // Verify data
            if (levelDataList == null || levelDataList.Count == 0)
            {
                Debug.LogError("LevelManager: No level data available!");
            }
        }

        public void LoadLevel(int levelIndex)
        {
            Debug.Log($"LevelManager: Request to load level {levelIndex}");

            if (levelIndex < 0 || levelIndex >= this.levelDataList.Count)
            {
                Debug.LogError($"LevelManager: Level index {levelIndex} is out of range.");
                return;
            }

            StartCoroutine(LoadLevelRoutine(levelIndex));
        }

        public void RestartLevel()
        {
            Debug.Log($"LevelManager: Restarting level {_currentLevelIndex}");
            LoadLevel(this._currentLevelIndex);
        }

        public void LoadNextLevel()
        {
            int nextLevel = this._currentLevelIndex + 1;
            Debug.Log($"LevelManager: Loading next level {nextLevel}");

            if (nextLevel >= levelDataList.Count)
            {
                Debug.LogWarning("LevelManager: No more levels available. Restarting from first level.");
                nextLevel = 0;
            }

            LoadLevel(nextLevel);
        }

        private IEnumerator LoadLevelRoutine(int levelIndex)
        {
            Debug.Log($"LevelManager: Starting level load routine for level {levelIndex}");

            // Unload current level if exists
            if (this._currentLevelInstance != null)
            {
                Debug.Log("LevelManager: Unloading current level...");
                Destroy(this._currentLevelInstance);
                OnLevelUnLoaded?.Invoke();
                yield return null; // Wait one frame to ensure destruction completes
            }

            // Load new level
            var levelData = this.levelDataList[levelIndex];
            Debug.Log($"LevelManager: Instantiating level prefab for level {levelIndex}");

            // Instantiate at origin with identity rotation
            this._currentLevelInstance = Instantiate(levelData.levelPrefab, Vector3.zero, Quaternion.identity);
            this._currentLevelIndex    = levelIndex;

            if (this._currentLevelInstance == null)
            {
                Debug.LogError("LevelManager: Failed to instantiate level prefab!");
                yield break;
            }

            yield return new WaitForEndOfFrame();

            var floorsInLevel = _currentLevelInstance.GetComponentsInChildren<Floor>();

            if (FloorManager.Instance != null && floorsInLevel.Length > 0)
            {
                var listOfFloors = new List<Floor>(floorsInLevel);
                FloorManager.Instance.floors = listOfFloors;
                FloorManager.Instance.InitializeFloors(listOfFloors);

                Debug.Log($"Found and registered {floorsInLevel.Length} floors");
            }

            var levelPrefab = _currentLevelInstance.GetComponent<LevelPrefab>();
            var startPoints = levelPrefab._startPoints;
            _startPoints = new List<Transform>(startPoints);

            // Debug information
            Debug.Log($"LevelManager: Level {levelIndex} prefab instantiated");
            Debug.Log($"LevelManager: Level position: {_currentLevelInstance.transform.position}");
            Debug.Log($"LevelManager: Level active: {_currentLevelInstance.activeSelf}");
            Debug.Log($"LevelManager: Child count: {_currentLevelInstance.transform.childCount}");

            // Ensure it's visible and active
            _currentLevelInstance.SetActive(true);

            // Wait for everything to initialize properly
            new WaitForSeconds(0.5f);

            // Setup characters, player, and opponents
            Debug.Log("LevelManager: Setting up level components...");
            SetupLevel(levelData);

            // Notify that level is loaded and ready
            Debug.Log($"LevelManager: Level {levelIndex} setup complete, notifying listeners");
            OnLevelLoaded?.Invoke(levelIndex);

            // Start the level
            OnLevelStarted?.Invoke();
        }

        private void SetupLevel(LevelData levelData)
        {
            Debug.Log($"LevelManager: Setting up level with {levelData.numberOfOpponents} opponents");

            // Setup player first
            SetupPlayer(levelData);

            // Setup opponents
            SetupOpponents(levelData.opponentColors);

            // Make sure FloorManager is properly initialized
            if (FloorManager.Instance != null)
            {
                Debug.Log($"LevelManager: Floor manager found with {FloorManager.Instance.floors.Count} floors");
            }
            else
            {
                Debug.LogError("LevelManager: FloorManager.Instance is null!");
            }
        }

        private void SetupPlayer(LevelData levelData)
        {
            if (_startPoints == null || _startPoints.Count == 0)
            {
                Debug.LogError("LevelManager: No start points defined!");
                return;
            }

            var player = FindPlayerCharacter();
            if (player == null)
            {
                Debug.LogError("LevelManager: Player character not found!");
                return;
            }

            Debug.Log("LevelManager: Setting up player character");

            // Set player position to first start point
            var playerStartPoint = _startPoints[0];
            player.transform.position = playerStartPoint.position + Vector3.up * 0.5f;
            player.transform.rotation = playerStartPoint.rotation;

            // Set player color
            /*
            player.SetCharacterColor(levelData.playerColor);
            */
            Debug.Log($"LevelManager: Player set to position {player.transform.position} with color {levelData.playerColor}");
        }

        private Character FindPlayerCharacter()
        {
            if (FloorManager.Instance != null && FloorManager.Instance.allCharacters.Count > 0)
            {
                foreach (var character in FloorManager.Instance.allCharacters)
                {
                    if (character != null && character.CompareTag("Player"))
                    {
                        return character;
                    }
                }
            }

            // Try to find in scene if not found in FloorManager
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            return playerObject?.GetComponent<Character>();
        }

        private void SetupOpponents(BrickColor[] opponentColors)
        {
            if (_startPoints == null || _startPoints.Count <= 1)
            {
                Debug.LogError("LevelManager: Not enough start points for opponents!");
                return;
            }

            var bots = FindBotCharacters();
            if (bots == null || bots.Count == 0)
            {
                Debug.LogError("LevelManager: No bot characters found!");
                return;
            }

            Debug.Log($"LevelManager: Setting up {bots.Count} bot characters");

            // Get start points for bots (skip first one which is for player)
            var botStartPoints = new List<Transform>();
            for (int i = 1; i < _startPoints.Count; i++)
            {
                botStartPoints.Add(_startPoints[i]);
            }

            // Position and set color for each bot
            for (int i = 0; i < bots.Count; i++)
            {
                if (i >= botStartPoints.Count)
                {
                    Debug.LogError("LevelManager: Not enough start points for all opponents!");
                    break;
                }

                var bot        = bots[i];
                var startPoint = botStartPoints[i];

                // Set position and rotation
                bot.transform.position = startPoint.position + Vector3.up * 0.5f;
                bot.transform.rotation = startPoint.rotation;

                /*// Set color if available
                if (i < opponentColors.Length)
                {
                    bot.SetCharacterColor(opponentColors[i]);
                    Debug.Log($"LevelManager: Bot {i} set to position {bot.transform.position} with color {opponentColors[i]}");
                }*/
            }
        }

        private static List<BotController> FindBotCharacters()
        {
            if (FloorManager.Instance != null)
            {
                if (FloorManager.Instance.allBots != null && FloorManager.Instance.allBots.Count > 0)
                {
                    return FloorManager.Instance.allBots;
                }

                // Try to get bots from all characters
                if (FloorManager.Instance.allCharacters.Count > 0)
                {
                    var botList = new List<BotController>();
                    foreach (var character in FloorManager.Instance.allCharacters)
                    {
                        if (character != null && character is BotController botController)
                        {
                            botList.Add(botController);
                        }
                    }
                    return botList;
                }
            }

            // Try to find in scene if not found in FloorManager
            var botObjects = GameObject.FindGameObjectsWithTag("Bot");
            var bots       = new List<BotController>();

            foreach (var bot in botObjects)
            {
                var botController = bot.GetComponent<BotController>();
                if (botController != null)
                {
                    bots.Add(botController);
                }
            }

            return bots;
        }
    }
}