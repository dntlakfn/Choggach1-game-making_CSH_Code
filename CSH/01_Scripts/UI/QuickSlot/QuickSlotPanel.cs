using CSH.Scripts.Items;
using DG.Tweening;
using PSB.Code.BattleCode.Events;
using PSB.Code.CoreSystem.SaveSystem;
using PSW.Code.EventBus;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Work.CSH.Code.Enums;
using Work.CSH.Code.UIs;
using YIS.Code.Combat;
using YIS.Code.Events;
using YIS.Code.Items;

namespace CSH.Scripts.UIs
{
    public class QuickSlotPanel : MonoBehaviour
    {
        [SerializeField] private QuickSlotCode inventory;
        [SerializeField] private Transform items;
        [SerializeField] private EnemySelectUI enemySelectUI;
        [SerializeField] private ItemUseText itemUseTextPrefab;

        private QuickSlot[] _itemPanelArray;
        private bool isSelectMode;
        private ItemUseContext useContext { get; set; }

        private void Awake()
        {
            Bus<SetItemContextEvent>.OnEvent += OnSetItemContext;

        }
        private void Start()
        {
            if(inventory == null)
                inventory = FindAnyObjectByType<QuickSlotCode>();
            _itemPanelArray = items.GetComponentsInChildren<QuickSlot>();

            SetQuickSlots();
        }
        private void OnDestroy()
        {
            Bus<SetItemContextEvent>.OnEvent -= OnSetItemContext;
            for(int i = 0; i < items.childCount; i++)
            {
                _itemPanelArray[i].OnButtonClick -= OnClickHandle;
            }
        }

        public void SetQuickSlots()
        {
            for(int i = 0; i < items.childCount; i++)
            {
                var stack = inventory.inventorySlots[i];
                if(stack.item != null)
                {
                    _itemPanelArray[i].Bind(i, stack.item, stack.amount);
                    _itemPanelArray[i].OnButtonClick += OnClickHandle;
                }

            }
        }

        public void RefreshQuickSlots()
        {
            for (int i = 0; i < items.childCount; i++)
            {
                var panel = _itemPanelArray[i];
                var stack = inventory.inventorySlots[i];

                ItemDataSO item = null;
                int amount = 0;

                if (stack.item != null && stack.amount > 0)
                {
                    item = stack.item;
                    amount = stack.amount;
                }

                panel.Bind(i, item, amount);
            }
        }

        public void OnClickHandle(QuickSlot quickSlot)
        {
            if(quickSlot.ItemData == null) return;

            var useFunction = quickSlot.ItemData.useFunction;
            Debug.Log("UseItem");
            Debug.Assert(useFunction != null, "사용이 불가능한 아이템이 왜 퀵슬롯에 들어가 있느뇨...");

            if (useFunction.toTarget)
            {
                isSelectMode = true;
                enemySelectUI.gameObject.SetActive(true); 
                enemySelectUI.transform.position = quickSlot.transform.position;
                enemySelectUI.Initialize(useContext.user, ItemUse, quickSlot.SlotIndex);
                enemySelectUI._rectTrm.anchoredPosition = Vector2.zero;


            }
            else
            {
                ItemUse(quickSlot.SlotIndex);
            }
            RefreshQuickSlots();
        }

        public void ItemUse(int slotIndex)
        {

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

            // 여기다 아이템 사용 한거 플레이어 위에 텍스트 띄우는거 넣을거임 필요한거 : ItemVisualDataSO, 플레이어 위치
            ShowItemUseText(useContext.user.transform, inventory.inventorySlots[slotIndex].item.visualData);


            bool removed = inventory.TryRemoveAtSlot(slotIndex, 1, compactAfter: true);
            if (!removed)
                return;
            RefreshQuickSlots();

        }

        private void ShowItemUseText(Transform userTrm, ItemVisualDataSO item)
        {
            Instantiate(itemUseTextPrefab, transform).Initialize(userTrm, item);
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

        private void OnSetItemContext(SetItemContextEvent evt)
        {
            useContext = evt.context;
            enemySelectUI.UpdateTarget(useContext);
        }

    }
}
