using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RogueLike2D
{
    [RequireComponent(typeof(Animator))]
    public class Player : MovingObject
    {
        
        private static readonly int PlayerHit = Animator.StringToHash("playerHit");
        private static readonly int PlayerChop = Animator.StringToHash("playerChop");

        [SerializeField] private int wallDamage = 1;
        [SerializeField] private int pointPerFood = 10;
        [SerializeField] private int pointPerSoda = 20;
        [SerializeField] private float restartLevelDelay = 1f;

        [SerializeField] private Text foodText = default;

        [SerializeField] private AudioClip moveSound1 = default;
        [SerializeField] private AudioClip moveSound2 = default;
        [SerializeField] private AudioClip eatSound1 = default;
        [SerializeField] private AudioClip eatSound2 = default;
        [SerializeField] private AudioClip drinkSound1 = default;
        [SerializeField] private AudioClip drinkSound2 = default;
        [SerializeField] private AudioClip gameOverSound = default;

        private Vector2 _touchOrigin = -Vector2.one;

        private Animator _animator;
        private int _food;

        protected override void Start()
        {
            _animator = GetComponent<Animator>();

            _food = GameManager.instance.playerFoodPoint;

            foodText.text = $"Food: {_food}";

            base.Start();
        }

        public void LoseFood(int loss)
        {
            _animator.SetTrigger(PlayerHit);
            _food -= loss;
            foodText.text = $"-{loss} Food: {_food}";
            CheckIfGameOver();
        }

        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            _food--;
            foodText.text = $"Food: {_food}";

            base.AttemptMove<T>(xDir, yDir);


            if (Move(xDir, yDir, out _))
            {
                SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            }

            CheckIfGameOver();

            GameManager.instance.playersTurns = false;
        }

        protected override void OnCantMove<T>(T component)
        {
            var hitWall = component as Wall;
            hitWall.DamageWall(wallDamage);
            _animator.SetTrigger(PlayerChop);
        }

        private void OnDisable()
        {
            GameManager.instance.playerFoodPoint = _food;
        }

        private void Update()
        {
            if (!GameManager.instance.playersTurns) return;

            var horizontal = 0;
            var vertical = 0;

#if UNITY_STANDALONE || UNITY_WEBPLAYER

            horizontal = (int) Input.GetAxisRaw("Horizontal");
            vertical = (int) Input.GetAxisRaw("Vertical");

            if (horizontal != 0) vertical = 0;

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            if (Input.touchCount > 0)
            {
                Touch myTouch = Input.touches[0];

                if (myTouch.phase == TouchPhase.Began)
                {
                    _touchOrigin = myTouch.position;
                }

                else if (myTouch.phase == TouchPhase.Ended && _touchOrigin.x >= 0)
                {
                    Vector2 touchEnd = myTouch.position;

                    float x = touchEnd.x - _touchOrigin.x;

                    float y = touchEnd.y - _touchOrigin.y;

                    _touchOrigin.x = -1;

                    if (Mathf.Abs(x) > Mathf.Abs(y))
                        horizontal = x > 0 ? 1 : -1;
                    else
                        vertical = y > 0 ? 1 : -1;
                }
            }
#endif

            if (horizontal != 0 || vertical != 0)
            {
                AttemptMove<Wall>(horizontal, vertical);
            }
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