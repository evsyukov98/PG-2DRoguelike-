using System.Collections;
using UnityEngine;

namespace RogueLike2D
{
    [RequireComponent(typeof(Animator))]
    public class Enemy : MovingObject
    {
        
        private static readonly int EnemyAttack = Animator.StringToHash("enemyAttack");

        [SerializeField] private int playerDamage = default;
        [SerializeField] private AudioClip enemyAttack1 = default;
        [SerializeField] private AudioClip enemyAttack2 = default;

        private Animator _animator;
        private Transform _target;
        private bool _skipMove;

        protected override void Start()
        {
            GameManager.instance.AddEnemyToList(this);
            
            _animator = GetComponent<Animator>();

            _target = GameObject.FindGameObjectWithTag("Player").transform;

            base.Start();
        }

        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            if (_skipMove)
            {
                _skipMove = false;
                return;
            }

            base.AttemptMove<T>(xDir, yDir);

            _skipMove = true;
        }

        public IEnumerator MoveEnemy()
        {
            while (!endMoving)
            {
                yield return null;
            }
            
            var xDir = 0;
            var yDir = 0;

            if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon)
            {
                yDir = _target.position.y > transform.position.y ? 1 : -1;
            }
            else
            {
                xDir = _target.position.x > transform.position.x ? 1 : -1;
            }

            AttemptMove<Player>(xDir, yDir);
        }

        
        
        protected override void OnCantMove<T>(T component)
        {
            var hitPlayer = component as Player;

            hitPlayer.LoseFood(playerDamage);

            _animator.SetTrigger(EnemyAttack);

            SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
        }
    }
}
