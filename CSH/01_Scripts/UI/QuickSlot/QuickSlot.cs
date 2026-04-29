using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YIS.Code.Combat;
using YIS.Code.Items;

namespace CSH.Scripts.UIs
{
    public class QuickSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Component")]
        [SerializeField] private Button button;
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private ItemInfoBox itemInfoBox;

        [field: SerializeField] public ItemDataSO ItemData { get; private set; }
        [field: SerializeField] public int SlotIndex { get; private set; } = -1;
        public int Amount { get; private set; }

        public event Action<QuickSlot> OnButtonClick;

        private bool isBinded;

        public void Bind(int slotIndex, ItemDataSO data, int amount)
        {
            SlotIndex = slotIndex;
            ItemData = data;
            Amount = amount;

            if (ItemData == null || Amount <= 0)
            {
                SetEmpty();
                isBinded = false;
                return;
            }

            if (itemImage != null)
            {
                itemImage.enabled = true;
                itemImage.sprite = ItemData.visualData.icon;
            }

            if (amountText != null)
                amountText.SetText(Amount.ToString());
            isBinded = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ClickHandle);
            itemInfoBox.Initialize(ItemData.visualData.uiName, ItemData.visualData.itemDescription);
        }
        private void SetEmpty()
        {
            ItemData = null;
            Amount = 0;

            if (itemImage != null)
            {
                itemImage.sprite = null;
                itemImage.enabled = false;
            }

            if (amountText != null)
                amountText.SetText("");

        }

        public void ClickHandle()
        {
            OnButtonClick?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isBinded)
            {
                itemInfoBox.gameObject.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            itemInfoBox.gameObject.SetActive(false);
        }

        



    }

}
