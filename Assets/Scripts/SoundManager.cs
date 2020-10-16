using UnityEngine;

namespace RogueLike2D
{
    public class SoundManager : MonoBehaviour
    {

        public static SoundManager instance = null;
        public AudioSource musicSource;

        [SerializeField] private AudioSource efxSource = default;

        [SerializeField] private float lowPitchRange = .95f;
        [SerializeField] private float highPitchRange = 1.05f;

        private void Awake()
        {
            if (instance == null) instance = this;
            else if (instance != this) Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public void PlaySingle(AudioClip clip)
        {
            efxSource.clip = clip;
            efxSource.Play();
        }

        public void RandomizeSfx(params AudioClip[] clips)
        {
            var randomIndex = Random.Range(0, clips.Length);

            var randomPitch = Random.Range(lowPitchRange, highPitchRange);

            efxSource.pitch = randomPitch;
            efxSource.clip = clips[randomIndex];
            efxSource.Play();
        }
    }
}
