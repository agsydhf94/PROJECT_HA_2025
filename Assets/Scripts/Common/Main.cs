using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace HA
{
    public enum SceneType
    {
        None,
        Empty,
        TitleScreen,
        InGameScene01,
        InGameScene02
    }

    // Main : ������ �������� ��, ���� ���� ����Ǵ� Entry ������ �ϴ� Ŭ����
    public class Main : Singleton<Main>
    {
        // #1. ���� ������ Scene�� �����ϴ� SceneController ������ ����
        // #2. ������ �ʱ�ȭ ������ ����ϴ� ����

        public SceneType CurrentSceneType => currentSceneType;

        [SerializeField] private SceneType currentSceneType = SceneType.None;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            StartCoroutine(MainSystemInitialize());
        }

        IEnumerator MainSystemInitialize()
        {
            yield return null;

            // TODO : �� �� �ʿ��� �ý����� �ʱ�ȭ
            // TODO : ù ����Ǵ� ������ ��ȯ
            ChangeScene(SceneType.TitleScreen);
        }

        public void SystemQuit()
        {
            // ���� ���� ���
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }


        public SceneBase CurrentSceneController => currentSceneController;
        public bool IsOnProgressSceneChanging { get; private set; } = false;

        private SceneBase currentSceneController = null;

        public void ChangeScene(SceneType sceneType, System.Action sceneLoadedCallback = null)
        {
            if (currentSceneType == sceneType)
                return;

            switch (sceneType)
            {
                case SceneType.TitleScreen:
                    ChangeScene<TitleScene>(sceneType, sceneLoadedCallback);
                    break;
                case SceneType.InGameScene01:
                    ChangeScene<InGameScene01>(sceneType, sceneLoadedCallback);
                    break;
                case SceneType.InGameScene02:
                    ChangeScene<InGameScene02>(sceneType, sceneLoadedCallback);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private void ChangeScene<T>(SceneType sceneType, System.Action sceneLoadedCallback = null) where T : SceneBase
        {
            if (IsOnProgressSceneChanging)
                return;

            StartCoroutine(ChangeSceneAsync<T>(sceneType, sceneLoadedCallback));
        }

        private IEnumerator ChangeSceneAsync<T>(SceneType sceneType, System.Action sceneLoadedCallback = null) where T : SceneBase
        {
            IsOnProgressSceneChanging = true;

            // TODO : Show Loading UI
            var loadingUI = UIManager.Instance.Show<LoadingUI>(UIList.LoadingUI);
            loadingUI.SetProgress(0f);

            if (currentSceneController != null)
            {
                yield return currentSceneController.OnEnd();
                Destroy(currentSceneController.gameObject);
            }

            loadingUI.SetProgress(0.1f);
            yield return null;

            AsyncOperation emtpyOperation = SceneManager.LoadSceneAsync(SceneType.Empty.ToString(), LoadSceneMode.Single);
            yield return new WaitUntil(() => emtpyOperation.isDone);

            loadingUI.SetProgress(0.3f);
            yield return null;

            GameObject sceneGo = new GameObject(typeof(T).Name);
            sceneGo.transform.parent = transform;
            currentSceneController = sceneGo.AddComponent<T>();
            currentSceneType = sceneType;

            yield return StartCoroutine(currentSceneController.OnStart());

            loadingUI.SetProgress(1f);
            yield return null;

            // Hide Loading UI
            UIManager.Instance.Hide<LoadingUI>(UIList.LoadingUI);

            IsOnProgressSceneChanging = false;
            sceneLoadedCallback?.Invoke();
        }
    }
}
