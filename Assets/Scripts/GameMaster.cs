using System.Collections;
using System.Collections.Generic;
// using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace HA
{
    public class GameMaster : MonoBehaviour
    {
        public static GameMaster instance;
        // Static�� �󸶳� ���� �ν��Ͻ��� ����� ����
        // �� ó�� ����� �������� ���� �ν��Ͻ��� ���� ������ ������ �̾��� �������� �Ǵ� �Ӽ��� �ִ�
        // ��, ��� �ν��Ͻ��� �޸� ������ �Ҵ�� ���� �ϳ��� ��ü�� ����Ų��

        // �÷��̾� ĳ���� ������ ����
        // �÷��̾� ĳ������ ���� ��ġ ������ ���´�
        public GameObject playerCharacter;
        public GameObject start_Position;
        public GameObject character_Customization;

        // // 7/3
        public bool isGameOver;
        public CharacterBase playerCharacterbase;

        // ���� ��/������ ������ �޴´�
        public Scene current_Scene;

        public InventorySystem inventory;

        // UI ��ҵ� ����
        public bool Display_Settings = false;
        public UIController ui_Controller;
        public int level = 78;

        // �����, FX����(ȿ����)�� �ʱ� ����� ����
        public float audio_Level = 1.0f;
        public float fx_Level = 1.0f;


        private void Awake()
        {
            // �̱���
            if (instance == null)
            {
                instance = this;
                // 7/13
                Debug.Log(playerCharacterbase);
                // playerCharacterbase.onCharacterDead += GameOverCheck;
                

                // �κ��丮 �ý��� �ʱ�ȭ
                // 2024.06.29 MonoBehavior �� ��ӹ޴� Ŭ������ new Ű����� �ν��Ͻ�ȭ �� �� ����
                // instance.inventorySystem = new InventorySystem();
                // InventoryItem temp = new InventoryItem();

                instance.inventory = gameObject.GetComponent<InventorySystem>();

                // �׽�Ʈ ��Ƽ� ������ �׳� �����غ� ��(������ �ν��Ͻ�)
                /*
                InventoryItem temp = gameObject.GetComponent<InventoryItem>();

                temp.Category = ItemCategory.CLOTHING;
                temp.Name = "Testing";
                temp.Description = "Testing the item type";
                temp.Strength = 0.5f;
                temp.Weight = 0.2f;
                instance.inventory.AddItem(temp);

                Debug.Log(instance);
                Debug.Log(instance.inventory);
                Debug.Log(temp);
                */
                

            }

            

            else if (instance != this)
            {
                Destroy(this);
            }
            

            // �� ������ �ٸ� ������ �Ѿ �� ���� ������Ʈ�� �����ǵ��� �Ѵ�
            DontDestroyOnLoad(this);

        }
        

        // �ʱ�ȭ
        private void Start()
        {
            // �ҷ��� ���� UI ��Ʈ�ѷ� ������ ã��
            // ���� ������ UI ��Ʈ�ѷ� ���� ���� Ȯ��
            if (GameObject.FindGameObjectWithTag("UI") != null)
            {
                // �����ϸ� ������ ��´�
                instance.ui_Controller =
                    GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
            }


            // instance.ui_Controller.settingsCanvas.gameObject.SetActive(instance.Display_Settings);

        }

        public void GameOverCheck()
        {
            // to do : game over
            Debug.Log("enemy destroyed");
        }

        public void MasterVolume(float volume)
        {
            instance.audio_Level = volume;
            instance.GetComponent<AudioSource>().volume = instance.audio_Level;
        }

        public void StartGame()
        {
            // ������ �����ϴ� �Լ��̸�, �÷��̾ ĳ���͸� Ŀ���͸���¡�ϴ� ���� �ε��մϴ�
            // SceneManager.LoadScene(SceneName.CharacterCustomizaion);
        }


    }
}

