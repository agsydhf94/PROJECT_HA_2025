using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public interface IDamagable
    {
        public void Damage(float damage, Vector3 hitPoint, Vector3 hitNormal);
    }
}
