namespace _GAME.Scripts.FSM.Bridge
{
    using System;
    using _GAME.Scripts.FSM.Brick;
    using _GAME.Scripts.Character;
    using UnityEngine;

    public class BridgeStep : MonoBehaviour
    {
        public BrickColor CurrentColor  { get; private set; } = BrickColor.Grey;
        public BrickColor PreviousColor { get; set; }
        /*
        public bool       IsBuilt       { get; private set; } = false;
        */

        [SerializeField] private Material defaultMaterial;

        private void Awake()
        {
            if (this.defaultMaterial != null)
            {
                this.GetComponentInChildren<Renderer>().material = this.defaultMaterial;
            }
        }
        public void Reset()
        {
            CurrentColor  = BrickColor.Grey;
            PreviousColor = BrickColor.Grey;

            if (this.defaultMaterial != null)
            {
                this.GetComponentInChildren<Renderer>().material = this.defaultMaterial;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player      = other.GetComponent<PlayerController>();
                var topBrick    = player.GetTopBrick();
                var playerColor = player.characterColor;

                if (player.BrickCount <= 0 || CurrentColor == playerColor)
                {
                    return;
                }

                PreviousColor = CurrentColor;
                CurrentColor  = playerColor;

                var material = MaterialManager.Instance.GetMaterial(playerColor);

                this.GetComponentInChildren<Renderer>().material = material;

                other.GetComponent<PlayerController>().PlaceBrick(topBrick);

            }
        }
    }
}