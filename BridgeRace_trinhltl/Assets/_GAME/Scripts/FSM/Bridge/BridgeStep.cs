namespace _GAME.Scripts.FSM.Bridge
{
    using System;
    using _GAME.Scripts.FSM.Brick;
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
    }
}