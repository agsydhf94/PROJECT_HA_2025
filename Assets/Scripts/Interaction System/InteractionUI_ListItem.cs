using UnityEngine;

namespace HA
{
    public class InteractionUI_ListItem : MonoBehaviour
    {
        
        public GameObject selection;
        public TMPro.TextMeshProUGUI text;

        private IInteractable interactionData;
        public IInteractable InteractionData
        {
            get => interactionData;
            set => interactionData = value;
        }
        public string key;

        public bool IsShowShortcut
        {
            set
            {
                selection.SetActive(value);
            }
        }

        public string DataKey
        {
            get => key;
            set => key = value;
        }

        public string Message
        {
            set => text.text = value;
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected; // 필드 변수를 반환
            set
            {
                _isSelected = value; // 필드 변수에 값을 저장
                selection.SetActive(value); // UI 요소의 활성화 상태를 설정
            }
        }

        

        /*
        public bool IsSelected
        {
            get => IsSelected;
            set => selection.SetActive(value);
        }
        */





    }
}
