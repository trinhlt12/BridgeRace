namespace _GAME.Scripts.FSM.Brick
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MaterialManager : MonoBehaviour
    {
        public static MaterialManager Instance { get; private set; }

        [System.Serializable]
        public class BrickColorData
        {
            public BrickColor brickColor;
            public Material material;
        }

        [SerializeField] private List<BrickColorData> materialList = new List<BrickColorData>();

        private readonly Dictionary<BrickColor, Material> _materialsDict = new Dictionary<BrickColor, Material>();
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

            foreach (var brick in this.materialList)
            {
                this._materialsDict[brick.brickColor] = brick.material;
            }
        }

        public Material GetMaterial(BrickColor brickColor)
        {
            return this.materialList[(int)brickColor].material;

        }

    }
}