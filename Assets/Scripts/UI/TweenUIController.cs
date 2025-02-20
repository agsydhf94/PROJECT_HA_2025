using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HA
{
    public class TweenUIController : Singleton<TweenUIController>
    {
        public CanvasGroup centerAimCircle; // CanvasGroup을 사용

        public void ShowAimCircle()
        {

            centerAimCircle.gameObject.SetActive(true);
            centerAimCircle.transform.localScale = Vector3.zero;

            CanvasGroup canvasGroup = centerAimCircle.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = centerAimCircle.gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;


            // Tween 실행
            canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutQuad);
            centerAimCircle.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        public void HideAimCircle()
        {
            centerAimCircle.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => centerAimCircle.gameObject.SetActive(false));
        }
    }
}
