using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueLike2D
{
    
    public class PoolManager : Singleton<PoolManager>
    {
        [SerializeField] private GameObject[] prefab = default;

        private List<GameObject> _poolParents;

        private int _currentPoolElement;

        private void Start()
        {
            _poolParents = new List<GameObject>();

            InstantiatePoolParent(PoolType.Knife, "Pool_1", 15);
        }

        public GameObject GetPoolObject(PoolType type)
        {
            var poolObject = _poolParents[(int) type].transform.GetChild(_currentPoolElement).gameObject;

            poolObject.GetComponent<IPoolable>().OnSpawn();

            var childCount = _poolParents[(int) type].transform.childCount;

            _currentPoolElement++;
            if (_currentPoolElement > childCount - 1) _currentPoolElement = 0;

            return poolObject;
        }

        private void InstantiatePoolParent(PoolType type, string poolParentsName, int poolLenght)
        {
            _poolParents.Add(new GameObject(poolParentsName));

            var poolScript = _poolParents[(int) type].AddComponent<Pool>();

            poolScript.poolObjects = new GameObject[poolLenght];

            for (var i = 0; i < poolLenght; i++)
            {
                poolScript.poolObjects[i] = Instantiate(
                    prefab[(int) type],
                    _poolParents[(int) type].transform.position,
                    _poolParents[(int) type].transform.rotation,
                    _poolParents[(int) type].transform);

                poolScript.poolObjects[i].SetActive(false);
            }
        }
    }
}
