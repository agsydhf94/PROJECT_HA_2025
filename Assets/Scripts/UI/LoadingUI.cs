using HA;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class LoadingUI : UIBase
    {
        public static LoadingUI Instance => UIManager.Instance.GetUI<LoadingUI>(UIList.LoadingUI);

        public Image loadingBar;
        public TextMeshProUGUI loadingText;

        private void OnEnable()
        {
            loadingBar.fillAmount = 0;
            loadingText.text = $"0 %";
        }

        public void SetProgress(float progress)
        {
            loadingBar.fillAmount = progress;
            loadingText.text = $"{progress * 100}%";
        }
    }
}
