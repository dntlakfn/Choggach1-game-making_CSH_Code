using PSW.Code.EventBus;
using UnityEngine;
using YIS.Code.Modules;

namespace CSH.Scripts.Items
{
    public abstract class ItemUseFunctionSO : ScriptableObject
    {
        [Header("Apply")]
        public bool toUser;
        public bool toTarget;

        public abstract bool Use(ItemUseContext context);
    }

    public struct ItemUseContext
    {
        public ModuleOwner user;
        public ModuleOwner target;
        public bool isField;

        public ItemUseContext(ModuleOwner user, ModuleOwner target, bool isField)
        {
            this.user = user;
            this.target = target;
            this.isField = isField;
        }
    }

    #region Events

    public struct SetItemContextEvent : IEvent
    {
        public ItemUseContext context;
        public SetItemContextEvent(ItemUseContext context)
        {
            this.context = context;
        }
    }

    #endregion
}