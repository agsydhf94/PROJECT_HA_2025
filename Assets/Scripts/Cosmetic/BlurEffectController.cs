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
            // Volume �������Ͽ��� DepthOfField ���� ��������
            if (volume.profile.TryGet(out depthOfField))
            {
                DisableBlur(); // �ʱ�ȭ �� �� ��Ȱ��ȭ
            }
        }

        // �� Ȱ��ȭ
        public void EnableBlur()
        {
            if (depthOfField != null)
            {
                depthOfField.active = true;
            }
        }

        // �� ��Ȱ��ȭ
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

