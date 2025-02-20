using HA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class CameraSystem : Singleton<CameraSystem>
    {

        public float TargetFOV { get; set; } = 60.0f;


        public Cinemachine.CinemachineVirtualCamera playerCamera;
        public Cinemachine.CinemachineImpulseSource impulseSource;

        public float zoomSpeed = 5.0f;

        public void ShakeCamera(Vector3 velocity, float duration, float force)
        {
            impulseSource.m_DefaultVelocity = velocity;
            impulseSource.m_ImpulseDefinition.m_ImpulseDuration = duration;
            impulseSource.GenerateImpulseWithForce(force);
        }


        private void LateUpdate()
        {
            playerCamera.m_Lens.FieldOfView = Mathf.Lerp(playerCamera.m_Lens.FieldOfView, TargetFOV, zoomSpeed * Time.deltaTime);
        }
    }
}
