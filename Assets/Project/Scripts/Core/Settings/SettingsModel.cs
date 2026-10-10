using System;
using UnityEngine;

namespace MarketTycoon.Core.Settings
{
    public class SettingsModel
    {
        private const string MasterVolumeKey = "Settings_MasterVolume";
        private const string MusicVolumeKey = "Settings_MusicVolume";
        private const string SfxVolumeKey = "Settings_SfxVolume";

        public float MasterVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }

        public event Action<float> OnMasterVolumeChanged;
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSfxVolumeChanged;

        public SettingsModel()
        {
            LoadSettings();
        }

        public void SetMasterVolume(float volume)
        {
            MasterVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
            PlayerPrefs.Save();
            OnMasterVolumeChanged?.Invoke(MasterVolume);
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
            PlayerPrefs.Save();
            OnMusicVolumeChanged?.Invoke(MusicVolume);
        }

        public void SetSfxVolume(float volume)
        {
            SfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
            PlayerPrefs.Save();
            OnSfxVolumeChanged?.Invoke(SfxVolume);
        }

        private void LoadSettings()
        {
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1.0f);
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.8f);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.9f);
        }
    }
}