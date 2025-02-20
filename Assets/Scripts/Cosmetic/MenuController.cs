using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace HA
{
    public class MenuController : MonoBehaviour
    {

        [Header("Volume Settings")]
        public TMP_Text volumeValue;
        public Slider volumeSlider;
        private float defaultVolume = 0.5f;

        public GameObject confirmationPrompt;

        [Header("GamePlay Settings")]
        public TMP_Text controllerSensitivityValue;
        public Slider controllerSensitivitySlider;
        public int defaultSensitivity = 5;
        public int mainSensitivity = 5;

        [Header("Toggle Settings")]
        public Toggle invertYAxisToggle;

        [Header("Levels To Load")]
        public string newGameLevel;
        public string levelToLoad;

        [SerializeField]
        private GameObject noSavedGameDialogue = null;
        
        public void NewGameDialogueYes()
        {
            SceneManager.LoadScene(newGameLevel);
        }

        public void LoadGameDialogueYes()
        {
            if(PlayerPrefs.HasKey("SavedLevel"))
            {
                levelToLoad = PlayerPrefs.GetString("SavedLevel");
                SceneManager.LoadScene(levelToLoad);
            }
            else
            {
                noSavedGameDialogue.SetActive(true);
            }
        }

        public void ExitButtion()
        {
            Application.Quit();
        }


        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
            volumeValue.text = (volume * 100).ToString("0");
        }

        public void VolumeApply()
        {
            PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);

            StartCoroutine(ConfirmationBox());
        }

        public void SetControllerSensitivity(float sensitivity)
        {
            mainSensitivity = Mathf.RoundToInt(sensitivity);
            controllerSensitivityValue.text = (sensitivity * 100).ToString("0");
        }

        public void GamePlayApply()
        {
            if(invertYAxisToggle.isOn)
            {
                PlayerPrefs.SetInt("masterInvertY", 1);
            }
            else
            {
                PlayerPrefs.SetInt("masterInvertY", 0);
            }

            PlayerPrefs.SetFloat("masterSen", mainSensitivity);
            StartCoroutine(ConfirmationBox());
        }

        public void ResetButton(string menuType)
        {
            if(menuType == "Audio")
            {
                AudioListener.volume = defaultVolume;
                volumeSlider.value = defaultVolume;
                volumeValue.text = (defaultVolume * 100).ToString("0");
                VolumeApply();
            }

            if(menuType == "GamePlay")
            {
                controllerSensitivityValue.text = (defaultSensitivity * 100).ToString("0");
                controllerSensitivitySlider.value = defaultSensitivity;
                mainSensitivity = defaultSensitivity;
                invertYAxisToggle.isOn = false;
                GamePlayApply();
            }
        }

        public IEnumerator ConfirmationBox()
        {
            confirmationPrompt.SetActive(true);
            yield return new WaitForSeconds(2);
            confirmationPrompt.SetActive(false);
        }
    }
}
