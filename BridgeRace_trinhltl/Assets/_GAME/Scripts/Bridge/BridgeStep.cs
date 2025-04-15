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

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                var player      = other.gameObject.GetComponent<PlayerController>();
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

                other.gameObject.GetComponent<PlayerController>().PlaceBrick(topBrick);

            }else if (other.gameObject.CompareTag("Bot"))
            {
                var bot      = other.gameObject.GetComponent<BotController>();
                var topBrick = bot.GetTopBrick();
                var botColor = bot.characterColor;

                if (bot.BrickCount <= 0 || CurrentColor == botColor)
                {
                    return;
                }

                PreviousColor = CurrentColor;
                CurrentColor  = botColor;
                var material = MaterialManager.Instance.GetMaterial(botColor);
                this.GetComponentInChildren<Renderer>().material = material;
                other.gameObject.GetComponent<BotController>().PlaceBrick(topBrick);
            }
        }

        public bool IsColorMatch(BrickColor color)
        {
            return CurrentColor == color;
        }
    }
}