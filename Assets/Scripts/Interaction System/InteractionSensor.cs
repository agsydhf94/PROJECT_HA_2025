using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class InteractionSensor : MonoBehaviour
    {
        private IInteractable[] interactableObjects;
        public List<IInteractable> interactables = new List<IInteractable>();
        public bool HasInteractable => interactables.Count > 0;

        // ����Ƽ���� �����ϴ� Action ��������Ʈ
        public System.Action<IInteractable> OnDetected;
        public System.Action<IInteractable> OnLost;


        // ��ȣ�ۿ�
        private void OnEnable()
        {
            OnDetected += OnDetectedInteraction;
            OnLost += OnLostInteraction;
        }
 


        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.TryGetComponent(out IInteractable interactable))
            {
                interactables.Add(interactable);
                OnDetected?.Invoke(interactable);

                var interactionUI = UIManager.Instance.GetUI<InteractionUI>(UIList.InteractionUI);
                interactionUI.AddInteractionData(interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.root.TryGetComponent(out IInteractable interactable))
            {
                interactables.Remove(interactable);
                OnLost?.Invoke(interactable);
            }
        }



        private void OnLostInteraction(IInteractable interactable)
        {
            var interactionUI = UIManager.Instance.GetUI<InteractionUI>(UIList.InteractionUI);
            interactionUI.RemoveInteractionData(interactable);
        }

        private void OnDetectedInteraction(IInteractable interactable)
        {
            var interactionUI = UIManager.Instance.GetUI<InteractionUI>(UIList.InteractionUI);
            interactionUI.AddInteractionData(interactable);
        }


    }
}
