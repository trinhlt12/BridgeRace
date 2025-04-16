namespace _GAME.Scripts.Floor
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.Character;
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    public class Floor : MonoBehaviour
    {
        [SerializeField] private SpawnPointGenerator _spawnPointGenerator;
        [SerializeField] private List<Character>     charactersOnFloor = new List<Character>();
        public List<FloorGate>           floorGate;
        private                  bool                _isActive = false;
        public                   Transform           _brickParent;

        public void Activate(bool activate)
        {
            this._isActive = activate;

            if (activate)
            {
                SpawnBricksForCharactersOnFloor();
            }

            BrickSpawner.Instance.ActivateAllBricks(activate);
        }

        public SpawnPointGenerator GetSpawnPointGenerator()
        {
            return this._spawnPointGenerator;
        }

        private void SpawnBricksForCharactersOnFloor()
        {
            if (BrickSpawner.Instance != null && this._isActive)
            {
                BrickSpawner.Instance.SetCurrentFloor(this);
                BrickSpawner.Instance.SpawnBricksForCharacters(this.charactersOnFloor);
            }
        }

        public void RegisterCharacter(Character character)
        {
            if (!charactersOnFloor.Contains(character))
            {
                charactersOnFloor.Add(character);
                if (this._isActive)
                {
                    this.SpawnBricksForCharactersOnFloor();
                }
            }
        }

        public void UnregisterCharacter(Character character)
        {
            if (charactersOnFloor.Contains(character))
            {
                charactersOnFloor.Remove(character);

                if (this._isActive)
                {
                    this.SpawnBricksForCharactersOnFloor();
                }
            }

            if (charactersOnFloor.Count == 0 && !FloorManager.Instance.IsCurrentFloor(this))
            {
                Activate(false);
            }
        }

        public int GetCharacterCount()
        {
            return charactersOnFloor.Count;
        }

        public bool IsActive()
        {
            return this._isActive;
        }
    }
}