namespace _GAME.Scripts.Level
{
    using System.Collections.Generic;
    using UnityEngine;

    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<Transform> _startPoints;
        [SerializeField] private Transform       _finishLine;

    }
}