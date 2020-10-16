using UnityEngine;

namespace RogueLike2D
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Wall : MonoBehaviour
    {

        [SerializeField] private Sprite dmgSprite = default;
        [SerializeField] private int hp = 4;
        [SerializeField] private AudioClip chopSound1 = default;
        [SerializeField] private AudioClip chopSound2 = default;

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void DamageWall(int loss)
        {
            SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
            _spriteRenderer.sprite = dmgSprite;
            hp -= loss;
            if (hp <= 0) gameObject.SetActive(false);
        }
    }
}