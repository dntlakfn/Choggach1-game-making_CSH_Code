using PSW.Code.EventBus;
using Unity.VisualScripting;
using UnityEngine;
using Work.PSB.Code.CoreSystem.Sounds;

namespace Work.CSH.Code.Maps
{
    public class BGMObject : MonoBehaviour
    {
        [SerializeField] private SoundSO sound;
        public void Start()
        {
            Bus<PlaySFXEvent>.Raise(new PlaySFXEvent {clip = sound,channel = 1});


        }
    }
}