using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class AnimationEventListener : MonoBehaviour
    {
        public System.Action<string> OnTakeAnimationEvent;

        public void OnAnimationEvent(string eventName)
        {
            OnTakeAnimationEvent?.Invoke(eventName);
        }
    }
}