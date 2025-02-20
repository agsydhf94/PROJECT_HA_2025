using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class CubeScaler : MonoBehaviour
    {
        public GameObject Ladder;

        float mfX;
        float mfY;
        float mfZ;
        // Use this for initialization
        void Start()
        {
            mfX = Ladder.transform.position.x - Ladder.transform.localScale.x / 2.0f;
            mfY = Ladder.transform.position.y - Ladder.transform.localScale.y / 2.0f;
            mfZ = Ladder.transform.position.z - Ladder.transform.localScale.z / 2.0f;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 v3Scale = Ladder.transform.localScale;
            Ladder.transform.localScale = new Vector3(1, v3Scale.y + 0.1f, 1);
            Ladder.transform.position = new Vector3(/*mfX + Ladder.transform.localScale.x / 2.0f*/0, mfY + Ladder.transform.localScale.y / 2.0f, mfZ + Ladder.transform.localScale.z / 2.0f);
        }
    }
}
