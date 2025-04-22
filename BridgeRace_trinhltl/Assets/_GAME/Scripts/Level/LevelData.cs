namespace _GAME.Scripts.Level
{
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LevelData", menuName = "BridgeRace/LevelData", order = 0)]
    public class LevelData : ScriptableObject
    {
        public int levelIndex;
        public string levelName;
        public GameObject levelPrefab;
        public int numberOfOpponents;
        public BrickColor[] opponentColors;
    }
}