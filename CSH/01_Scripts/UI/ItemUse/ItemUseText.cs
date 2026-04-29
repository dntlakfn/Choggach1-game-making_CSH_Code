using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YIS.Code.Items;

namespace CSH.Scripts.UIs
{
    public class ItemUseText : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private CanvasGroup canvasGroup;

        public void Initialize(Transform userTrm, ItemVisualDataSO item)
        {
            if (item == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            icon.sprite = item.icon;
            text.text = $"{item.uiName}";

            transform.position = Camera.main.WorldToScreenPoint(userTrm.position) + new Vector3(-20,100,0);
            transform.DOMoveY(transform.position.y + 5, 1f).SetEase(Ease.OutQuad);
            canvasGroup.DOFade(0, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Destroy(gameObject);

            });
        }
    }
}