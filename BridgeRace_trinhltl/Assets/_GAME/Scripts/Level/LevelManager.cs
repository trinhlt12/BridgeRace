namespace _GAME.Scripts.Level
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using _GAME.Scripts.Floor;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<Transform> _startPoints;
        [SerializeField] private Transform       _finishLine;
        [SerializeField] private List<LevelData> levelDataList;

        public static LevelManager Instance { get; private set; }

        private GameObject _currentLevelInstance;
        private int        _currentLevelIndex = -1;

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
        }

        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= this.levelDataList.Count)
            {
                Debug.LogError($"Level index {levelIndex} is out of range.");
                return;
            }

            StartCoroutine(LoadLevelRoutine(levelIndex));
        }

        public void SaveProgress(){
            PlayerPrefs.SetInt("LastCompletedLevel", this._currentLevelIndex);
            PlayerPrefs.Save();
        }

        public void RestartLevel()
        {
            LoadLevel(this._currentLevelIndex);
        }

        public void LoadNextLevel()
        {
            LoadLevel(this._currentLevelIndex + 1);
        }

        private IEnumerator LoadLevelRoutine(int levelIndex)
        {
            //unload current lv if exists
            if (this._currentLevelInstance != null)
            {
                Destroy(this._currentLevelInstance);
                OnLevelUnLoaded?.Invoke();
                yield return null;
            }

            //load new level
            var levelData = this.levelDataList[levelIndex];
            this._currentLevelInstance = Instantiate(levelData.levelPrefab);
            Debug.Log($"Level {levelIndex} loaded.");
            this._currentLevelIndex    = levelIndex;

            yield return new WaitForSeconds(0.5f);

            OnLevelLoaded?.Invoke(levelIndex);

            OnLevelStarted?.Invoke();
        }

        private void SetupLevel(LevelData levelData)
        {
            this.SetupOpponents(levelData.opponentColors);

            SetupPlayer();
        }

        private void SetupPlayer()
        {
            var player = FloorManager.Instance.allCharacters.Find(c => c.CompareTag("Player"));
            if (player == null)
            {
                Debug.LogError("Player character not found.");
                return;
            }

            var playerStartPoint = _startPoints[0];
            player.transform.position = playerStartPoint.position + Vector3.up * 0.5f;
            player.transform.rotation = playerStartPoint.rotation;

            var currentLevelData = this.levelDataList[this._currentLevelIndex];
            player.SetCharacterColor(currentLevelData.playerColor);
        }

        private void SetupOpponents(BrickColor[] levelDataOpponentColors)
        {
            var bots = FloorManager.Instance.allBots;
            if (bots == null || bots.Count == 0)
            {
                Debug.LogError("No opponent characters found.");
                return;
            }

            var botStartPoints = _startPoints.GetRange(1, _startPoints.Count - 1);
            for (int i = 0; i < bots.Count; i++)
            {
                if (i >= botStartPoints.Count)
                {
                    Debug.LogError("Not enough start points for all opponents.");
                    break;
                }

                var bot = bots[i];
                var startPoint = botStartPoints[i];

                bot.transform.position = startPoint.position + Vector3.up * 0.5f;
                bot.transform.rotation = startPoint.rotation;

                if (i < levelDataOpponentColors.Length)
                {
                    bot.SetCharacterColor(levelDataOpponentColors[i]);
                }
            }
        }
    }
}