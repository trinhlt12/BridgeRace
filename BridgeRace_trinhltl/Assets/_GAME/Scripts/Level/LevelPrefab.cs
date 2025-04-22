namespace _GAME.Scripts.Level
{
    using System.Linq;
    using _GAME.Scripts.Floor;
    using UnityEngine;

    public class LevelPrefab : MonoBehaviour, ILevelData
    {
        [SerializeField] private Floor[] _levelFloors;

        public Floor[] GetFloors()
        {
            return _levelFloors.ToArray();
        }
    }
}