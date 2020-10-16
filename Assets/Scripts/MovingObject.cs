using System.Collections;
using UnityEngine;

namespace RogueLike2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class MovingObject : MonoBehaviour
    {

        public float moveTime = 0.1f;

        protected static bool endMoving = true;

        [SerializeField] private LayerMask blockingLayer = default;

        private BoxCollider2D _boxCollider;
        private Rigidbody2D _rb2D;
        private float _inverseMoveTime;

        protected virtual void Start()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _rb2D = GetComponent<Rigidbody2D>();
            _inverseMoveTime = 1f / moveTime;
        }

        /// <summary>
        /// Попытка движения.
        /// Если ничего нет передвинуться.
        /// Если есть (blockingLayer) вызвать его поведенье.
        /// </summary>
        /// <typeparam name="T">скрипт обьекта на который мы движемся</typeparam>
        protected virtual void AttemptMove<T>(int xDir, int yDir)
            where T : Component
        {
            var canMove = Move(xDir, yDir, out var hit);

            if (hit.transform == null) return;

            var hitComponent = hit.transform.GetComponent<T>();

            if (!canMove && hitComponent != null)
            {
                OnCantMove(hitComponent);
            }
        }

        /// <summary>
        /// Двигаться если нет блокирующих обьектов (blockingLayer)
        /// </summary>
        /// <param name="yDir"></param>
        /// <param name="xDir"></param>
        /// <param name="hit">На выход обьект по направлению</param>
        /// <returns>true - если ничего нет по направлению.</returns>
        protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
        {
            Vector2 start = transform.position;
            var end = start + new Vector2(xDir, yDir);

            _boxCollider.enabled = false;
            hit = Physics2D.Linecast(start, end, blockingLayer);
            _boxCollider.enabled = true;

            if (hit.transform != null) return false;

            StartCoroutine(SmoothMovement(end));
            return true;
        }

        /// <summary>
        /// Плавное передвижение.
        /// </summary>
        /// <param name="end">Конечная точка</param>
        /// <returns></returns>
        private IEnumerator SmoothMovement(Vector3 end)
        {
            var sqrMagnitudeDistance = (transform.position - end).sqrMagnitude;

            // float.epsilon - число приближенное к нулю
            while (sqrMagnitudeDistance > float.Epsilon)
            {
                var newPosition = Vector3.MoveTowards(_rb2D.position, end, _inverseMoveTime * Time.deltaTime);
                _rb2D.MovePosition(newPosition);
                sqrMagnitudeDistance = (transform.position - end).sqrMagnitude;
                // позволяет выполнятся всему остальному коду с сохраненим состояния. 

                endMoving = false;
                yield return null;
            }

            endMoving = true;
        }

        protected abstract void OnCantMove<T>(T component) where T : Component;
    }
}
