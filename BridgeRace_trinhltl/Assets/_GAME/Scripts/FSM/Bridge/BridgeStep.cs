namespace _GAME.Scripts.FSM.Bridge
{
    using System;
    using _GAME.Scripts.FSM.Brick;
    using _GAME.Scripts.Player;
    using UnityEngine;

    public class BridgeStep : MonoBehaviour
    {
        public BrickColor CurrentColor { get; private set; } = BrickColor.Grey;
        public bool IsBuilt { get; private set; } = false;

        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Renderer renderer;

        private void Awake()
        {
            if(this.defaultMaterial != null)
            {
                this.renderer.material = this.defaultMaterial;
            }
        }

        public void BuildStep(BrickColor color)
        {

        }

        public void Reset()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();

                if(player.brickCount <= 0) return;

                var playerColor = player.characterColor;
                var material = MaterialManager.Instance.GetMaterial(playerColor);
                if (player.brickCount > 0)
                {
                    this.renderer.material = material;
                }
                other.GetComponent<PlayerController>().placeBrick();
            }
        }
    }
}