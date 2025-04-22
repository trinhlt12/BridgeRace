namespace _GAME.Scripts.Level
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using _GAME.Scripts.Character;
    using UnityEngine;

    public enum GameState
    {
        Playing,
        Win,
        Lose,
        Pause
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public                   List<Character> winners    = new List<Character>();
        [SerializeField] private int             maxWinners = 2;
        public                   GameState       CurrentState { get; private set; } = GameState.Playing;

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
            ChangeState(GameState.Playing);
        }

        private void ChangeState(GameState newState)
        {
            if ((CurrentState == GameState.Lose || CurrentState == GameState.Win))
            {
                return;
            }

            CurrentState = newState;

            switch (newState)
            {
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
        }

        private void HandleWin()
        {
        }

        private void StartGamePlay()
        {
        }

        private void PauseGame()
        {

        }

        public void CharacterReachedFinish(Character character)
        {
            if(CurrentState != GameState.Playing) return;

            if(this.winners.Contains(character)) return;

            if (this.winners.Count >= this.maxWinners)
            {
                return;
            }

            this.winners.Add(character);

            var rank = this.winners.Count;
            Debug.Log($"Character {character.name} reached the finish line and is ranked {rank}");

            if (character.CompareTag("Player"))
            {
                ChangeState(GameState.Win);
                Debug.Log($"Player {character.name} won the game!");

            }else if(this.winners.Count >= this.maxWinners && !this.winners.Any(w => w.CompareTag("Player")))
            {
                ChangeState(GameState.Lose);
            }
        }
    }
}