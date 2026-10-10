using System.Collections;
using MarketTycoon.UI.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarketTycoon.Core.Loading
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private LoadingScreenView _loadingScreenView;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            if (_loadingScreenView != null)
            {
                yield return _loadingScreenView.FadeInRoutine();
                _loadingScreenView.UpdateProgress(0f);
            }

            // Small delay to ensure UI renders
            yield return new WaitForSecondsRealtime(0.1f);

            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
            asyncOp.allowSceneActivation = false;

            while (!asyncOp.isDone)
            {
                // AsyncOperation.progress maxes out at 0.9 before activation
                float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);

                if (_loadingScreenView != null)
                {
                    _loadingScreenView.UpdateProgress(progress);
                }

                if (asyncOp.progress >= 0.9f)
                {
                    // Artificial small pause for smooth perception
                    yield return new WaitForSecondsRealtime(0.2f);
                    asyncOp.allowSceneActivation = true;
                }

                yield return null;
            }

            if (_loadingScreenView != null)
            {
                _loadingScreenView.UpdateProgress(1f);
                yield return _loadingScreenView.FadeOutRoutine();
            }
        }
    }
}