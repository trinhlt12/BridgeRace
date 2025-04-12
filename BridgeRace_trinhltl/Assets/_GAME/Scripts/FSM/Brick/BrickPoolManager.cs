namespace _GAME.Scripts.FSM.Brick
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.Utils;
    using UnityEngine;

    public class BrickPoolManager : MonoBehaviour
    {
        public static BrickPoolManager Instance { get; private set; }

        [SerializeField] private Brick _brickPrefab;
        [SerializeField] private int _poolSize = 10;
        [SerializeField] private Transform _poolParent;

        private Dictionary<BrickColor, ObjectPool<Brick>> _brickPools = new Dictionary<BrickColor, ObjectPool<Brick>>();
        private MaterialManager _materialManager;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _materialManager = MaterialManager.Instance;
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var color in Enum.GetValues(typeof(BrickColor))){
                this._brickPools[(BrickColor)color] = new ObjectPool<Brick>(_brickPrefab, _poolSize, _poolParent);
            }
        }

        public Brick SpawnBrick(BrickColor color, Vector3 position)
        {
            if(!_brickPools.ContainsKey(color)) return null;

            var brick = _brickPools[color].Get();
            brick.transform.position = position;

            var material = _materialManager.GetMaterial(color);
            brick.Initialize(color, material);
            return brick;
        }

        public void ReturnBrick(Brick brick, BrickColor color)
        {
            if (_brickPools.ContainsKey(brick.Color))
            {
                _brickPools[brick.Color].Return(brick);
            }
        }
    }
}