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

            // ��� ĳ���͸� ��Ȱ��ȭ
            foreach (GameObject character in characters)
            {
                character.SetActive(false);
            }

            // ù ��° ĳ���� Ȱ��ȭ
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
            // ĳ���� ��ȯ
            if (Input.GetKeyDown(KeyCode.G))
            {
                int newIndex = currentCharacterIndex - 1;
                if (newIndex < 0)
                {
                    newIndex = characters.Length - 1;  // �迭�� ������ �ε����� ��ȯ
                }
                SwitchCharacter(newIndex);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                int newIndex = currentCharacterIndex + 1;
                if (newIndex >= characters.Length)
                {
                    newIndex = 0;  // �迭�� ù ��° �ε����� ��ȯ
                }
                SwitchCharacter(newIndex);
            }

            // ���� Ȱ��ȭ�� ĳ������ ���� �ޱ�
            currentActiveCharacter = GetActiveCharacter();
            currentCharacterMode = currentActiveCharacter.GetComponent<PlayerData>().characterMode;
            currentCameraPivot = currentActiveCharacter.GetComponent<PlayerController>().cinemachineCameraTarget;
            cinemachineVirtualCamera.Follow = currentCameraPivot.transform;
            currentActiveCharacter.GetComponent<PlayerData>().currentActivateSkillsUI.SetActive(true);

            // ���ο� ĳ������ ���� Ȱ��ȭ�� ��ų �� ��������
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
                // ���� ĳ������ OnDeath �̺�Ʈ ���� ����
                PlayerData currentHealth = characters[currentCharacterIndex].GetComponent<PlayerData>();
                if (currentHealth != null)
                {
                    currentHealth.OnDeath -= EndGame;

                    // ���� ĳ���� ���� ����
                    currentHealth.SaveCharacterState();
                }

                // ���� ĳ���͸� ��Ȱ��ȭ
                characters[currentCharacterIndex].SetActive(false);
                var pastCharacterMode = characters[currentCharacterIndex].GetComponent<PlayerData>().characterMode;
                var pastSkillTreeUI = SkillManager.Instance.GetSkillTreeUI(pastCharacterMode);
                pastSkillTreeUI.SetActive(false);
                characters[currentCharacterIndex].GetComponent<PlayerData>().currentActivateSkillsUI.SetActive(false);

                // �ε��� ����
                currentCharacterIndex = newIndex;

                // ���ο� ĳ���͸� Ȱ��ȭ
                characters[currentCharacterIndex].SetActive(true);
                var nowCharacterMode = characters[currentCharacterIndex].GetComponent<PlayerData>().characterMode;
                var nowSkillTreeUI = SkillManager.Instance.GetSkillTreeUI(nowCharacterMode);
                nowSkillTreeUI.SetActive(true);
                characters[currentCharacterIndex].GetComponent<PlayerData>().currentActivateSkillsUI.SetActive(true);

                // ���ο� ĳ������ �ִϸ����� ��������
                currentCharacterAnimator = characters[currentCharacterIndex].GetComponentInChildren<Animator>();

                // ���ο� ĳ������ ��ų�� ��������
                skillSlots = SkillManager.Instance.SkillSlotUIInitialize(nowCharacterMode);
                SkillManager.Instance.selected_SkillDescription.text = SkillManager.Instance.Detect_SelectedSlot();

                // ���ο� ĳ������ Ȱ��ȭ ��ųâ ��������
                currentcharacterActiveSkillsUI_Slots = characters[currentCharacterIndex].GetComponent<PlayerData>().currentActivateSkillsUI_Slots;
                SkillManager.Instance.activeSkillSlot = currentcharacterActiveSkillsUI_Slots;


                // ���ο� ĳ������ OnDeath �̺�Ʈ ����
                PlayerData newHealth = characters[currentCharacterIndex].GetComponent<PlayerData>();
                if (newHealth != null)
                {
                    newHealth.RestoreCharacterState();
                    newHealth.OnDeath += EndGame;
                }

                // ĳ���� ����ġ�� �Ͼ�� ��������Ʈ�� �����Ǿ��� �Լ� �κ�ũ
                OnCharacterSwitch?.Invoke(characters[currentCharacterIndex].GetComponent<CharacterBase>());

                // ĳ���� ��ȯ ��, ������ ���� ������Ʈ
                UpdateZombieTargets();
            }
        }

        public GameObject GetActiveCharacter()
        {
            // characters �迭�� ��ȸ�ϸ� Ȱ��ȭ�� ĳ���͸� ã���ϴ�.
            foreach (GameObject character in characters)
            {
                if (character.activeSelf) // ĳ���Ͱ� Ȱ��ȭ�Ǿ� �ִ��� Ȯ��
                {
                    return character;
                }
            }
            return null; // ���� Ȱ��ȭ�� ĳ���Ͱ� ���ٸ� null ��ȯ
        }

        private void UpdateZombieTargets()
        {
            Enemy[] zombies = FindObjectsOfType<Enemy>();
            foreach (Enemy zombie in zombies)
            {
                zombie.ResetTarget(); // �� ������ Ÿ���� �ʱ�ȭ
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