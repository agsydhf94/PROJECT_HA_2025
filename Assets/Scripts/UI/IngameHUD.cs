using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using static Cinemachine.DocumentationSortingAttribute;

namespace HA
{
    public class IngameHUD : MonoBehaviourPun
    {

        // public GunController gunController;

        // 싱글턴 할당 변수 선언
        public static IngameHUD instance;

        // 싱글턴 접근 후 싱글턴의 존재가 확인되면 할당
        public static IngameHUD Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<IngameHUD>();
                }
                return instance;
            }
        }

        
        
        public PlayerController playerController;
        public Weapon currentWeapon;

        public GameObject bulletHUD;

        public TMP_Text[] text_Bullet; // 탄 개수 반영

        public TMP_Text scoreText;
        public TMP_Text waveText;
        public GameObject gameOverUI;

        public RectTransform fxHolder;
        public Image circleImage;
        public TMP_Text levelText;
        public int currentCharacterLevel;

        /*
        private void Awake()
        {
            gunController = GameObject.Find("WeaponHolder").GetComponent<GunController>();
        }
        */

        private void Awake()
        {
            bulletHUD = GameObject.FindGameObjectWithTag("BulletUI");
            levelText.text = $"{currentCharacterLevel}";
        }

        private void Update()
        {
            if(PhotonNetwork.IsConnected && !photonView.IsMine)
            {
                return;
            }

            if(bulletHUD != null && GameManager.Instance.currentCharacterMode == CharacterMode.Kunoichi)
            {
                bulletHUD.SetActive(false);
                return;
            }
            else
            {
                bulletHUD.SetActive(true);
                CheckBullet();
            }

            var currentCharacter = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>();
            currentCharacterLevel = currentCharacter.level;

            // currentWeapon = currentCharacter.currentWeapon.GetComponent<Weapon>();

            circleImage.fillAmount = (currentCharacter.exp - currentCharacter.expNeededOnPreviousLevel)
                                    / (currentCharacter.expToNextLevel - currentCharacter.expNeededOnPreviousLevel);
            
        }

        private void CheckBullet()
        {

            // currentWeapon = playerController.currentWeapon;
            if(GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().currentWeapon)
            {
                currentWeapon = GameManager.Instance.currentActiveCharacter.GetComponent<PlayerData>().currentWeapon.GetComponent<Weapon>();
                text_Bullet[0].text = currentWeapon.bulletTotal.ToString();
                text_Bullet[1].text = currentWeapon.currentBullet_inMagazine.ToString();
            }
            
        }

        public void UpdateLevelUI()
        {
            if (levelText != null)
            {
                levelText.text = $"{currentCharacterLevel}";
            }
        }

        public void UpdateScoreText(int score)
        {
            scoreText.text = "Score : " + score.ToString();
        }

        public void UpdateWaveText(int wave, int count)
        {
            waveText.text = "Wave : " + wave.ToString() + "\nEnemy Remaining : " + count.ToString();
        }

        public void GameOverScreen(bool active)
        {
            gameOverUI.SetActive(active);
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
