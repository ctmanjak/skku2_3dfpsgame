using Core;
using UnityEngine;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private PlayerBomb _playerBomb;

        public void Fire()
        {
            _playerBomb.Fire();
        }

        public void GameOver()
        {
            GameManager.Instance.GameOver();
        }
    }
}