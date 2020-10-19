using System.Collections;
using UnityEngine;

namespace RogueLike2D
{
    [RequireComponent(typeof(Animator))]
    public class Enemy : MovingObject, IDamageble
    {
        
        private static readonly int EnemyAttack = Animator.StringToHash("enemyAttack");

        [SerializeField] private int hp = 2;
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

        protected override void AttemptMove(int xDir, int yDir)
        {
            if (_skipMove)
            {
                _skipMove = false;
                return;
            }

            base.AttemptMove(xDir, yDir);

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

            AttemptMove(xDir, yDir);
        }

        protected override void OnCantMove(IDamageble component)
        {
            if (!(component is Player)) return;
            
            component.Damaged(playerDamage);

            _animator.SetTrigger(EnemyAttack);

            SoundManager.instance.RandomizeSfx(enemyAttack2);
        }


        public void Damaged(int loss)
        {
            hp -= loss;

            SoundManager.instance.RandomizeSfx(enemyAttack1);

            if (hp > 0) return;
            
            GameManager.instance.RemoveEnemyFromList(gameObject.GetComponent<Enemy>());
            
            Destroy(gameObject);
        }
    }
}
