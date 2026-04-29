using CSH.Scripts.Items;
using DG.Tweening;
using PSB.Code.BattleCode.UIs;
using PSB.Code.CoreSystem.Events;
using PSB.Code.CoreSystem.SaveSystem;
using PSW.Code.EventBus;
using System.Collections.Generic;
using UnityEngine;
using Work.CSH.Code.Enums;
using Work.CSH.Code.UIs;
using YIS.Code.Combat;
using YIS.Code.Events;
using YIS.Code.Items;

namespace CSH.Scripts.UIs
{
    public class QuickSlotRegister : MainUiPopCompo
    {
        [Header("References")]
        [SerializeField] private ItemDeleteUI deletePopup;
        [SerializeField] private QuickSlotCode inventory;
        [SerializeField] private Transform itemParentTrm;
        [SerializeField] private ItemMainPanelUI mainPanel;

        [Header("Prefab")]
        [SerializeField] private ItemPanelUI itemPanelPrefab;

        [Header("Animation")]
        private bool _isOpened;

        private readonly List<ItemPanelUI> _itemPanels = new();

        private ItemPanelUI _currentItem;
        private ItemUseContext useContext;

        private void Awake()
        {
            UiInit();
            Initialize();
        }

        private void Initialize()
        {

            _rect = transform as RectTransform;
            _canvasGroup = GetComponentInParent<CanvasGroup>();

            if (inventory == null)
                inventory = FindAnyObjectByType<QuickSlotCode>();

            Bus<SetItemContextEvent>.OnEvent += OnSetItemContext;
            Bus<QuickSlotInOutEvent>.OnEvent += QuickSlotInOutHandle;

            BuildSlots();
            RefreshFromInventory();
            SetImmediate(false);

        }

        private void OnEnable()
        {
            Bus<ItemGainedEvent>.OnEvent += OnItemGained;
        }

        private void OnDisable()
        {
            Bus<ItemGainedEvent>.OnEvent -= OnItemGained;
        }

        private void OnDestroy()
        {
            Bus<SetItemContextEvent>.OnEvent -= OnSetItemContext;
            Bus<QuickSlotInOutEvent>.OnEvent -= QuickSlotInOutHandle;
        }

        private void OnItemGained(ItemGainedEvent e) => RefreshFromInventory();
        private void OnSetItemContext(SetItemContextEvent evt) => useContext = evt.context;

        public void BuildSlots()
        {
            if (inventory == null || itemParentTrm == null || itemPanelPrefab == null)
                return;

            for (int i = itemParentTrm.childCount - 1; i >= 0; i--)
                Destroy(itemParentTrm.GetChild(i).gameObject);

            _itemPanels.Clear();

            inventory.EnsureSlotsInitialized();

            for (int i = 0; i < inventory.slotCount; i++)
            {
                
                ItemPanelUI panel = Instantiate(itemPanelPrefab, itemParentTrm);
                panel.OnButtonClick += HandleButtonClick;
                _itemPanels.Add(panel);
            }
        }
        

        public void QuickSlotInOutHandle(QuickSlotInOutEvent evt)
        {
            if(_currentItem == null)
            {
                if(evt.targetPanel == null) return;
                OnRegisterQuickSlotButton(evt.mainInventory, evt.targetPanel);
            }
            else
            {
                OnUnregisterQuickSlotButton(evt.mainInventory, _currentItem);
            }
        }
        public void OnRegisterQuickSlotButton(InventoryCode mainInventory, ItemPanelUI targetPanel)
        {
            int slotIndex = targetPanel.SlotIndex;
            var registerItem = mainInventory.inventorySlots[slotIndex].item;
            bool isFull = false;

            if (inventory.IsFull() && !inventory.ContainsItem(registerItem)) return;

            if (registerItem.useFunction == null)
            {
                GuidePanelHelper.Instance.ShowGuidePanel(GuideTextType.CantRegisterQuickSlot);
                return;
            }
            for(int i = 0; i < inventory.slotCount; i++)
            {
                if(inventory.inventorySlots[i].item == registerItem)
                {
                    if(inventory.GetAmountAtSlot(i) == 3)
                    {
                        isFull = true;
                        break;
                    }
                }
            }
            if(!isFull)
            {
                inventory.TryAddItem(registerItem);
                mainInventory.TryRemoveAtSlot(slotIndex, 1);
                RefreshFromInventory();
                Bus<ItemVisualDataEvent>.Raise(new ItemVisualDataEvent(mainInventory.inventorySlots[slotIndex].item?.visualData));
            }
            
        }

        public void OnUnregisterQuickSlotButton(InventoryCode mainInventory, ItemPanelUI targetPanel)
        {
            int slotIndex = targetPanel.SlotIndex;
            mainInventory.TryAddItem(inventory.inventorySlots[slotIndex].item);
            inventory.TryRemoveSlot(slotIndex);
            Bus<ItemVisualDataEvent>.Raise(new ItemVisualDataEvent(inventory.inventorySlots[slotIndex].item?.visualData));

            RefreshFromInventory();
        }


