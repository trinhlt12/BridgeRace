namespace _GAME.Scripts.Utils
{
    using System;
    using _GAME.Scripts.Character;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class DisplayBrickCount : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI  brickCounttext;
        [SerializeField] private PlayerController player;

        private void Update()
        {
            var brickCount = player.BrickCount;
            if (this.brickCounttext != null)
            {
                this.brickCounttext.text = $"Picked Bricks: {brickCount}";
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component is not assigned.");
            }
        }
    }
}