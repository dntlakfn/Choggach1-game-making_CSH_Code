using PSB_Lib.Dependencies;
using PSB_Lib.ObjectPool.RunTime;
using PSW.Code.EventBus;
using System.Collections.Generic;
using UnityEngine;
using Work.CSH.Code.Enums;

namespace Work.CSH.Code.UIs
{
    public class GuidePanelHelper : MonoBehaviour
    {
        [Inject] private PoolManagerMono poolManager;
        [SerializeField] private PoolItemSO guidePanelPoolItem;
        private List<GuideTextType> guidePanelList = new List<GuideTextType>();
        public static GuidePanelHelper Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Bus<RemoveGuidePanelTypeInListEvent>.OnEvent += HandleRemoveGuidePanelTypeInListEvent;
        }

        private void HandleRemoveGuidePanelTypeInListEvent(RemoveGuidePanelTypeInListEvent evt)
        {
            guidePanelList.Remove(evt.Type);
        }

        public void ShowGuidePanel(GuideTextType type)
        {
            GuidePanel guidePanel;
            if (guidePanelList.Contains(type))
            {
                guidePanelList.Remove(type);
                guidePanelList.Add(type);
                Debug.Log("GuidePanelHelper: " + type + " is already in the list, moved to the end.");
            }
            else
            {
                guidePanelList.Add(type);
                Debug.Log("GuidePanelHelper: " + type + " added to the list.");
            }

            for (int i = transform.childCount - 1; i >= 0 ; i--)
            {
                Transform child = transform.GetChild(i);

                if (child.gameObject.activeSelf)
                    poolManager.Push(child.GetComponent<GuidePanel>());
            }

            for (int i = 0; i < guidePanelList.Count; i++)
            {
                guidePanel = poolManager.Pop<GuidePanel>(guidePanelPoolItem);
                guidePanel.transform.SetParent(transform);
                guidePanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                guidePanel.transform.localScale = Vector3.one;
                guidePanel.ShowPanel(guidePanelList[i]);
                Debug.Log("GuidePanelHelper: " + guidePanelList[i] + "Index : " + i);

            }
        }
    }

    #region Events

    public struct RemoveGuidePanelTypeInListEvent : IEvent
    {
        public GuideTextType Type;
        public RemoveGuidePanelTypeInListEvent(GuideTextType type)
        {
            Type = type;
        }
    }

    #endregion
}