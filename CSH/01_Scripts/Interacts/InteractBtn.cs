using DG.Tweening;
using PSB_Lib.ObjectPool.RunTime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Work.CSH.Code.Interacts
{

    public class InteractBtn : MonoBehaviour, IPoolable
    {
        [field:SerializeField] public PoolItemSO PoolItem { get; set; }

        [SerializeField] private Image background;
        [SerializeField] private Image flange;
        [SerializeField] private Image key;

        private Pool _pool;
        [SerializeField] private TextMeshProUGUI nameText;

        public void ResetItem()
        {

        }

        public void SetUpPool(Pool pool)
        {
            _pool = pool;
            ResetBtn();
        }

        public void PushBtn()
        {
            transform.SetParent(transform.parent.parent, false);
            _pool.Push(this);
        }

        public void SetText(string name) => nameText.text = name;

        private void ResetBtn()
        {

            background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);
            flange.color = new Color(flange.color.r, flange.color.g, flange.color.b, 0f);
            key.color = new Color(key.color.r, key.color.g, key.color.b, 0f);
            nameText.color = new Color(1f, 1f, 1f, 0f);
        }

        public void ShowBtn()
        {
            ResetBtn();

            background.DOColor(new Color(background.color.r, background.color.g, background.color.b, 0.86f), 0.2f);
            flange.DOColor(new Color(flange.color.r, flange.color.g, flange.color.b, 1f), 0.2f);
            key.DOColor(new Color(key.color.r, key.color.g, key.color.b, 1f), 0.2f);
            nameText.DOColor(new Color(1f, 1f, 1f, 1f), 0.2f);
        }

        public void HideBtn()
        {
            background.DOColor(new Color(background.color.r, background.color.g, background.color.b, 0f), 0.2f);
            flange.DOColor(new Color(flange.color.r, flange.color.g, flange.color.b, 0f), 0.2f);
            key.DOColor(new Color(key.color.r, key.color.g, key.color.b, 0f), 0.2f);
            nameText.DOColor(new Color(1f, 1f, 1f, 0f), 0.2f).OnComplete(() => PushBtn());
        }

    }
}