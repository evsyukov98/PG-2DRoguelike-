using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace RogueLike2D
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]

    public class Player : MovingObject, IDamageble
    {
        
        private static readonly int PlayerHit = Animator.StringToHash("playerHit");
        private static readonly int PlayerChop = Animator.StringToHash("playerChop");

        [SerializeField] private int wallDamage = 1;
        [SerializeField] private int pointPerFood = 10;
        [SerializeField] private int pointPerSoda = 20;
        [SerializeField] private float restartLevelDelay = 1f;

        [SerializeField] private Text foodText = default;
        
        [SerializeField] private AudioClip eatSound1 = default;
        [SerializeField] private AudioClip eatSound2 = default;
        [SerializeField] private AudioClip drinkSound1 = default;
        [SerializeField] private AudioClip drinkSound2 = default;
        [SerializeField] private AudioClip gameOverSound = default;

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private int _food;

        private bool _isInDistanceAttack = false;

        protected override void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
            _animator = GetComponent<Animator>();

            _food = GameManager.instance.playerFoodPoint;

            foodText.text = $"Food: {_food}";

            base.Start();
        }

        public void Damaged(int loss)
        {
            _animator.SetTrigger(PlayerHit);
            _food -= loss;
            foodText.text = $"-{loss} Food: {_food}";
            CheckIfGameOver();
        }

        protected override void AttemptMove(int xDir, int yDir)
        {
            _food--;
            foodText.text = $"Food: {_food}";

            base.AttemptMove(xDir, yDir);

            CheckIfGameOver();

            GameManager.instance.playersTurns = false;
        }

        protected override void OnCantMove(IDamageble component)
        {
            var hitWall = component;
            
            hitWall.Damaged(wallDamage);
            
            _animator.SetTrigger(PlayerChop);
        }

        private void OnDisable()
        {
            GameManager.instance.playerFoodPoint = _food;
        }

        private Vector2 _direction;
        
        private void Update()
        {
            if (!GameManager.instance.playersTurns) return;
            if (IsMoving) return;
            
            if (Input.GetKeyDown(KeyCode.X))
            {
                _isInDistanceAttack = !_isInDistanceAttack;
                foodText.text = _isInDistanceAttack ? "Distance attack" : "Moving";
            }
            var horizontal = (int) Input.GetAxisRaw("Horizontal");
            var vertical = (int) Input.GetAxisRaw("Vertical");
            
            if (horizontal == 0 && vertical == 0) return;
            if (horizontal != 0) vertical = 0;
            
            _direction = new Vector2(horizontal,vertical);

            if (_isInDistanceAttack) StartCoroutine(LaunchProjectile());
            else AttemptMove(horizontal, vertical);
        }
        
        private IEnumerator LaunchProjectile()
        {
            while (_direction == Vector2.zero)
            {
                IsMoving = false;
                yield return null;
            }

            _isInDistanceAttack = false;
            var projectileObject = PoolManager.instance.GetPoolObject(PoolType.Knife);
            
            projectileObject.transform.position = _rigidbody2D.position + _direction;
            IsMoving = true;

            if (projectileObject.TryGetComponent<Projectile>(out var projectile))
            {
                projectile.Launch(_direction, 0.05f, OnProjectileDespawned);
                
                _food -= 10;
                foodText.text = $"-10 Food: {_food}";
                yield break;
            }

            OnProjectileDespawned();
        }

        private void OnProjectileDespawned()
        {
            GameManager.instance.playersTurns = false;
            IsMoving = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Exit"))
            {
                Invoke(nameof(Restart), restartLevelDelay);
                enabled = false;
            }
            else if (other.CompareTag("Food"))
            {
                _food += pointPerFood;
                foodText.text = $"+{pointPerFood} Food: {_food}";
                SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
                other.gameObject.SetActive(false);
            }
            else if (other.CompareTag("Soda"))
            {
                _food += pointPerSoda;
                foodText.text = $"+{pointPerSoda} Food: {_food}";
                SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
                other.gameObject.SetActive(false);
            }
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void CheckIfGameOver()
        {
            if (_food > 0) return;

            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}