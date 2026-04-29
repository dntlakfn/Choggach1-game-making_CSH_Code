using System;
using PSB.Code.CoreSystem.Events;
using PSB.Code.CoreSystem.SaveSystem;
using PSB_Lib.Dependencies;
using PSW.Code.EventBus;
using System.Collections.Generic;
using PSB.Code.BattleCode.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using YIS.Code.Effects;
using YIS.Code.Events;
using YIS.Code.Items;

namespace CSH.Scripts.UIs
{
    public class QuickSlotCode : MonoBehaviour, ISaveable
    {
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private int maxStackSize = 64;

        public int slotCount { get; set; }
        public ItemStack[] inventorySlots = new ItemStack[3];

        [Inject] private IInventoryReader _reader;
        [Inject] private ISaveStore _store;

        [field: SerializeField] public SaveId SaveId { get; private set; }

        private void Awake()
        {
            EnsureSlotsInitialized();
        }

        private void OnValidate()
        {
            EnsureSlotsInitialized();
        }

        private void OnEnable()
        {
            Bus<VillageResetEvent>.OnEvent += HandleVillageReset;
        }

        private void OnDestroy()
        {
            Bus<VillageResetEvent>.OnEvent -= HandleVillageReset;
        }

        private void HandleVillageReset(VillageResetEvent evt)
        {
            ClearInventory();
        }
        

#if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.f6Key.wasPressedThisFrame)
            {
                var slots = _reader.GetInventoryAllSlots(SaveId.ID);

                for (int i = 0; i < slots.Length; i++)
                {
                    var (itemId, amount) = slots[i];
                    Debug.Log(amount > 0
                        ? $"���� {i}: itemId={itemId}, amount={amount}"
                        : $"���� {i}: �������");
                }
            }
        }
#endif

        public void EnsureSlotsInitialized()
        {
            if (inventorySlots == null || inventorySlots.Length != slotCount)
            {
                inventorySlots = new ItemStack[slotCount];
            }
        }

        #region Save

        public string GetSaveData()
        {
            EnsureSlotsInitialized();

            List<InvenData> collectionData = new List<InvenData>();
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                var stack = inventorySlots[i];
                if (stack.item != null && stack.amount > 0)
                {
                    collectionData.Add(new InvenData
                    {
                        slotNumber = i,
                        itemId = stack.item.itemId,
                        amount = stack.amount
                    });
                }
            }

