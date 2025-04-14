namespace _GAME.Scripts.Floor
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.FSM;
    using UnityEngine;

    public class FloorManager : MonoBehaviour
    {
        public static FloorManager Instance { get; private set; }

        [SerializeField] public List<Floor> floors = new List<Floor>();
        [SerializeField] private List<Character> allCharacters = new List<Character>();

        private Floor currentFloor;
        private Dictionary<Character, Floor> characterFloorMap = new Dictionary<Character, Floor>();

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
            InitializeFloors();
        }

        private void InitializeFloors()
        {
            for (var i = 0; i < this.floors.Count; i++)
            {
                Debug.Log(this.floors[i].name);
            }

            if (this.floors.Count > 0)
            {
                this.currentFloor = this.floors[0];
                foreach (var character in this.allCharacters)
                {
                    RegisterCharacterToFloor(character, this.floors[0]);
                }
            }
        }

        public void RegisterCharacterToFloor(Character character, Floor floor)
        {
            if (this.characterFloorMap.ContainsKey(character))
            {
                var previousFloor = this.characterFloorMap[character];
                if (previousFloor != null)
                {
                    previousFloor.UnregisterCharacter(character);
                }

                floor.RegisterCharacter(character);
                this.characterFloorMap[character] = floor;

                if(!floor.IsActive())
                {
                    floor.Activate(true);
                }

                if (character.CompareTag("Player"))
                {
                    currentFloor = floor;
                }
            }
        }

        public int GetFloorIndex(Floor floor)
        {
            return this.floors.IndexOf(floor);
        }

        public Floor GetFloorAtIndex(int index)
        {
            if (index >= 0 && index < this.floors.Count)
            {
                return this.floors[index];
            }
            return null;
        }

        public bool IsCurrentFloor(Floor floor)
        {
            return floor == this.currentFloor;
        }
    }
}