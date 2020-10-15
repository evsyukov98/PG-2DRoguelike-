using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

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

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5,9);
    public Count foodCount = new Count(1,9);
    public GameObject exit;
    
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    
    // куда возможно поставить обьекты
    private List<Vector3> gridPosition = new List<Vector3>();

    private void InitialiseList()
    {
        gridPosition.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows; y++)
            {
                gridPosition.Add(new Vector3(x,y,0f));
            }
        }
    }

    private void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x< columns+1; x++)
        {
            for (int y = -1; y < rows +1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                
                // Стенки находятся на -1 значениях и максимальных в нашем случае(columns = 8)
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    private Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPosition.Count());
        Vector3 randomPosition = gridPosition[randomIndex];
        
        // удаляем из списка данную позицию 
        gridPosition.RemoveAt(randomIndex);
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
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        
        // увеличение кол-ва врагов в логарифмическом порядке 2^x = level
        int enemyCount = (int) Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount,enemyCount);

        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
 
}
