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

        // 다이얼로그 UI 패널과 텍스트를 연결
        public GameObject dialogPanel; // 다이얼로그 패널 UI
        public TMP_Text npcName; // NPC 이름
        public TMP_Text dialogText; // 다이얼로그 메시지 텍스트


        // NPC 대사 리스트
        public int npcID; // NPC 고유 ID
        private NPCDialogueData npcDialogueData; // NPC 대사 데이터
        public List<string> npcDialogues = new List<string>();
        private int currentDialogueIndex = 0;
        private bool isInteracting = false;
        private bool isDialogueEnded = false;

        public QuestData loadedQuest; // NPC가 가지고 있는 퀘스트
        public bool hasQuest = false; // NPC가 퀘스트를 가지고 있는지 여부
        public bool okay_ToStart;
        public string defaultText;

        public int preRe_NPC;
        public int preRe_Qe;





        private void Start()
        {
            // 시작 시 다이얼로그 패널을 비활성화
            dialogPanel.SetActive(false);

            // NPC의 대사 데이터와 이름을 JSON에서 로드
            npcDialogueData = NPCDialogueLoader.LoadNPCDialogue(npcID);
            

            if (npcDialogueData != null)
            {
                npcDialogues = npcDialogueData.dialogues; // 대사 데이터 설정
            }


            // NPC가 퀘스트를 가지고 있는지 확인 
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
            Debug.Log("인터페이스 작동");

            if (!isInteracting)
            {
                Debug.Log("인터페이스 안의 if로 들어옴");
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

        // 대화 시작 시
        // 첫 대사 로딩
        private void StartDialogue()
        {
            InteractionUI.Instance.gameObject.SetActive(false);

            // NPC 카메라의 Priority를 높여 활성화
            npcCamera.Priority = 20;
            playerCamera.Priority = 10;

            isInteracting = true;
            dialogPanel.SetActive(true); // 다이얼로그 패널 활성화
            currentDialogueIndex = 0;
            npcName.text = NPCDatabase.NPCs[npcDialogueData.npcID];



            // Interaction_NPC 스크립트에서 NPC와 대화할 때 실행            
            if (!QuestManager.npctalked_byPlayer.ContainsKey(npcID))
            {
                // NPC와 처음 대화할 때 기록 저장
                // 이 부분은 newActiveQuest와 직접적 연관은 없지만
                // 현재 퀘스트에 국한되지 않고 전체로 보았을 때의 NPC id 기록용도로서 대화 이력 추적용도로 쓰임
                NPCTalkedData_byPlayer nPCTalkedData_byPlayer = new NPCTalkedData_byPlayer();
                nPCTalkedData_byPlayer.talkedSessionCount++;
                QuestManager.npctalked_byPlayer.Add(npcID, new NPCTalkedData_byPlayer());


            }

            // 대화 퀘스트와 관련이 있는지 확인
            // 이 부분의 문제점
            // id 2 의 퀘스트를 로딩하는데 
            // 우리의 퀘스트는 id 1에 있기 때문
            /*
            if (QuestManager.activeQuests.TryGetValue(loadedQuest.id, out ActiveQuest _activeQuest))
            {
                if (_activeQuest.nPCToTalk.ContainsKey(npcID))
                {
                    // 대화 퀘스트 목표 NPC라면 대화 완료 상태로 설정
                    Debug.Log("디버그 타겟 포인트 진입");
                    _activeQuest.nPCToTalk[npcID].readyToComplete = true;
                }
            }
            */
            // activeQuests 안에 본 캐릭터를 대화 퀘스트 타겟으로 하는 퀘스트가 존재한다면
            foreach(var activeQuest in QuestManager.activeQuests.Values)
            {
                if(activeQuest.nPCToTalk.TryGetValue(npcID, out NPCToTalk targetNPC))
                {
                    targetNPC.readyToComplete = true;
                }
            }

            Debug.Log(loadedQuest.id);


            // 따로 조건이 필요없이, 처음부터 NPC와 대화함으로서 전개되는 대화
            if (loadedQuest.prerequisites.talkedNPC_id == -1 && loadedQuest.prerequisites.completedQuestID == -1)
            {
                if (okay_ToStart)
                {
                    dialogText.text = npcDialogues[currentDialogueIndex]; // 퀘스트 관련 대사
                }
                else
                {
                    dialogText.text = defaultText; // 일반 대사
                    isDialogueEnded = true;
                }
            }           
            // 특정 조건을 만족시켜야 실행되는 대화
            // 먼저 누군가와 대화하고 와야하는 경우, 특정 퀘스트를 완료 후에 와야할 경우
            else if (QuestManager.npctalked_byPlayer.ContainsKey(loadedQuest.prerequisites.talkedNPC_id) || 
                     QuestManager.finishedQuests.Contains(loadedQuest.prerequisites.completedQuestID))
            {
                dialogText.text = npcDialogues[currentDialogueIndex]; // 퀘스트 관련 대사
            }
            else
            {
                // 퀘스트가 없거나 목표 NPC가 아닌 경우 일반 대사로 처리
                if(okay_ToStart)
                {
                    dialogText.text = npcDialogues[0];
                }
                else
                {
                    dialogText.text = defaultText; // 일반 대사
                    isDialogueEnded = true;
                }


            }
            


        }

        // 다음 대사 표시
        private void ShowNextDialogue()
        {
            currentDialogueIndex++;
            if (currentDialogueIndex < npcDialogues.Count)
            {
                dialogText.text = npcDialogues[currentDialogueIndex]; // 다음 대사 표시
            }
            else
            {
                EndDialogue();
            }
        }

        // 대화 종료 시
        private void EndDialogue()
        {
            InteractionUI.Instance.gameObject.SetActive(true);
            DialogueJustExit();

            // QuestInfoUI.Instance.questUIBackground_Panel.SetActive(true);
            QuestInfoUI.Instance.QuestInfoUI_ON();

            // 퀘스트가 있는 NPC만 퀘스트를 제공
            if (hasQuest)
            {
                QuestManager.Instance.ShowQuestInformation(loadedQuest);
                QuestManager.Instance.QuestInformation_UIDisplay(loadedQuest);
            }
        }

        private void DialogueJustExit()
        {
            // 대화 종료 후 다시 플레이어 카메라로 전환
            npcCamera.Priority = 10;
            playerCamera.Priority = 20;

            isInteracting = false;
            isDialogueEnded = false;
            dialogPanel.SetActive(false); // 다이얼로그 패널 비활성
            
        }
        
        

        // 2D 에서는 UI가 핵심


    }
}
