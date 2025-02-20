using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class NPCDatabase : MonoBehaviour
    {
        public static Dictionary<int, string> NPCs = new Dictionary<int, string>()
    {
        {1, "Zouk"},
        {2, "Commander"},
    };

    }
}
