using DG.Tweening;
using PSB_Lib.Dependencies;
using PSB_Lib.ObjectPool.RunTime;
using PSW.Code.EventBus;
using UnityEngine;

namespace Work.CSH.Code.Interacts
{
    public class InteractPanel : MonoBehaviour
    {
        [Inject] private PoolManagerMono poolManager;
        private RectTransform rectTransform;
        [SerializeField] private PoolItemSO interactBtn;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Bus<ActiveInteractPanelEvent>.OnEvent += HandleActivePanel;
            Bus<SettingInteractBtnsEvent>.OnEvent += HandleSettingInteractBtns;
        }
        private void OnDestroy()
        {
            Bus<ActiveInteractPanelEvent>.OnEvent -= HandleActivePanel;
            Bus<SettingInteractBtnsEvent>.OnEvent -= HandleSettingInteractBtns;
        }
         
        public void HandleActivePanel(ActiveInteractPanelEvent evt)
        {
            if(evt.IsActive)
            {
                rectTransform.position = evt.Pos.position;
                rectTransform.anchoredPosition += new Vector2(0, 100);
                gameObject.SetActive(true);
            }
            else
            {
                var a = transform.GetComponentsInChildren<InteractBtn>();
                foreach(var btn in a)
                {
                    btn.HideBtn();
                    
                }
                DOVirtual.DelayedCall(0.2f, () => gameObject.SetActive(false));
                
            }
        }

        public void HandleSettingInteractBtns(SettingInteractBtnsEvent evt)
        {
            var btns = transform.GetComponentsInChildren<InteractBtn>();
            if(btns.Length != evt.ObjectNames.Length)
            {
                foreach (var btn in btns)
                {
                    
                    btn.PushBtn();

                }

                for (int i = 0; i < evt.ObjectNames.Length; i++)
                {
                    var btn = poolManager.Pop<InteractBtn>(interactBtn);
                    btn.transform.SetParent(transform, false);

                    btn.SetText(evt.ObjectNames[i]);
                    btn.ShowBtn();

                }
            }
            if(evt.ObjectNames.Length <= 1)
                return;
            for (int i = 0; i < evt.ObjectNames.Length; i++)
            {
                var btn = transform.GetChild(i).GetComponent<InteractBtn>();
                if (i == evt.SelectedIndex)
                    btn.transform.DOScale(Vector3.one * 0.6f, 0.1f);
                else
                    btn.transform.DOScale(Vector3.one * 0.4f, 0.1f);
            }

        }

    }
}