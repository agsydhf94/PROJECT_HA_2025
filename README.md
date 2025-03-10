영상 링크 : https://drive.google.com/drive/folders/1dBuJjGFpZouPWpuOlqhBzZwvrt40KOL4?usp=drive_link

<p align="center">
  <img src="https://github.com/user-attachments/assets/1269010e-a0aa-4295-bf52-8d8c48be4104" width="500" height="287"/>
</p>

---
# Hollow Point
## 장르 : 3인칭 슈팅 (TPS)
## 게임 소개 : 공중 기지에서 요원이 좀비 제거 미션을 받고 기지에 출몰한 좀비를 제거하고 지상에 워프하여 마지막 보스를 클리어하는 것이 목적인 게임입니다.
## 개발 목적 : 평소에 좋아했던 슈팅장르의 무기 및 효과와 보스전 연출 구현
## 사용 엔진 : Unity 2022.3.22f1
## 개발 기간 : 2024.06 ~ 2025.02

---

# 기술 개요

### 오브젝트 관리
> * Singleton 패턴
> * Queue를 이용한 ObjectPool

### 플레이어 UI
>  <details><summary>World Space 에서 플레이어를 따라다니는 UI</summary> <img src="https://github.com/user-attachments/assets/fd1665fb-ee92-4b11-af0c-28b40448ef55" width="500" height="287"/> </details>
> * 사격 시 UI 효과 - Post Processing 을 이용한 Motion Blur

### 플레이어 상호작용
> <details><summary>인터페이스를 이용한 상호작용</summary> <img src="https://github.com/user-attachments/assets/533b5602-9bf3-49bd-a138-8635a0fc4c29" width="500" height="287"/> </details>


### 무기 시스템
> * 클래스 상속을 이용한 무기 구현

### 인벤토리
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/Inventory/EquipmentSO.cs#L9))Scriptable Object로 장비와 아이템 정보 관리
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/Inventory/InventoryManager.cs#L124))장비 및 아이템 추가 로직
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/Inventory/EquipmentStrategy_Manager.cs#L6))Strategy 패턴을 이용한 인벤토리 장비 장착 <https://agsydhf94.tistory.com/1>
> * ([스크립트]())인벤토리 장비 장착 - TransformOffsetData 구조체 사용과 그 근거 <https://agsydhf94.tistory.com/2>


### 퀘스트
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/Quest/QuestData.cs#L11))퀘스트 시스템 - 퀘스트 정보 클래스의 구조 <https://agsydhf94.tistory.com/3>
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/Quest/QuestData.cs#L80))퀘스트 정보를 Json 으로부터 읽어오기
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/Quest/QuestData.cs#L80))퀘스트 진행도 추적
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/Quest/QuestManager.cs#L561))퀘스트 정보 UI에서 보상 아이템 목록 UI 동적 생성
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/Quest/ActiveQuestButton.cs#L11))퀘스트 북 안에서 보고싶은 퀘스트를 선택할 수 있는 활성 퀘스트 버튼 UI

### 스킬 트리
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/PlayerHealth/SkillSO.cs#L11))Scriptable Object로 스킬 정보 관리
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/PlayerHealth/SkillManager.cs#L171))RectTransform 조정으로 활성화된 스킬 UI를 플레이어 UI에 표시

### 보스전
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/BossBattle/BossEnemy.cs#L163))이진 검색 알고리즘으로 Cinemachine Dolly Track에서 Cart가 가장 최근에 지난 웨이포인트 가져오기
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/BossBattle/BossEnemy.cs#L442))보스 공격 직전 플레이어에게 경고성 레이저를 투사하고 레이저가 특정 레이어를 갖는 오브젝트에 닿았을 때의 지점과 Normal을 반영한 공격 구역 표시
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/BossBattle/EnemySegment.cs#L67))보스 분절(Segment) 가 피격되었을 때 DoTween를 이용하여 Material 색상 깜빡이기 및 충격으로 떨리는 효과구현
> * ([스크립트](https://github.com/agsydhf94/PROJECT_HA_2025/blob/master/Assets/Scripts/BossBattle/BossEnemy.cs#L392))보스 분절 파괴 시 MeshRenderer, Collider 비활성화 및 오브젝트 풀링에 의한 폭발 효과 로딩과 UniTask 비동기 처리에 의한 복귀

### 미사일 런처 무기
> * ([스크립트]())특정 클래스를 상속한 오브젝트의 콜라이더를 감지했을 때의 타겟팅 UI를 게임 화면상에 표시
> * ([스크립트]())정렬 알고리즘으로 조준원의 중앙에 가장 가까운 타겟을 감지
