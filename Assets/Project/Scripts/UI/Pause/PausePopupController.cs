using System;
using MarketTycoon.Core.Bootstrap;
using MarketTycoon.Core.Loading;
using MarketTycoon.Core.Settings;
using MarketTycoon.UI.Pause;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarketTycoon.UI.Pause
{
    public class PausePopupController : IDisposable
    {
        private readonly PausePopupView _view;
        private readonly SettingsController _settingsController;
        private readonly string _mainMenuSceneName;

        public bool IsPaused { get; private set; }

        public PausePopupController(
            PausePopupView view,
            SettingsController settingsController,
            string mainMenuSceneName = "01_MainMenu")
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _settingsController = settingsController;
            _mainMenuSceneName = mainMenuSceneName;

            _view.OnResumeClicked += Resume;
            _view.OnSettingsClicked += HandleSettings;
            _view.OnMainMenuClicked += HandleReturnToMainMenu;

            _view.SetVisible(false);
        }

        public void Pause()
        {
            IsPaused = true;
            Time.timeScale = 0f;
            _view.SetVisible(true);
        }

        public void Resume()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            _view.SetVisible(false);
        }

        private void HandleSettings()
        {
            _settingsController?.OpenSettings();
        }

        private void HandleReturnToMainMenu()
        {
            // Restore normal time before transitioning scenes
            Time.timeScale = 1f;

            // Trigger an auto-save so current progress is preserved
            if (GameBootstrap.Instance != null)
            {
                GameBootstrap.Instance.SaveGameState();
            }

            // Load main menu scene smoothly via SceneLoader
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadSceneAsync(_mainMenuSceneName);
            }
            else
            {
                SceneManager.LoadScene(_mainMenuSceneName);
            }
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.OnResumeClicked -= Resume;
                _view.OnSettingsClicked -= HandleSettings;
                _view.OnMainMenuClicked -= HandleReturnToMainMenu;
            }

            Time.timeScale = 1f;
        }
    }
}