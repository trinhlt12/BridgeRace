namespace _GAME.Scripts.FSM.Brick
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BrickSpawner : MonoBehaviour
    {
        [SerializeField] private SpawnPointGenerator _spawnPointGenerator;
        [SerializeField] private int                 maxBricksPerColor = 25;
        [SerializeField] private float spawnInterval = 2f;

        private BrickPoolManager _brickPoolManager;
        private Dictionary<BrickColor, List<Brick>> _activeBricks = new Dictionary<BrickColor, List<Brick>>();

        private void Start()
        {
            _brickPoolManager = BrickPoolManager.Instance;

            foreach (var color in Enum.GetValues(typeof(BrickColor)))
            {
                _activeBricks[(BrickColor)color] = new List<Brick>();
            }

            StartCoroutine(SpawnBricksRoutine());
        }

        private IEnumerator SpawnBricksRoutine()
        {
            while (true)
            {
                foreach (BrickColor color in Enum.GetValues(typeof(BrickColor)))
                {
                    if (this._activeBricks[color].Count < this.maxBricksPerColor)
                    {
                        var bricksToSpawn = Mathf.Min(
                            5,
                            this.maxBricksPerColor - this._activeBricks[color].Count);
                        this.SpawnBricksOfColor(color, bricksToSpawn);
                    }
                }
                yield return new WaitForSeconds(this.spawnInterval);
            }
        }

        private void SpawnBricksOfColor(BrickColor color, int count)
        {
            var      spawnPoints   = _spawnPointGenerator.GetRandomSpawnPoints(count);

            Material brickMaterial = null;

            if(this._brickPoolManager != null && MaterialManager.Instance != null)
            {
                brickMaterial = MaterialManager.Instance.GetMaterial(color);
            }


            foreach (var spawnPoint in spawnPoints)
            {
                var brick = _brickPoolManager.SpawnBrick(color, spawnPoint);
                if (brick != null)
                {
                    brick.Initialize(color, brickMaterial);
                    _activeBricks[color].Add(brick);
                }
            }
        }

        private void RemoveBrick(Brick brick)
        {
            if (_activeBricks.ContainsKey(brick.Color))
            {
                _activeBricks[brick.Color].Remove(brick);
            }
            brick.ReturnToPool();
        }
    }
}