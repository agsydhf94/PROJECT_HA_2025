using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace HA
{
    public class GameManager : Singleton<GameManager>
    {

        [Header("Character Switch")]
        public GameObject[] characters;
        private int currentCharacterIndex = 0;
        public GameObject currentActiveCharacter;
        public CharacterMode currentCharacterMode;
        public GameObject currentCameraPivot;

        [Header("Character Skill Information")]
        public SkillSlot[] skillSlots;
        public List<SkillSlot> currentcharacterActiveSkills;
        public GameObject[] currentcharacterActiveSkillsUI_Slots;


        [Header("Character Animation Information")]
        public Animator currentCharacterAnimator;


        [Header("Player Level Information")]

        public float money;
        

        public delegate void CharacterSwitchHandler(CharacterBase newCharacter);
        public event CharacterSwitchHandler OnCharacterSwitch;
        public CinemachineVirtualCamera cinemachineVirtualCamera;


        private delegate void PauseMenu();
        private PauseMenu pauseGame;
        public bool isPause;



        private int score = 0;
        public bool isGameOver { get; private set; }


        private void Start()
        {
            currentActiveCharacter.GetComponent<PlayerData>().OnDeath += EndGame;
            pauseGame += PauseGame;

            money = 6000;

            // 모든 캐릭터를 비활성화
            foreach (GameObject character in characters)
            {
                character.SetActive(false);
            }

            // 첫 번째 캐릭터 활성화
            if (characters.Length > 0)
            {
                characters[currentCharacterIndex].SetActive(true);

                skillSlots = SkillManager.Instance.SkillSlotUIInitialize(currentCharacterMode);
                currentcharacterActiveSkillsUI_Slots = characters[currentCharacterIndex].GetComponent<PlayerData>().currentActivateSkillsUI_Slots;

                currentCharacterAnimator = characters[currentCharacterIndex].GetComponentInChildren<Animator>();

                SkillManager.Instance.activeSkillSlot = currentcharacterActiveSkillsUI_Slots;
            }
        }

        private void Update()
        {
            // 캐릭터 전환
            if (Input.GetKeyDown(KeyCode.G))
            {
                int newIndex = currentCharacterIndex - 1;
                if (newIndex < 0)
                {
                    newIndex = characters.Length - 1;  // 배열의 마지막 인덱스로 순환
                }
                SwitchCharacter(newIndex);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                int newIndex = currentCharacterIndex + 1;
                if (newIndex >= characters.Length)
                {
                    newIndex = 0;  // 배열의 첫 번째 인덱스로 순환
                }
                SwitchCharacter(newIndex);
            }

            // 현재 활성화된 캐릭터의 정보 받기
            currentActiveCharacter = GetActiveCharacter();
            currentCharacterMode = currentActiveCharacter.GetComponent<PlayerData>().characterMode;
            currentCameraPivot = currentActiveCharacter.GetComponent<PlayerController>().cinemachineCameraTarget;
            cinemachineVirtualCamera.Follow = currentCameraPivot.transform;
            currentActiveCharacter.GetComponent<PlayerData>().currentActivateSkillsUI.SetActive(true);

            // 새로운 캐릭터의 현재 활성화된 스킬 셋 가져오기
            currentcharacterActiveSkills = characters[currentCharacterIndex].GetComponent<PlayerData>().currentActiveSkills;

            if(Input.GetKeyDown(KeyCode.P))
            {
                PauseGame();
            }
        }

        public void AddScore(int scoreGain)
        {
            if(!isGameOver)
            {
                score += scoreGain;
                IngameHUD.Instance.UpdateScoreText(score);
            }
        }
        

        

        private void SwitchCharacter(int newIndex)
        {
            if (newIndex >= 0 && newIndex < characters.Length)
            {
                // 현재 캐릭터의 OnDeath 이벤트 연결 해제
                PlayerData currentHealth = characters[currentCharacterIndex].GetComponent<PlayerData>();
                if (currentHealth != null)
                {
                    currentHealth.OnDeath -= EndGame;

                    // 현재 캐릭터 상태 저장
                    currentHealth.SaveCharacterState();
                }

                // 현재 캐릭터를 비활성화
                characters[currentCharacterIndex].SetActive(false);
                var pastCharacterMode = characters[currentCharacterIndex].GetComponent<PlayerData>().characterMode;
                var pastSkillTreeUI = SkillManager.Instance.GetSkillTreeUI(pastCharacterMode);
                pastSkillTreeUI.SetActive(false);
                characters[currentCharacterIndex].GetComponent<PlayerData>().currentActivateSkillsUI.SetActive(false);

                // 인덱스 갱신
                currentCharacterIndex = newIndex;

                // 새로운 캐릭터를 활성화
                characters[currentCharacterIndex].SetActive(true);
                var nowCharacterMode = characters[currentCharacterIndex].GetComponent<PlayerData>().characterMode;
                var nowSkillTreeUI = SkillManager.Instance.GetSkillTreeUI(nowCharacterMode);
                nowSkillTreeUI.SetActive(true);
                characters[currentCharacterIndex].GetComponent<PlayerData>().currentActivateSkillsUI.SetActive(true);

                // 새로운 캐릭터의 애니메이터 가져오기
                currentCharacterAnimator = characters[currentCharacterIndex].GetComponentInChildren<Animator>();

                // 새로운 캐릭터의 스킬셋 가져오기
                skillSlots = SkillManager.Instance.SkillSlotUIInitialize(nowCharacterMode);
                SkillManager.Instance.selected_SkillDescription.text = SkillManager.Instance.Detect_SelectedSlot();

                // 새로운 캐릭터의 활성화 스킬창 가져오기
                currentcharacterActiveSkillsUI_Slots = characters[currentCharacterIndex].GetComponent<PlayerData>().currentActivateSkillsUI_Slots;
                SkillManager.Instance.activeSkillSlot = currentcharacterActiveSkillsUI_Slots;


                // 새로운 캐릭터의 OnDeath 이벤트 연결
                PlayerData newHealth = characters[currentCharacterIndex].GetComponent<PlayerData>();
                if (newHealth != null)
                {
                    newHealth.RestoreCharacterState();
                    newHealth.OnDeath += EndGame;
                }

                // 캐릭터 스위치가 일어나면 델리게이트에 구독되었던 함수 인보크
                OnCharacterSwitch?.Invoke(characters[currentCharacterIndex].GetComponent<CharacterBase>());

                // 캐릭터 전환 시, 좀비의 추적 업데이트
                UpdateZombieTargets();
            }
        }

        public GameObject GetActiveCharacter()
        {
            // characters 배열을 순회하며 활성화된 캐릭터를 찾습니다.
            foreach (GameObject character in characters)
            {
                if (character.activeSelf) // 캐릭터가 활성화되어 있는지 확인
                {
                    return character;
                }
            }
            return null; // 만약 활성화된 캐릭터가 없다면 null 반환
        }

        private void UpdateZombieTargets()
        {
            Enemy[] zombies = FindObjectsOfType<Enemy>();
            foreach (Enemy zombie in zombies)
            {
                zombie.ResetTarget(); // 각 좀비의 타겟을 초기화
            }
        }

        public void EndGame()
        {
            isGameOver = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            IngameHUD.Instance.GameOverScreen(true);
        }

        private void PauseGame()
        {
            isPause = true;
            Time.timeScale = 0f;
            UIManager.Instance.Show<PauseMenuUI>(UIList.PauseMenuUI);
        }


        public PlayerDataDTO CharacterDataOrganize()
        {
            var data_Asuna = characters[0].GetComponent<PlayerData>();
            var data_Kunoichi = characters[1].GetComponent<PlayerData>();

            PlayerDataDTO playerDataDTO = new PlayerDataDTO()
            {
                asuna_CurrentHealth = data_Asuna.currentHP,
                asuna_MaxHealth = data_Asuna.maxHP,
                asuna_Skill = data_Asuna.skillSlots,
                asuna_Position = data_Asuna.gameObject.transform.position,

                kunoichi_CurrentHealth = data_Kunoichi.currentHP,
                kunoichi_MaxHealth = data_Kunoichi.maxHP,
                kunoichi_Skill = data_Kunoichi.skillSlots,
                kunoichi_Position = data_Kunoichi.gameObject.transform.position,
            };

            return playerDataDTO;
        }
    }
}