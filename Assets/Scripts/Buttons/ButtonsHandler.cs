using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Buttons
{
    public class ButtonsHandler : MonoBehaviour
    {
        [SerializeField] private Button _pause;
        [SerializeField] private Button _play;

        private void Start()
        {
            _pause.onClick.AddListener(Pause);
            _play.onClick.AddListener(Play);
        }

        private void OnDestroy()
        {
            _pause.onClick.RemoveListener(Pause);
            _play.onClick.RemoveListener(Play);
        }

        private void Pause()
        {
            Time.timeScale = 0;

            SwitchButtons(false);
        }

        private void Play()
        {
            Time.timeScale = 1;

            SwitchButtons(true);
        }

        private void SwitchButtons(bool showPause)
        {
            _pause.gameObject.SetActive(showPause);
            _play.gameObject.SetActive(!showPause);
        }
    }
}