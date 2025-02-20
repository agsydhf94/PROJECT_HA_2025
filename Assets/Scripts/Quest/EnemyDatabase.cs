using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class EnemyDatabase : MonoBehaviour
    {
        public static Dictionary<int, string> enemyDatabase = new Dictionary<int, string>()
        {
            {0, "Zombie"}
        };
    }
}
