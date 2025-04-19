namespace _GAME.Scripts.Utils
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ObjectPool<T> where T : Component
    {
        private T _prefab;
        private List<T> _pool = new List<T>();
        private Transform _parent;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject(false);
            }
        }

        private T CreateNewObject(bool active)
        {
            T obj = Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(active);
            _pool.Add(obj);
            return obj;
        }

        public T Get()
        {
            foreach (var obj in this._pool)
            {
                if (!obj.gameObject.activeInHierarchy)
                {
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }
            //if no inactive object is found, create a new one
            return CreateNewObject(true);
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        public void ReturnAll()
        {
            foreach (var obj in this._pool)
            {
                if (obj.gameObject.activeInHierarchy)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }
}