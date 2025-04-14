namespace _GAME.Scripts.Floor
{
    using System.Collections.Generic;
    using _GAME.Scripts.FSM;
    using _GAME.Scripts.FSM.Brick;
    using UnityEngine;

    public class Floor : MonoBehaviour
    {
        [SerializeField] private List<Character> charactersOnFloor = new List<Character>();

        public void Activate(bool activate)
        {
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

            if (charactersOnFloor.Count == 0)
            {
                Activate(false);
            }
        }
    }
}