            return JsonUtility.ToJson(new InvenCollection
            {
                slotCount = slotCount,
                data = collectionData
            });
        }

        public void RestoreSaveData(string saveData)
        {
            if (string.IsNullOrEmpty(saveData))
                return;

            InvenCollection loadedData = JsonUtility.FromJson<InvenCollection>(saveData);
            slotCount = loadedData.slotCount;
            if (slotCount == 0) slotCount = 3;

            if (inventorySlots == null || inventorySlots.Length != slotCount)
                inventorySlots = new ItemStack[slotCount];

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].item = null;
                inventorySlots[i].amount = 0;
            }

            foreach (InvenData item in loadedData.data)
            {
                if (item.slotNumber < 0 || item.slotNumber >= inventorySlots.Length)
                    continue;

                ItemDataSO itemData = itemDatabase.GetItemData(item.itemId);

                inventorySlots[item.slotNumber].item = itemData;
                inventorySlots[item.slotNumber].amount = item.amount;
            }
        }

        #endregion

        public bool IsFull()
        {
            EnsureSlotsInitialized();
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].item == null || inventorySlots[i].amount <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool ContainsItem(ItemDataSO item)
        {
            EnsureSlotsInitialized();
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].item == item && inventorySlots[i].amount > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryAddItem(ItemDataSO item)
        {
            if (item == null)
                return false;

            EnsureSlotsInitialized();

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].item == item &&
                    inventorySlots[i].amount < maxStackSize)
                {
                    inventorySlots[i].amount += 1;

                    Bus<ItemGainedEvent>.Raise(new ItemGainedEvent
                    {
                        Item = item,
                        Amount = 1
                    });

                    Bus<RequestSaveEvent>.Raise(new RequestSaveEvent());
                    return true;
                }
            }

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].item == null ||
                    inventorySlots[i].amount <= 0)
                {
                    inventorySlots[i].item = item;
                    inventorySlots[i].amount = 1;

                    Debug.Log($"[Inventory] {item.name} ��(��) ���� {i}�� �� �������� �߰� (1/{maxStackSize})");

                    Bus<ItemGainedEvent>.Raise(new ItemGainedEvent
                    {
                        Item = item,
                        Amount = 1
                    });

                    Bus<RequestSaveEvent>.Raise(new RequestSaveEvent());
                    return true;
                }
            }

            Debug.LogWarning($"[Inventory] �κ��丮�� ���� ���� {item.name} ��(��) �߰��� �� �����ϴ�.");
            return false;
        }

        public bool TryRemoveSlot(int slotIndex)
        {
            EnsureSlotsInitialized();

            if (slotIndex < 0 || slotIndex >= inventorySlots.Length)
                return false;

            var stack = inventorySlots[slotIndex];
            if (stack.item == null || stack.amount <= 0)
                return false;

            ItemDataSO removedItem = stack.item;

            stack.amount -= 1;
            if (stack.amount <= 0)
            {
                stack.amount = 0;
                stack.item = null;
            }

            inventorySlots[slotIndex] = stack;
            CompactSlots();

            Bus<BattleLootConsumedEvent>.Raise(new BattleLootConsumedEvent(removedItem, 1));
            Bus<RequestSaveEvent>.Raise(new RequestSaveEvent());
            return true;
        }

        public bool TryRemoveAtSlot(int slotIndex, int amount, bool compactAfter = true)
        {
            EnsureSlotsInitialized();

            if (slotIndex < 0 || slotIndex >= inventorySlots.Length)
                return false;

            if (amount <= 0)
                return true;

            var stack = inventorySlots[slotIndex];
            if (stack.item == null || stack.amount <= 0)
                return false;

            if (stack.amount < amount)
                return false;

            ItemDataSO removedItem = stack.item;

            stack.amount -= amount;
            if (stack.amount <= 0)
            {
                stack.amount = 0;
                stack.item = null;
            }

            inventorySlots[slotIndex] = stack;

            if (compactAfter)
                CompactSlots();

            Bus<BattleLootConsumedEvent>.Raise(new BattleLootConsumedEvent(removedItem, amount));
            Bus<RequestSaveEvent>.Raise(new RequestSaveEvent());
            return true;
        }

        public bool TryRemoveItem(ItemDataSO item, int amount, bool compactAfter = true)
        {
            EnsureSlotsInitialized();

            if (item == null || amount <= 0)
                return false;

            int remain = amount;
            bool removedAny = false;

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (remain <= 0)
                    break;

                var stack = inventorySlots[i];
                if (stack.item != item || stack.amount <= 0)
                    continue;

                int remove = Mathf.Min(stack.amount, remain);
                stack.amount -= remove;
                remain -= remove;
                removedAny = true;

                if (stack.amount <= 0)
                {
                    stack.amount = 0;
                    stack.item = null;
                }

                inventorySlots[i] = stack;
            }

            if (!removedAny)
                return false;

            if (compactAfter)
                CompactSlots();

            Bus<BattleLootConsumedEvent>.Raise(new BattleLootConsumedEvent(item, amount - remain));
            Bus<RequestSaveEvent>.Raise(new RequestSaveEvent());
            return true;
        }

        public int GetAmountAtSlot(int slotIndex)
        {
            EnsureSlotsInitialized();
            if (slotIndex < 0 || slotIndex >= inventorySlots.Length) return 0;

            var s = inventorySlots[slotIndex];
            if (s.item == null || s.amount <= 0) return 0;

            return s.amount;
        }

        public void ClearInventory()
        {
            EnsureSlotsInitialized();

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].item = null;
                inventorySlots[i].amount = 0;
            }

            _store.DeleteById(SaveId);

            Bus<RequestSaveEvent>.Raise(new RequestSaveEvent());
        }

        public void CompactSlots()
        {
            EnsureSlotsInitialized();

            int write = 0;

            for (int read = 0; read < inventorySlots.Length; read++)
            {
                var s = inventorySlots[read];

                bool hasItem = (s.item != null && s.amount > 0);
                if (!hasItem)
                    continue;

                if (write != read)
                {
                    inventorySlots[write] = s;
                    inventorySlots[read] = new ItemStack { item = null, amount = 0 };
                }

                write++;
            }
        }

    }
}