using PSB.Code.BattleCode.Entities;
using PSB.Code.CoreSystem.SaveSystem;
using UnityEngine;
using Work.YIS.Code.Buffs;
using YIS.Code.Defines;
using YIS.Code.Modules;


namespace CSH.Scripts.Items
{
    [CreateAssetMenu(fileName = "OldCoinEffect", menuName = "SO/ItemEffect/OldCoin")]
    public class OldCoinEffect : ItemUseFunctionSO
    {
        [Range(1, 100)][SerializeField] private int percent;

        [SerializeField] private bool canUseField;
        [SerializeField] private bool canUseBattle;

        

        [Header("설명 (수정 ㄴㄴ)")]
        [TextArea] public string _ = "사용 시 1턴 동안 자신에게 현재 가진 코인의 {percent}% 만큼 공격력 증가";
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
            if (toUser)
            {
                buffModule = context.user.GetModule<BuffModule>();
                buffModule.BuffApply(BuffType.ATTACK_BUFF, CurrencyContainer.Get(ItemType.Coin) * (percent / 100), 1);
            }
            if (toTarget)
            {
                buffModule = context.target.GetModule<BuffModule>();
                buffModule.BuffApply(BuffType.ATTACK_BUFF, CurrencyContainer.Get(ItemType.Coin) * (percent / 100), 1);
            }
            return true;
        }
    }
}