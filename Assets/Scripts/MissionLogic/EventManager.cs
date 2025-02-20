using System;
using System.Collections.Generic;

namespace HA
{
    public static class EventManager
    {
        private static Dictionary<string, Action<int>> eventDictionary = new Dictionary<string, Action<int>>();

        public static void StartListening(string eventName, Action<int> listener)
        {
            if (eventDictionary.TryGetValue(eventName, out Action<int> thisEvent))
            {
                thisEvent += listener;
                eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, Action<int> listener)
        {
            if (eventDictionary.TryGetValue(eventName, out Action<int> thisEvent))
            {
                thisEvent -= listener;
                eventDictionary[eventName] = thisEvent;
            }
        }

        public static void TriggerEvent(string eventName, int param)
        {
            if (eventDictionary.TryGetValue(eventName, out Action<int> thisEvent))
            {
                thisEvent.Invoke(param);
            }
        }
    }
}
