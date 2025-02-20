using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class Interaction_Door : MonoBehaviour, IInteractable
    {
        public Vector3 openRotation;
        public Vector3 closeRotation;

        public bool isOpen = false;

        public string Key => "Door." + gameObject.GetHashCode();

        public string Message => isOpen ? "Close" : "Open";

        public void Interact()
        {
            isOpen = !isOpen;
        }

        private void Update()
        {
            if (isOpen)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(openRotation), Time.deltaTime * 5f);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(closeRotation), Time.deltaTime * 5f);
            }
        }

    }
}
