namespace _GAME.Scripts.Floor
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    public class Floor : MonoBehaviour
    {
        [SerializeField] private List<Character> charactersOnFloor = new List<Character>();
        private bool _isActive = false;

        public void Activate(bool activate)
        {
            this._isActive = activate;
            BrickSpawner.Instance.ActivateAllBricks(activate);
        }

        private void SpawnBricksOnFloor()
        {

        }

        public void RegisterCharacter(Character character)
        {
            if (!charactersOnFloor.Contains(character))
            {
                charactersOnFloor.Add(character);
            }
        }

        public void UnregisterCharacter(Character character)
        {
            if (charactersOnFloor.Contains(character))
            {
                charactersOnFloor.Remove(character);
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