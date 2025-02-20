using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public interface IInteractable
    {
        public string Key { get; }
        public string Message { get; }

        public void Interact();
    }
}
