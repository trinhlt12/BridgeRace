namespace _GAME.Scripts.Floor
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.Character;
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    public class FloorManager : MonoBehaviour
    {
        public static FloorManager Instance { get; private set; }

        public  List<Floor>             floors        = new List<Floor>();
        public List<Character> allCharacters = new List<Character>();

        private Floor                        currentFloor;
        private Dictionary<Character, Floor> characterFloorMap = new Dictionary<Character, Floor>();
        public  List<BotController>          allBots;
        public int totalFloors => this.floors.Count;
        public int highestFloorIndex => this.floors.Count - 1;

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
            var floors = FindObjectsOfType<Floor>();
            foreach (var floor in floors)
            {
                this.floors.Add(floor);
            }
        }
        public void InitializeFloors(List<Floor> levelFloors)
        {
            if (levelFloors.Count > 0)
            {
                this.currentFloor = levelFloors[0];
                foreach (var character in this.allCharacters)
                {
                    RegisterCharacterToFloor(character, levelFloors[0]);
                }
            }
        }

        public void RegisterCharacterToFloor(Character character, Floor floor)
        {
            if (this.characterFloorMap.TryGetValue(character, out var previousFloor))
            {
                if (previousFloor != null)
                {
                    previousFloor.UnregisterCharacter(character);
                }
            }

            floor.RegisterCharacter(character);
            this.characterFloorMap[character] = floor;

            if(!floor.IsActive())
            {
                floor.Activate(true);
            }

            BrickSpawner.Instance.SetCurrentFloor(floor);

            if (character.CompareTag("Player"))
            {
                currentFloor = floor;
            }

        }

        public Floor GetCurrentFloorForCharacter(Character character)
        {
            return this.characterFloorMap.GetValueOrDefault(character);
        }

        public int GetCurrentFloorIndexForCharacter(Character character)
        {
            if (characterFloorMap.TryGetValue(character, out var floor))
            {
                return this.floors.IndexOf(floor);
            }
            return -1;
        }

        public int GetCurrentFloor()
        {
            return this.floors.IndexOf(this.currentFloor) + 1;
        }

        public Floor GetCurrentFloorObject()
        {
            return this.currentFloor;
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

        public List<BotController> GetAllBots()
        {
            var bots = new List<BotController>();
            foreach (var character in allCharacters)
            {
                if (character is BotController bot)
                {
                    bots.Add(bot);
                }
            }
            return bots;
        }

        public int GetFloorGateIndex(FloorGate gate)
        {
            foreach (var floor in floors)
            {
                if (floor.floorGate.Contains(gate))
                {
                    return floor.floorGate.IndexOf(gate);
                }
            }
            return -1;
        }

        public void RegisterFloors(Floor[] newFloors)
        {
            floors.Clear();

            foreach (Floor floor in newFloors)
            {
                if (floor != null && !floors.Contains(floor))
                {
                    floors.Add(floor);
                    Debug.Log($"FloorManager: Registered floor {floor.name}");
                }
            }

            if (floors.Count > 0)
            {
                currentFloor = floors[0];
                currentFloor.Activate(true);
                Debug.Log($"FloorManager: Set current floor to {currentFloor.name}");
            }
        }
    }
}