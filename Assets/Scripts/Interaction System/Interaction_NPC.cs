using Cinemachine;
using HA;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

namespace HA
{
    public class Interaction_NPC : MonoBehaviour, IInteractable
    {
        public string Key => "NPC." + gameObject.GetHashCode();

        public string Message => "Talk";

        public CinemachineVirtualCamera playerCamera;
        public CinemachineVirtualCamera npcCamera;

        // ���̾�α� UI �гΰ� �ؽ�Ʈ�� ����
        public GameObject dialogPanel; // ���̾�α� �г� UI
        public TMP_Text npcName; // NPC �̸�
        public TMP_Text dialogText; // ���̾�α� �޽��� �ؽ�Ʈ


        // NPC ��� ����Ʈ
        public int npcID; // NPC ���� ID
        private NPCDialogueData npcDialogueData; // NPC ��� ������
        public List<string> npcDialogues = new List<string>();
        private int currentDialogueIndex = 0;
        private bool isInteracting = false;
        private bool isDialogueEnded = false;

        public QuestData loadedQuest; // NPC�� ������ �ִ� ����Ʈ
        public bool hasQuest = false; // NPC�� ����Ʈ�� ������ �ִ��� ����
        public bool okay_ToStart;
        public string defaultText;

        public int preRe_NPC;
        public int preRe_Qe;





        private void Start()
        {
            // ���� �� ���̾�α� �г��� ��Ȱ��ȭ
            dialogPanel.SetActive(false);

            // NPC�� ��� �����Ϳ� �̸��� JSON���� �ε�
            npcDialogueData = NPCDialogueLoader.LoadNPCDialogue(npcID);
            

            if (npcDialogueData != null)
            {
                npcDialogues = npcDialogueData.dialogues; // ��� ������ ����
            }


            // NPC�� ����Ʈ�� ������ �ִ��� Ȯ�� 
            if (npcDialogueData.questID != 0)
            {
                // loadedQuest = QuestManager.Instance.GetQuestByID(npcDialogueData.questID);
                loadedQuest = QuestLoader.LoadQuest(npcDialogueData.questID);

                if (loadedQuest != null)
                {
                    hasQuest = true;
                    okay_ToStart = npcDialogueData.okay_ToStart;
                    preRe_NPC = loadedQuest.prerequisites.talkedNPC_id;
                    preRe_Qe = loadedQuest.prerequisites.completedQuestID;
                }
            }
        }

        public void Interact()
        {
            Debug.Log("�������̽� �۵�");

            if (!isInteracting)
            {
                Debug.Log("�������̽� ���� if�� ����");
                StartDialogue();
            }
            else if(!isDialogueEnded)
            {
                ShowNextDialogue();
            }
            else
            {
                DialogueJustExit();
            }
        }

