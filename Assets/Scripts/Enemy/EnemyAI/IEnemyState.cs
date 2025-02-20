using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public interface IEnemyState
    {
        void EnterState(BossEnemy enemy);
        void UpdateState(BossEnemy enemy);
        void ExitState(BossEnemy enemy);
    }
}