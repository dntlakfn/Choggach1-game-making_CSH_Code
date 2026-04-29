using PSB_Lib.ObjectPool.RunTime;
using PSW.Code.EventBus;
using UnityEngine;
using UnityEngine.UI;

namespace Work.Scripts.UI
{
    public class StageUICloud : MonoBehaviour, IPoolable
    {
        [SerializeField] private Sprite[] cloudSprites;
        [SerializeField] private PoolItemSO poolItem;
        public PoolItemSO PoolItem => poolItem;

        private Pool _pool;
        private RectTransform _rectTrm;
        [SerializeField] private Image image;
        public int goalYPos;

        public void SetUpPool(Pool pool)
        {
            _pool = pool;
            _rectTrm = GetComponent<RectTransform>();
            image.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
        }

        private void Update()
        {
            _rectTrm.anchoredPosition += new Vector2(0, 1);
            if (_rectTrm.anchoredPosition.y - _rectTrm.sizeDelta.y >= goalYPos)
            {
                Bus<PushCloudAfterEvent>.Raise(new PushCloudAfterEvent());
                _pool.Push(this);
            }
        }

        public void ResetItem()
        {

        }

        public RectTransform GetRectTransform() => _rectTrm;
    }

    public struct PushCloudAfterEvent : IEvent
    {

    }
}