        // ��ȭ ���� ��
        // ù ��� �ε�
        private void StartDialogue()
        {
            InteractionUI.Instance.gameObject.SetActive(false);

            // NPC ī�޶��� Priority�� ���� Ȱ��ȭ
            npcCamera.Priority = 20;
            playerCamera.Priority = 10;

            isInteracting = true;
            dialogPanel.SetActive(true); // ���̾�α� �г� Ȱ��ȭ
            currentDialogueIndex = 0;
            npcName.text = NPCDatabase.NPCs[npcDialogueData.npcID];



            // Interaction_NPC ��ũ��Ʈ���� NPC�� ��ȭ�� �� ����            
            if (!QuestManager.npctalked_byPlayer.ContainsKey(npcID))
            {
                // NPC�� ó�� ��ȭ�� �� ��� ����
                // �� �κ��� newActiveQuest�� ������ ������ ������
                // ���� ����Ʈ�� ���ѵ��� �ʰ� ��ü�� ������ ���� NPC id ��Ͽ뵵�μ� ��ȭ �̷� �����뵵�� ����
                NPCTalkedData_byPlayer nPCTalkedData_byPlayer = new NPCTalkedData_byPlayer();
                nPCTalkedData_byPlayer.talkedSessionCount++;
                QuestManager.npctalked_byPlayer.Add(npcID, new NPCTalkedData_byPlayer());


            }

            // ��ȭ ����Ʈ�� ������ �ִ��� Ȯ��
            // �� �κ��� ������
            // id 2 �� ����Ʈ�� �ε��ϴµ� 
            // �츮�� ����Ʈ�� id 1�� �ֱ� ����
            /*
            if (QuestManager.activeQuests.TryGetValue(loadedQuest.id, out ActiveQuest _activeQuest))
            {
                if (_activeQuest.nPCToTalk.ContainsKey(npcID))
                {
                    // ��ȭ ����Ʈ ��ǥ NPC��� ��ȭ �Ϸ� ���·� ����
                    Debug.Log("����� Ÿ�� ����Ʈ ����");
                    _activeQuest.nPCToTalk[npcID].readyToComplete = true;
                }
            }
            */
            // activeQuests �ȿ� �� ĳ���͸� ��ȭ ����Ʈ Ÿ������ �ϴ� ����Ʈ�� �����Ѵٸ�
            foreach(var activeQuest in QuestManager.activeQuests.Values)
            {
                if(activeQuest.nPCToTalk.TryGetValue(npcID, out NPCToTalk targetNPC))
                {
                    targetNPC.readyToComplete = true;
                }
            }

            Debug.Log(loadedQuest.id);


            // ���� ������ �ʿ����, ó������ NPC�� ��ȭ�����μ� �����Ǵ� ��ȭ
            if (loadedQuest.prerequisites.talkedNPC_id == -1 && loadedQuest.prerequisites.completedQuestID == -1)
            {
                if (okay_ToStart)
                {
                    dialogText.text = npcDialogues[currentDialogueIndex]; // ����Ʈ ���� ���
                }
                else
                {
                    dialogText.text = defaultText; // �Ϲ� ���
                    isDialogueEnded = true;
                }
            }           
            // Ư�� ������ �������Ѿ� ����Ǵ� ��ȭ
            // ���� �������� ��ȭ�ϰ� �;��ϴ� ���, Ư�� ����Ʈ�� �Ϸ� �Ŀ� �;��� ���
            else if (QuestManager.npctalked_byPlayer.ContainsKey(loadedQuest.prerequisites.talkedNPC_id) || 
                     QuestManager.finishedQuests.Contains(loadedQuest.prerequisites.completedQuestID))
            {
                dialogText.text = npcDialogues[currentDialogueIndex]; // ����Ʈ ���� ���
            }
            else
            {
                // ����Ʈ�� ���ų� ��ǥ NPC�� �ƴ� ��� �Ϲ� ���� ó��
                if(okay_ToStart)
                {
                    dialogText.text = npcDialogues[0];
                }
                else
                {
                    dialogText.text = defaultText; // �Ϲ� ���
                    isDialogueEnded = true;
                }


            }
            


        }

        // ���� ��� ǥ��
        private void ShowNextDialogue()
        {
            currentDialogueIndex++;
            if (currentDialogueIndex < npcDialogues.Count)
            {
                dialogText.text = npcDialogues[currentDialogueIndex]; // ���� ��� ǥ��
            }
            else
            {
                EndDialogue();
            }
        }

        // ��ȭ ���� ��
        private void EndDialogue()
        {
            InteractionUI.Instance.gameObject.SetActive(true);
            DialogueJustExit();

            // QuestInfoUI.Instance.questUIBackground_Panel.SetActive(true);
            QuestInfoUI.Instance.QuestInfoUI_ON();

            // ����Ʈ�� �ִ� NPC�� ����Ʈ�� ����
            if (hasQuest)
            {
                QuestManager.Instance.ShowQuestInformation(loadedQuest);
                QuestManager.Instance.QuestInformation_UIDisplay(loadedQuest);
            }
        }

        private void DialogueJustExit()
        {
            // ��ȭ ���� �� �ٽ� �÷��̾� ī�޶�� ��ȯ
            npcCamera.Priority = 10;
            playerCamera.Priority = 20;

            isInteracting = false;
            isDialogueEnded = false;
            dialogPanel.SetActive(false); // ���̾�α� �г� ��Ȱ��
            
        }
        
        

        // 2D ������ UI�� �ٽ�


    }
}
