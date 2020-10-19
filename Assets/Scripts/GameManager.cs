using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RogueLike2D
{
    [RequireComponent(typeof(BoardManager))]
    public class GameManager : Singleton<GameManager>
    {
        
        public int playerFoodPoint = 100;

        [SerializeField] private float levelStartDelay = 2f;
        [SerializeField] private float turnDelay = 0.1f;
        [SerializeField] private int level = 0;
        [SerializeField] private BoardManager boardScript;

        [HideInInspector] public bool playersTurns = true;

        private Text _levelText;
        private GameObject _levelImage;
        private List<Enemy> _enemies;
        private bool _enemiesMoving;

        private void Awake()
        {

            DontDestroyOnLoad(gameObject);
            _enemies = new List<Enemy>();
            boardScript = GetComponent<BoardManager>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void GameOver()
        {
            _levelText.text = $"After {level} days, you starved.";
            _levelImage.SetActive(true);
            enabled = false;
        }

        public void AddEnemyToList(Enemy script)
        {
            _enemies.Add(script);
        }
        
        public void RemoveEnemyFromList(Enemy script)
        {
            _enemies.Remove(script);
        }

        private void Update()
        {
            if (playersTurns || _enemiesMoving) return;

            StartCoroutine(MoveEnemies());
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            level += 1;
            InitGame();
        }

        private void InitGame()
        {
            _levelImage = GameObject.Find("LevelImage");
            _levelText = GameObject.Find("LevelText").GetComponent<Text>();
            _levelText.text = $"Day {level}";
            _levelImage.SetActive(true);
            Invoke(nameof(HideLevelImage), levelStartDelay);

            _enemies.Clear();
            boardScript.SetupScene(level);
        }

        private void HideLevelImage()
        {
            _levelImage.SetActive(false);
        }

        private IEnumerator MoveEnemies()
        {
            _enemiesMoving = true;
            yield return new WaitForSeconds(turnDelay);

            if (_enemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDelay);
            }

            foreach (var enemy in _enemies)
            {
                StartCoroutine(enemy.MoveEnemy());
                yield return new WaitForSeconds(enemy.moveTime);
            }

            playersTurns = true;
            _enemiesMoving = false;
        }
    }
}