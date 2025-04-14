namespace _GAME.Scripts.Floor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class FloorManager : MonoBehaviour
    {
        public static FloorManager Instance { get; private set; }

        [SerializeField] private List<Floor> floors = new List<Floor>();

        private int currentFloorIndex = 0;

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

        public void ActivateFloor(int floorIndex)
        {
            //TODO: Deactivate previous floor + activate current floor
        }
    }
}