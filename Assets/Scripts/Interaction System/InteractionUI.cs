using HA;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HA
{
    public class InteractionUI : UIBase
    {
        public static InteractionUI Instance { get; private set; } = null;

        public Transform root;
        public InteractionUI_ListItem itemPrefab;

        private List<InteractionUI_ListItem> createdItems = new List<InteractionUI_ListItem>();
        private int selectedIndex = 0;


        private void Awake()
        {
            Instance = this;
            itemPrefab.gameObject.SetActive(false);

        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void AddInteractionData(IInteractable interactableData)
        {
            var newListItem = Instantiate(itemPrefab, root);
            newListItem.gameObject.SetActive(true);

            newListItem.DataKey = interactableData.Key;
            newListItem.Message = interactableData.Message;
            newListItem.InteractionData = interactableData;
            //newListItem.IsSelected = false;

            if(createdItems.Count == 0)
            {
                newListItem.IsSelected = true;
            }
            else
            {
                newListItem.IsSelected = false;
            }

            if (createdItems.Find(x => x.key.Equals(newListItem.key)) != null)
            {
                Destroy(newListItem.gameObject);
            }
            else
            {
                createdItems.Add(newListItem);
            }

                
            
        }


        public void RemoveInteractionData(IInteractable interactableData)
        {
            Debug.Log("Remove InteractionData");
            var targetItem = createdItems.Find(x => x.DataKey.Equals(interactableData.Key));
            if (targetItem != null)
            {
                if(targetItem.IsSelected && createdItems.Count > 1)
                {
                    createdItems[createdItems.Count - 2].IsSelected = true;
                    selectedIndex--;
                }

                createdItems.Remove(targetItem);
                Destroy(targetItem.gameObject);
            }
        }

        public void SelectPrev()
        {
            if (createdItems.Count > 0)
            {
                if (selectedIndex > 0 && selectedIndex < createdItems.Count)
                {
                    createdItems[selectedIndex].IsSelected = false;
                }

                selectedIndex--;
                selectedIndex = Mathf.Clamp(selectedIndex, 0, createdItems.Count - 1);
                createdItems[selectedIndex].IsSelected = true;
            }

        }

        public void SelectNext()
        {
            if (createdItems.Count > 0)
            {
                if (selectedIndex >= 0 && selectedIndex < createdItems.Count)
                {
                    createdItems[selectedIndex].IsSelected = false;
                }


                selectedIndex++;
                selectedIndex = Mathf.Clamp(selectedIndex, 0, createdItems.Count - 1);
                createdItems[selectedIndex].IsSelected = true;
            }

        }


        public void DoInteract()
        {
            if (createdItems.Count > 0 && selectedIndex >= 0)
            {
                createdItems[selectedIndex].InteractionData.Interact();
            }
        }

    }
}
