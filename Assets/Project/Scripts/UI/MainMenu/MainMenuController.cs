using MarketTycoon.Core.Loading;
using MarketTycoon.Core.Settings;
using MarketTycoon.UI.MainMenu;
using MarketTycoon.UI.Settings;
using System;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;

namespace MarketTycoon.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Scene Config")]
        [SerializeField] private SceneAsset _gameplaySceneName;

        [Header("View References")]
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private SettingsView _settingsView;

        private SettingsModel _settingsModel;
        private SettingsController _settingsController;

        private void Start()
        {
            _settingsModel = new SettingsModel();

            if (_settingsView != null)
            {
                _settingsController = new SettingsController(_settingsModel, _settingsView);
                _settingsView.SetVisible(false);
            }

            if (_mainMenuView != null)
            {
                _mainMenuView.OnPlayClicked += HandlePlay;
                _mainMenuView.OnSettingsClicked += HandleSettings;
                _mainMenuView.OnExitClicked += HandleExit;
            }
        }

        private void HandlePlay()
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadSceneAsync(_gameplaySceneName.ToString());
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(_gameplaySceneName.ToString());
            }
        }

        private void HandleSettings()
        {
            _settingsController?.OpenSettings();
        }

        private void HandleExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnDestroy()
        {
            if (_mainMenuView != null)
            {
                _mainMenuView.OnPlayClicked -= HandlePlay;
                _mainMenuView.OnSettingsClicked -= HandleSettings;
                _mainMenuView.OnExitClicked -= HandleExit;
            }

            _settingsController?.Dispose();
        }
    }
}