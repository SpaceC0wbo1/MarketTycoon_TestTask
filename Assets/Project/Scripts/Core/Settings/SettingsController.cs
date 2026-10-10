using System;
using MarketTycoon.Core.SaveSystem;
using MarketTycoon.UI.Settings;
using UnityEngine;
using UnityEngine.Audio;

namespace MarketTycoon.Core.Settings
{
    public class SettingsController : IDisposable
    {
        private readonly SettingsModel _model;
        private readonly SettingsView _view;
        private readonly AudioMixer _audioMixer;

        // Exposed parameter names inside AudioMixer (if using AudioMixer)
        private const string MasterParam = "MasterVolume";
        private const string MusicParam = "MusicVolume";
        private const string SfxParam = "SfxVolume";

        public SettingsController(SettingsModel model, SettingsView view, AudioMixer audioMixer = null)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _audioMixer = audioMixer;

            _view.InitializeSliders(_model.MasterVolume, _model.MusicVolume, _model.SfxVolume);

            // Apply loaded volumes immediately
            ApplyAudioVolume(MasterParam, _model.MasterVolume);
            ApplyAudioVolume(MusicParam, _model.MusicVolume);
            ApplyAudioVolume(SfxParam, _model.SfxVolume);

            // Subscribe to UI interactions
            _view.OnMasterVolumeSliderChanged += HandleMasterVolumeChanged;
            _view.OnMusicVolumeSliderChanged += HandleMusicVolumeChanged;
            _view.OnSfxVolumeSliderChanged += HandleSfxVolumeChanged;
            _view.OnClearSaveClicked += HandleClearSave;
            _view.OnCloseClicked += HandleClose;
        }

        private void HandleMasterVolumeChanged(float value)
        {
            _model.SetMasterVolume(value);
            ApplyAudioVolume(MasterParam, value);
        }

        private void HandleMusicVolumeChanged(float value)
        {
            _model.SetMusicVolume(value);
            ApplyAudioVolume(MusicParam, value);
        }

        private void HandleSfxVolumeChanged(float value)
        {
            _model.SetSfxVolume(value);
            ApplyAudioVolume(SfxParam, value);
        }

        private void ApplyAudioVolume(string paramName, float linearValue)
        {
            if (_audioMixer != null)
            {
                // Convert linear (0..1) to decibels (-80dB..0dB)
                float db = linearValue > 0.001f ? Mathf.Log10(linearValue) * 20f : -80f;
                _audioMixer.SetFloat(paramName, db);
            }
            else
            {
                // Fallback to global volume if AudioMixer is not supplied
                if (paramName == MasterParam)
                {
                    AudioListener.volume = linearValue;
                }
            }
        }

        private void HandleClearSave()
        {
            SaveLoadManager.ClearSave();
            Debug.Log("[SettingsController] Save data cleared via settings menu.");
        }

        private void HandleClose()
        {
            _view.SetVisible(false);
        }

        public void OpenSettings()
        {
            _view.SetVisible(true);
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.OnMasterVolumeSliderChanged -= HandleMasterVolumeChanged;
                _view.OnMusicVolumeSliderChanged -= HandleMusicVolumeChanged;
                _view.OnSfxVolumeSliderChanged -= HandleSfxVolumeChanged;
                _view.OnClearSaveClicked -= HandleClearSave;
                _view.OnCloseClicked -= HandleClose;
            }
        }
    }
}