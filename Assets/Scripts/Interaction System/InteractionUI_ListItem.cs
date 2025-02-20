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
            get => _isSelected; // �ʵ� ������ ��ȯ
            set
            {
                _isSelected = value; // �ʵ� ������ ���� ����
                selection.SetActive(value); // UI ����� Ȱ��ȭ ���¸� ����
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
