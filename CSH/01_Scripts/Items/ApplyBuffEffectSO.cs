using PSB.Code.BattleCode.Players;
using UnityEngine;
using Work.CSH.Code.Enums;
using Work.CSH.Code.UIs;
using Work.YIS.Code.Buffs;
using YIS.Code.Defines;
using YIS.Code.Modules;

namespace CSH.Scripts.Items
{
    [CreateAssetMenu(fileName = "ApplyBuffEffect", menuName = "SO/ItemEffect/ApplyBuff")]
    public class ApplyBuffEffectSO : ItemUseFunctionSO
    {
        public BuffType buffType;
        public float value;
        [Min(0)] public int duration;

        [SerializeField] private bool canUseField;
        [SerializeField] private bool canUseBattle;

        

        public override bool Use(ItemUseContext context)
        {
            if (!canUseField)
            {
                if (context.isField == true)
                    return false;
            }
            if (!canUseBattle)
            {
                if (context.isField == false)
                    return false;
            }
            if (!(toUser || toTarget)) return false;

            BuffModule buffModule;

            if(toUser)
            {
                buffModule = context.user.GetModule<BuffModule>();
                buffModule.BuffApply(buffType, value, duration);
            }

            if(toTarget)
            {
                buffModule = context.target.GetModule<BuffModule>();
                buffModule.BuffApply(buffType, value, duration);
            }


            return true;
        }
    }
}