using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace HA
{
    public class CutsceneManager : MonoBehaviour
    {
        public PlayableDirector playableDirector;
        public TimelineAsset[] timelineAssets;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag.Equals("Character"))
            {
                playableDirector.Play(timelineAssets[0]);
            }
        }
    }

}
