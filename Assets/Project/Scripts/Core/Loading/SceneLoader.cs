using System.Collections;
using MarketTycoon.UI.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarketTycoon.Core.Loading
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [Header("View Reference")]
        [SerializeField] private LoadingScreenView _loadingScreenView;

        [Header("Pacing")]
        [Tooltip("Minimum real seconds the loading screen will be visible for a smooth UX")]
        [SerializeField] private float _minDisplayDuration = 1.5f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                // If returning to MainMenu, destroy duplicate instances automatically
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_loadingScreenView != null)
            {
                DontDestroyOnLoad(_loadingScreenView.gameObject);
                _loadingScreenView.ResetView();
                _loadingScreenView.gameObject.SetActive(false);
            }
        }

        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            if (_loadingScreenView != null)
            {
                // 1. Reset progress to 0 FIRST before activating the object
                _loadingScreenView.ResetView();
                _loadingScreenView.gameObject.SetActive(true);
            }

            // Small delay to let UI render 0% cleanly
            yield return null;

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
            asyncOp.allowSceneActivation = false;

            float timer = 0f;

            // 2. Drive loading progress steadily based on real loading and minimum screen time
            while (asyncOp.progress < 0.9f || timer < _minDisplayDuration)
            {
                timer += Time.unscaledDeltaTime;

                float realProgress = Mathf.Clamp01(asyncOp.progress / 0.9f);
                float timerProgress = Mathf.Clamp01(timer / _minDisplayDuration);
                float combinedTarget = Mathf.Min(realProgress, timerProgress) * 0.9f;

                if (_loadingScreenView != null)
                {
                    _loadingScreenView.SetTargetProgress(combinedTarget);
                }

                yield return null;
            }

            // 3. Push to 100% and wait until visual bar smoothly reaches 100%
            if (_loadingScreenView != null)
            {
                _loadingScreenView.SetTargetProgress(1.0f);

                while (!_loadingScreenView.IsFillComplete())
                {
                    yield return null;
                }
            }

            // Brief readable pause at 100%
            yield return new WaitForSecondsRealtime(0.2f);

            // 4. Activate new scene
            asyncOp.allowSceneActivation = true;

            while (!asyncOp.isDone)
            {
                yield return null;
            }

            // 5. Smooth fade out
            if (_loadingScreenView != null)
            {
                yield return _loadingScreenView.FadeOutRoutine();
            }
        }
    }
}