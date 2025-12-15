using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        private EGameState _state = EGameState.Ready;
        public EGameState State => _state;
        [SerializeField] private TextMeshProUGUI _stateTextUI;

        private void Awake()
        {
            if (_instance != null) Destroy(gameObject);
            _instance = this;
        }

        private void Start()
        {
            Time.timeScale = 0;
            _stateTextUI.gameObject.SetActive(true);
            _state = EGameState.Ready;
            _stateTextUI.text = "READY..";
            StartCoroutine(StartToPlay_Coroutine());
        }
        
        private IEnumerator StartToPlay_Coroutine()
        {
            yield return new WaitForSecondsRealtime(2f);
            _stateTextUI.text = "START!";
            yield return new WaitForSecondsRealtime(0.5f);
            _state = EGameState.Playing;
            _stateTextUI.gameObject.SetActive(false);
            Time.timeScale = 1;
        }

        public void GameOver()
        {
            _stateTextUI.text = "Game Over...";
            _state = EGameState.GameOver;
            _stateTextUI.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
}