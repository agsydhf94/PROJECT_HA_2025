using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HA
{
    public class CameraMoving : MonoBehaviour
    {
        public float scale;


        private void Update()
        {
            transform.rotation *= Quaternion.Euler(0, Time.deltaTime * scale, 0);
        }
    }
}
