using UnityEngine;

namespace RogueLike2D
{
    public class Loader : MonoBehaviour
    {

        [SerializeField] private GameObject gameManager = default;

        private void Awake()
        {
            if (GameManager.instance == null) Instantiate(gameManager);
        }
    }
}
