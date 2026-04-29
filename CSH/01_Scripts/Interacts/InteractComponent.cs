using CIW.Code.Player.Field;
using PSW.Code.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Work.CSH.Code.PlayerComponents;
using YIS.Code.Modules;

namespace Work.CSH.Code.Interacts
{
    public class InteractComponent : MonoBehaviour, IModule
    {
        [SerializeField] private float interactRadius = 1f;
        private IInteractable[] interactables;
        private int selectedIndex = 0;
        private bool isActivePanel = false;

        private FieldPlayer _player;
        
        
        public void Initialize(ModuleOwner owner)
        {
            _player = owner as FieldPlayer;
        }


        private void Update()
        {
            CheckInterctables();
        }

        private void CheckInterctables()
        {
            interactables = Physics2D.OverlapCircleAll(transform.position, interactRadius, LayerMask.GetMask("Interactable")).ToList().Select((x) => x.GetComponent<IInteractable>()).ToArray();
            if (interactables.Length <= 0)
            {
                if(isActivePanel)
                    Bus<ActiveInteractPanelEvent>.Raise(new ActiveInteractPanelEvent(false, transform));
                selectedIndex = 0;
                isActivePanel = false;
            }
            else
            {
                Bus<ActiveInteractPanelEvent>.Raise(new ActiveInteractPanelEvent(true, interactables[0]?.Transform));
                if (!isActivePanel)
                {
                    //Bus<ActiveInteractPanelEvent>.Raise(new ActiveInteractPanelEvent(true, interactables[0]?.Transform));

                }
                List<string> names = new List<string>();
                for(int i = 0; i < interactables.Length; i++)
                {
                    if (interactables[i] == null) continue;
                    names.Add(interactables[i].Name);
                }

                Bus<SettingInteractBtnsEvent>.Raise(new SettingInteractBtnsEvent(names.ToArray(), selectedIndex));
                isActivePanel = true;
            }
        }

        public void HandleInteractPressed()
        {
            if(interactables.Length <= 0) return;
            interactables[selectedIndex].OnInteract();
        }

        public void HandleIncreaseSelectedIndexPressed(int modifier)
        {
            if (!isActivePanel || interactables.Length <= 0) return;
            selectedIndex = Mathf.Clamp(selectedIndex - modifier, 0, interactables.Length-1);


        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRadius);
        }
#endif
    }

    #region Events
    public struct SettingInteractBtnsEvent : IEvent
    {
        public string[] ObjectNames;
        public int SelectedIndex;
        public SettingInteractBtnsEvent(string[] objectName, int selectIndex)
        {
            ObjectNames = objectName;
            SelectedIndex = selectIndex;
        }
    }

    

    public struct ActiveInteractPanelEvent : IEvent
    {
        public bool IsActive;
        public Transform Pos;
        public ActiveInteractPanelEvent(bool isActive, Transform pos)
        {
            IsActive = isActive;
            Pos = pos;
        }
    }

    public struct ActiveInteractSlot
    {
        
    }
    #endregion
}


