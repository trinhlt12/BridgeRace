namespace _GAME.Scripts.FSM.Brick
{
    using System;
    using _GAME.Scripts.Player;
    using UnityEngine;

    public enum BrickColor
    {
        Pink = 0,
        Blue = 1,
        Orange = 2,
        Green = 3,
    }

    public class Brick : MonoBehaviour
    {
        public BrickColor Color { get; private set; }
        private Renderer _renderer;

        private void Awake()
        {
            this._renderer = this.GetComponentInChildren<Renderer>();
        }

        public void Initialize(BrickColor color, Material material)
        {
            Color = color;
            if (this._renderer != null && material != null)
            {
                _renderer.material = material;
            }
        }

        public void ReturnToPool()
        {
            BrickPoolManager.Instance.ReturnBrick(this, Color);
        }



        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                if (this.Color == BrickColor.Pink)
                {
                    other.GetComponent<PlayerController>().pickUpBrick(this.Color);
                }
            }
        }
    }
}