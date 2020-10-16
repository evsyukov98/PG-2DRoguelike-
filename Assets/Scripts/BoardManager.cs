using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace RogueLike2D
{
    public class BoardManager : MonoBehaviour
    {

        [Serializable]
        public class Count
        {
            public int minimum;
            public int maximum;

            public Count(int min, int max)
            {
                minimum = min;
                maximum = max;
            }
        }

        [SerializeField] private int columns = 8;
        [SerializeField] private int rows = 8;
        [SerializeField] private Count wallCount = new Count(5, 9);
        [SerializeField] private Count foodCount = new Count(1, 9);

        [SerializeField] private GameObject exit = default;
        [SerializeField] private GameObject[] floorTiles = default;
        [SerializeField] private GameObject[] wallTiles = default;
        [SerializeField] private GameObject[] foodTiles = default;
        [SerializeField] private GameObject[] enemyTiles = default;
        [SerializeField] private GameObject[] outerWallTiles = default;

        private Transform _boardHolder;

        private readonly List<Vector3> _gridPosition = new List<Vector3>();

        private void InitialiseList()
        {
            _gridPosition.Clear();

            for (var x = 1; x < columns - 1; x++)
            {
                for (var y = 1; y < rows; y++)
                {
                    _gridPosition.Add(new Vector3(x, y, 0f));
                }
            }
        }

        private void BoardSetup()
        {
            _boardHolder = new GameObject("Board").transform;

            for (var x = -1; x < columns + 1; x++)
            {
                for (var y = -1; y < rows + 1; y++)
                {
                    var toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                    if (x == -1 || x == columns || y == -1 || y == rows)
                    {
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    }

                    var instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);

                    instance.transform.SetParent(_boardHolder);
                }
            }
        }

        private Vector3 RandomPosition()
        {
            var randomIndex = Random.Range(0, _gridPosition.Count);
            var randomPosition = _gridPosition[randomIndex];

            _gridPosition.RemoveAt(randomIndex);
            return randomPosition;
        }

        /// <summary>
        /// Генерация из массива случайных обьектов в случайных местах.
        /// </summary>
        /// <param name="tileArray">Массив обьектов</param>
        /// <param name="minimum">Минимальное кол-во обьектов</param>
        /// <param name="maximum">Максимальное кол-во обьектов</param>
        private void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            var objectCount = Random.Range(minimum, maximum + 1);

            for (var i = 0; i < objectCount; i++)
            {
                var randomPosition = RandomPosition();
                var tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }

        public void SetupScene(int level)
        {
            BoardSetup();
            InitialiseList();
            LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
            LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

            var enemyCount = (int) Mathf.Log(level, 2f);
            LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        }
    }
}
