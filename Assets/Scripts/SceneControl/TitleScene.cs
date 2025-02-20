using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HA
{
    public class TitleScene : SceneBase
    {
        public override bool IsAdditiveScene => false;

        public override IEnumerator OnStart()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(SceneType.TitleScreen.ToString(), LoadSceneMode);
            while (!async.isDone)
            {
                yield return null;

                float progress = async.progress / 0.9f;
                LoadingUI.Instance.SetProgress(progress);
            }

            // Title UI ¶ç¿ì±â
            UIManager.Instance.Show<TitleUI>(UIList.TitleUI);
        }

        public override IEnumerator OnEnd()
        {
            // Title UI ²ô±â
            UIManager.Instance.Hide<TitleUI>(UIList.TitleUI);

            yield return null;
        }

    }
}
