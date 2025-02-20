using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class CheckPopUpUI : UIBase
    {
        public static CheckPopUpUI instance;
        public static CheckPopUpUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CheckPopUpUI>();
                }
                return instance;
            }
        }

        public TMP_Text textComponent;
        public TMP_Text tempTMP;
        public string textData;
        public float showingSpeed;
        private int currentTextIndex = 0;
        float timer = 0f;


        public Image blinkingImage;
        public Color blinkingColor;
        public float blinkSpeed;

        public Image upperBarImage;
        public Image backGroundImage;

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            textComponent.text = "";
        }

        public void WriteText(string textInput)
        {
            while(currentTextIndex < textInput.Length)
            {
                timer += Time.deltaTime;
                if(timer > showingSpeed)
                {
                    textComponent.text += textInput[currentTextIndex].ToString();
                    currentTextIndex++;
                    timer = 0f;
                }
            }
            currentTextIndex = 0;
        }

        public void ImageBlink()
        {
            // 알파 값을 PingPong으로 변경
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 0.66f) + 0.01f;

            Color newColor = blinkingImage.color;
            newColor.a = alpha;
            blinkingImage.color = newColor;
        }



        // UI 애니메이션 시퀀스를 시작하는 메서드
        public void StartUIAnimationSequence(string textInput, Color blinkingColorInput)
        {
            StartCoroutine(UIAnimationSequence(textInput, blinkingColorInput));
        }

        // UI 애니메이션 시퀀스
        private IEnumerator UIAnimationSequence(string textInput, Color blinkingColorInput)
        {
            // 1. upperBarImage와 backGroundImage 채우기
            yield return StartCoroutine(FillImage(upperBarImage, 1f));
            yield return StartCoroutine(FillImage(backGroundImage, 1f));

            Debug.Log(textInput);
            textComponent.maxVisibleCharacters = textInput.Length;
            WriteText(textInput);


            // 2. 이미지 깜빡임을 3초 동안 유지
            float blinkTimer = 3f;

            Color newColor = blinkingColorInput;
            blinkingImage.color = newColor;

            while (blinkTimer > 0f)
            {
                ImageBlink();
                blinkTimer -= Time.deltaTime;
                yield return null;
            }

            // 깜빡거림을 멈추기 위해 알파 값을 1로 고정
            Color temp_Transparent = blinkingColorInput;
            temp_Transparent.a = 0f;
            blinkingImage.color = temp_Transparent;
            


            yield return StartCoroutine(FadeOutText(textInput));


            // 3. backGroundImage와 upperBarImage를 천천히 줄이기
            yield return StartCoroutine(FillImage(backGroundImage, 0f));
            yield return StartCoroutine(FillImage(upperBarImage, 0f));
        }

        // fillAmount를 점진적으로 증가/감소시키는 코루틴
        private IEnumerator FillImage(Image image, float targetFillAmount)
        {
            float startFillAmount = image.fillAmount;
            float elapsed = 0f;
            float duration = 0.1f; // 진행 시간 조절 가능

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                image.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsed / duration);
                yield return null;
            }

            image.fillAmount = targetFillAmount; // 타겟에 정확히 맞추기
        }

        private IEnumerator FadeOutText(string textInput)
        {
            int totalCharacters = textInput.Length;
            tempTMP.text = textInput;
            for (int i = totalCharacters; i >= 0; i--)
            {
                
                textComponent.maxVisibleCharacters = i; // 표시할 최대 문자의 개수를 점점 줄임
                yield return new WaitForSeconds(0.025f); // 글자가 사라지는 속도 조절
            }
            tempTMP.text = "";
            textComponent.text = "";
        }



    }
}
