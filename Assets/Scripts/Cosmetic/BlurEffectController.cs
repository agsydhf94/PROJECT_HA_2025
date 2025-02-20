using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HA
{
public class BlurEffectController : MonoBehaviour
{
        public Volume volume;
        private DepthOfField depthOfField;
        
        /*
        private void Start()
        {
            // Volume 프로파일에서 DepthOfField 설정 가져오기
            if (volume.profile.TryGet(out depthOfField))
            {
                DisableBlur(); // 초기화 시 블러 비활성화
            }
        }

        // 블러 활성화
        public void EnableBlur()
        {
            if (depthOfField != null)
            {
                depthOfField.active = true;
            }
        }

        // 블러 비활성화
        public void DisableBlur()
        {
            if (depthOfField != null)
            {
                depthOfField.active = false;
            }
        }
        */
    }

}

