namespace _GAME.Scripts.FSM.Brick
{
    using System;
    using _GAME.Scripts.Character;
    using UnityEngine;

    public enum BrickColor
    {
        Pink = 0,
        Blue = 1,
        Orange = 2,
        Green = 3,
        Grey = 4,
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
                var characterColor = other.GetComponent<Character>().characterColor;
                if (this.Color == characterColor)
                {
                    other.GetComponent<PlayerController>().PickUpBrick(this);
                }
            }
        }
    }
}