namespace _GAME.Scripts.Level
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using _GAME.Scripts.Character;
    using UnityEngine;

    public enum GameState
    {
        Initializing,
        Playing,
        Win,
        Lose,
        Pause
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public List<Character> winners = new List<Character>();
        [SerializeField] private int maxWinners = 2;
        public GameState CurrentState { get; private set; } = GameState.Initializing;

        [SerializeField] private int startingLevel = 0;

        public event Action<GameState> OnGameStateChanged;

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
        }

        private void Start()
        {
            // Initialize the game after all components have started
            InitializeGame();
        }

        private void InitializeGame()
        {
            Debug.Log("GameManager: Initializing game...");

            // Load the saved level or start from the first level
            int levelToLoad = startingLevel;

            int lastLevel = PlayerPrefs.GetInt("LastCompletedLevel", -1);
            if (lastLevel >= 0)
            {
                levelToLoad = lastLevel + 1;
                Debug.Log($"GameManager: Loading level {levelToLoad} based on player progress");
            }

            // Make sure LevelManager is ready
            if (LevelManager.Instance == null)
            {
                Debug.LogError("GameManager: LevelManager instance is null!");
                return;
            }

            // Subscribe to level events
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;

            // Load the level
            Debug.Log($"GameManager: Requesting to load level {levelToLoad}");
            LevelManager.Instance.LoadLevel(levelToLoad);
        }

        private void OnLevelLoaded(int levelIndex)
        {
            Debug.Log($"GameManager: Level {levelIndex} loaded successfully, starting gameplay");
            ChangeState(GameState.Playing);
        }

        private void ChangeState(GameState newState)
        {
            if ((CurrentState == GameState.Lose || CurrentState == GameState.Win) &&
                (newState != GameState.Initializing))
            {
                return;
            }

            Debug.Log($"GameManager: State changing from {CurrentState} to {newState}");
            CurrentState = newState;

            switch (newState)
            {
                case GameState.Initializing:
                    // Reset state for a new level
                    winners.Clear();
                    break;
                case GameState.Playing:
                    StartGamePlay();
                    break;
                case GameState.Win:
                    HandleWin();
                    break;
                case GameState.Lose:
                    HandleLose();
                    break;
                case GameState.Pause:
                    PauseGame();
                    break;
            }

            OnGameStateChanged?.Invoke(newState);
        }

        private void HandleLose()
        {
            Debug.Log("GameManager: Player lost the game");
            //TODO : Handle Lose UI
        }

        private void HandleWin()
        {
            Debug.Log("GameManager: Player won the game");
            // Save progress
            PlayerPrefs.SetInt("LastCompletedLevel", LevelManager.Instance.CurrentLevelIndex);
            PlayerPrefs.Save();

            //TODO : Show win UI and option to go to next level
        }

        private void StartGamePlay()
        {
            Debug.Log("GameManager: Starting gameplay");
            // Enable player controls, etc.
        }

        private void PauseGame()
        {
            Debug.Log("GameManager: Game paused");
            // Show pause UI, etc.
        }

        public void CharacterReachedFinish(Character character)
        {
            if (CurrentState != GameState.Playing) return;

            if (this.winners.Contains(character)) return;

            if (this.winners.Count >= this.maxWinners)
            {
                return;
            }

            this.winners.Add(character);

            var rank = this.winners.Count;
            Debug.Log($"GameManager: Character {character.name} reached the finish line and is ranked {rank}");

            if (character.CompareTag("Player"))
            {
                ChangeState(GameState.Win);
                Debug.Log($"GameManager: Player {character.name} won the game!");

            }
            else if (this.winners.Count >= this.maxWinners && !this.winners.Any(w => w.CompareTag("Player")))
            {
                ChangeState(GameState.Lose);
            }
        }

        public void RestartLevel()
        {
            ChangeState(GameState.Initializing);
            LevelManager.Instance.RestartLevel();
        }

        public void LoadNextLevel()
        {
            ChangeState(GameState.Initializing);
            LevelManager.Instance.LoadNextLevel();
        }

        private void OnDestroy()
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.OnLevelLoaded -= OnLevelLoaded;
            }
        }
    }
}