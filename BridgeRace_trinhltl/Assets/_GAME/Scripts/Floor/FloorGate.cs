namespace _GAME.Scripts.Floor
{
    using System;
    using _GAME.Scripts.Character;
    using _GAME.Scripts.FSM;
    using UnityEngine;

    public class FloorGate : MonoBehaviour
    {
        [SerializeField] private Floor currentFloor;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Bot"))
            {
                var character = other.GetComponent<Character>();
                if (character != null)
                {
                    var currentFloorIndex = FloorManager.Instance.GetFloorIndex(this.currentFloor);
                    var nextFloorIndex    = currentFloorIndex + 1;
                    if (nextFloorIndex < FloorManager.Instance.floors.Count)
                    {
                        FloorManager.Instance.RegisterCharacterToFloor(
                            character,
                            FloorManager.Instance.GetFloorAtIndex(nextFloorIndex)
                            );
                    }
                    else
                    {
                        Debug.Log("No more floors available.");
                    }

                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            this.gameObject.GetComponent<Collider>().isTrigger = false;
        }
    }
}