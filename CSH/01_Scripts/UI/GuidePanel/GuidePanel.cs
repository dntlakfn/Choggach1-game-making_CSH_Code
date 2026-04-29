using PSB_Lib.ObjectPool.RunTime;
using PSW.Code.EventBus;
using TMPro;
using UnityEngine;
using Work.CSH.Code.Enums;

namespace Work.CSH.Code.UIs
{
    public class GuidePanel : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshProUGUI guideText;
        [SerializeField] private GuideTextDataSOList guideTextDataList;

        [field:SerializeField] public PoolItemSO PoolItem { get; set; }
        private Pool _pool;
        private GuideTextType guideTextType;
        public void ShowPanel(GuideTextType type)
        {

            guideText.text = guideTextDataList.GetGuideText(type);
            guideTextType = type;

        }
        public void DestroyPanel()
        {
            Bus<RemoveGuidePanelTypeInListEvent>.Raise(new RemoveGuidePanelTypeInListEvent(guideTextType));
            _pool.Push(this);
        }

        public void SetUpPool(Pool pool)
        {
            _pool = pool;
        }

        public void ResetItem()
        {
            
        }
    }
}