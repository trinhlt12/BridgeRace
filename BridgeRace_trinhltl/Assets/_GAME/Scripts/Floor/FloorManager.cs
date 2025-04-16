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

        [SerializeField] public  List<Floor>             floors        = new List<Floor>();
        public List<Character> allCharacters = new List<Character>();

        private Floor                        currentFloor;
        private Dictionary<Character, Floor> characterFloorMap = new Dictionary<Character, Floor>();
        public  List<BotController>          allBots;

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
            Debug.Log(this.currentFloor.gameObject.name);
            Debug.Log("Numbers of chars on current floor:" + this.currentFloor.GetCharacterCount());
            Debug.Log("Numbers of chars in total" + this.allCharacters.Count);
            Debug.Log("Total floors: " + this.floors.Count);
            //log number of characters in current floor:
            Debug.Log($"Number of characters in current floor: {this.currentFloor.GetCharacterCount()}");
        }

        private void InitializeFloors()
        {
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
    }
}