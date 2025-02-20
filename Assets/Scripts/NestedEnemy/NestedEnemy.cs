using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class NestedEnemy : MonoBehaviour
    {
        public EnemySegment[] nestedSegments;

        public Collider GetSpecificCollider(Collider collider)
        {
            foreach(var segment in nestedSegments)
            {
                if(collider.gameObject.name.Equals(segment.gameObject.name))
                {
                    return collider;
                }
            }
            return null;
        }
    }
}