        public void RefreshFromInventory()
        {
            if (inventory == null) return;

            inventory.EnsureSlotsInitialized();
            var slots = inventory.inventorySlots;

            if (_itemPanels.Count != inventory.slotCount)
                BuildSlots();

            int count = slots.Length;

            for (int i = 0; i < count; i++)
            {
                var panel = _itemPanels[i];
                var stack = slots[i];

                ItemVisualDataSO visual = null;
                int amount = 0;

                if (stack.item != null && stack.amount > 0)
                {
                    visual = stack.item.visualData;
                    amount = stack.amount;
                }

                panel.Bind(i, visual, amount);
            }

            ItemPanelUI selectedPanel = null;
            if (_currentItem != null)
            {
                int idx = _currentItem.SlotIndex;
                if (idx >= 0 && idx < _itemPanels.Count)
                    selectedPanel = _itemPanels[idx];

                if (selectedPanel == null || selectedPanel.ItemData == null || selectedPanel.Amount <= 0)
                {
                    selectedPanel = null;
                    _currentItem = null;
                }
                else
                {
                    _currentItem = selectedPanel;
                }
            }

            for (int i = 0; i < _itemPanels.Count; i++)
            {
                _itemPanels[i].SetActiveItem(_itemPanels[i] == selectedPanel);
            }
        }

        private void HandleButtonClick(ItemPanelUI targetPanel)
        {
            if (_currentItem == targetPanel)
            {
                targetPanel.SetActiveItem(false);
                _currentItem = null;

                Bus<ItemVisualDataEvent>.Raise(new ItemVisualDataEvent(null));
                return;
            }

            mainPanel.ClearSelectSlots();
            foreach (ItemPanelUI panel in _itemPanels)
            {
                bool isSelected = (panel == targetPanel);
                panel.SetActiveItem(isSelected);
            }

            _currentItem = targetPanel;

            if (targetPanel.ItemData != null)
                Bus<ItemVisualDataEvent>.Raise(new ItemVisualDataEvent(targetPanel.ItemData));
        }

        private void SetImmediate(bool open)
        {
            _isOpened = open;

            //gameObject.SetActive(open);
            _canvasGroup.alpha = open ? 1f : 0f;
            _canvasGroup.interactable = open;
            _canvasGroup.blocksRaycasts = open;

            //_rect.anchoredPosition = open
            //    ? Vector2.zero
            //    : new Vector2(0f, -50f);
        }

        public void ClearSelectSlots()
        {
            foreach (ItemPanelUI panel in _itemPanels)
            {
                panel.SetActiveItem(false);
            }
            _currentItem = null;
        }

        public bool IsBlockingClose()
        {
            return deletePopup != null && deletePopup.IsInputEditing;
        }

        public void Toggle()
        {
            if (_isOpened)
            {
                PopDown();
                return;
            }

            if (deletePopup != null)
                deletePopup.Hide();

            RefreshFromInventory();
            PopUp();
        }

        public override void PopUp(TweenCallback endEnvet = null)
        {
            if (_isOpened || OnPopUpEvent.Invoke(weight, this) == false) return;
            _isOpened = true;

            base.PopUp(endEnvet);
            RefreshFromInventory();

        }

        public override void PopDown()
        {
            if (!_isOpened) return;
            _isOpened = false;

            base.PopDown();
        }

        public void ItemUse()
        {
            if (_currentItem == null)
                return;

            int slotIndex = _currentItem.SlotIndex;
            if (slotIndex < 0)
                return;

            int amount = inventory.GetAmountAtSlot(slotIndex);
            if (amount <= 0)
                return;

            bool useSuccess = TryApplyItemEffect(slotIndex);

            if (!useSuccess)
            {
                GuidePanelHelper.Instance.ShowGuidePanel(GuideTextType.CantUseItem);
                return;
            }

            bool removed = inventory.TryRemoveAtSlot(slotIndex, 1, compactAfter: true);
            if (!removed)
                return;

            //if (_currentItem != null)
            //    _currentItem.SetActiveItem(false);

            //_currentItem = null;
            RefreshFromInventory();
            Bus<ItemVisualDataEvent>.Raise(new ItemVisualDataEvent(inventory.inventorySlots[slotIndex].item?.visualData));

        }

        private bool TryApplyItemEffect(int slotIndex)
        {
            var stack = inventory.inventorySlots[slotIndex];
            if (stack.item == null)
                return false;

            if (stack.item.useFunction == null)
            {
                Debug.Log($"{stack.item.name} 아이탬은 사용 기능이 없어요");
                return false;
            }

            if (useContext.user == null)
            {
                Debug.Log("아이탬 사용자가 useContext에 안들어 왔어요");
                return false;
            }

            if (!stack.item.useFunction.Use(useContext))
                return false;

            Debug.Log($"{stack.item.name} 사용 성공");

            return true;
        }

        public void ItemDelete()
        {
            if (_currentItem == null)
                return;

            if (deletePopup == null)
                return;

            int slot = _currentItem.SlotIndex;
            int max = inventory.GetAmountAtSlot(slot);
            if (max <= 0)
                return;

            deletePopup.Show(
                visual: _currentItem.ItemData,
                slotIndex: slot,
                maxCount: max,
                onConfirm: ConfirmDelete
            );
        }



        private void ConfirmDelete(int slotIndex, int count)
        {
            bool ok = inventory.TryRemoveAtSlot(slotIndex, count, compactAfter: true);
            if (!ok) return;

            if (_currentItem != null)
                _currentItem.SetActiveItem(false);

            _currentItem = null;
            RefreshFromInventory();
            Bus<ItemVisualDataEvent>.Raise(new ItemVisualDataEvent(null));
        }
    }

    public struct QuickSlotInOutEvent : IEvent
    {
        public InventoryCode mainInventory;
        public ItemPanelUI targetPanel;
        public QuickSlotInOutEvent(InventoryCode mainInventory, ItemPanelUI targetPanel)
        {
            this.mainInventory = mainInventory;
            this.targetPanel = targetPanel;
        }
    }
}