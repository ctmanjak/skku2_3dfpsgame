using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Core
{
    public class PoolManager: MonoBehaviour
    {
        private static PoolManager _instance;

        [SerializeField] private GameObject[] _objects;
        
        private static Dictionary<GameObject, ObjectPool<GameObject>> _pools = new();
        private static Dictionary<GameObject, GameObject> _prefabs = new();

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
        }
        private void Start()
        {
            foreach (var obj in _objects)
            {
                _pools[obj] = new ObjectPool<GameObject>(
                    () =>
                    {
                        GameObject instantiated = Instantiate(obj);
                        instantiated.SetActive(false);
                        _prefabs[instantiated] = obj;
                        return instantiated;
                    },
                    (o) =>
                    {
                        o.SetActive(true);
                    },
                    (o) =>
                    {
                        o.SetActive(false);
                    },
                    (o) =>
                    {
                        _prefabs.Remove(o);
                        Destroy(o);
                    }
                );
            }
        }

        public static GameObject Get(GameObject prefab)
        {
            GameObject obj = _pools[prefab].Get();
            _prefabs[obj] = prefab;
            return obj;
        }

        public static GameObject Get(GameObject prefab, Transform parent)
        {
            GameObject obj = Get(prefab);
            obj.transform.SetParent(parent);
            return obj;
        }

        public static void Release(GameObject obj)
        {
            GameObject prefab = _prefabs[obj];
            ObjectPool<GameObject> pool = _pools[prefab];
            pool.Release(obj);
            _prefabs.Remove(obj);
        }
    }
}