using System;
using UnityEngine;
using UnityEngine.UI;

namespace MarketTycoon.UI.Settings
{
    public class SettingsView : MonoBehaviour
    {
        [Header("Audio Sliders")]
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        [Header("Buttons")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _clearSaveButton;

        public event Action<float> OnMasterVolumeSliderChanged;
        public event Action<float> OnMusicVolumeSliderChanged;
        public event Action<float> OnSfxVolumeSliderChanged;
        public event Action OnCloseClicked;
        public event Action OnClearSaveClicked;

        private void Awake()
        {
            if (_masterVolumeSlider != null)
                _masterVolumeSlider.onValueChanged.AddListener(val => OnMasterVolumeSliderChanged?.Invoke(val));

            if (_musicVolumeSlider != null)
                _musicVolumeSlider.onValueChanged.AddListener(val => OnMusicVolumeSliderChanged?.Invoke(val));

            if (_sfxVolumeSlider != null)
                _sfxVolumeSlider.onValueChanged.AddListener(val => OnSfxVolumeSliderChanged?.Invoke(val));

            if (_closeButton != null)
                _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

            if (_clearSaveButton != null)
                _clearSaveButton.onClick.AddListener(() => OnClearSaveClicked?.Invoke());
        }

        public void InitializeSliders(float master, float music, float sfx)
        {
            if (_masterVolumeSlider != null) _masterVolumeSlider.value = master;
            if (_musicVolumeSlider != null) _musicVolumeSlider.value = music;
            if (_sfxVolumeSlider != null) _sfxVolumeSlider.value = sfx;
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}