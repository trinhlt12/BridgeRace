namespace _GAME.Scripts.Utils
{
    using System;
    using _GAME.Scripts.Floor;
    using TMPro;
    using UnityEngine;

    public class CurrentFloorDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentFloorText;

        private void Update()
        {
            var currentFloor = FloorManager.Instance.GetCurrentFloor();
            if (this.currentFloorText != null)
            {
                this.currentFloorText.text = $"Current Floor: {currentFloor}";
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component is not assigned.");
            }
        }
    }
}