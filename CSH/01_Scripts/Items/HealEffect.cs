using PSB.Code.BattleCode.Entities;
using PSB.Code.BattleCode.Events;
using PSW.Code.EventBus;
using UnityEngine;


namespace CSH.Scripts.Items
{
    [CreateAssetMenu(fileName = "HealEffect", menuName = "SO/ItemEffect/Heal")]
    public class HealEffect : ItemUseFunctionSO
    {
        [SerializeField] private HealMode healMode;
        [Min(0)][SerializeField] private float value;

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

            EntityHealth hp;
            if (toUser)
            {
                Bus<HealRequest>.Raise(new HealRequest(value, healMode));

            }
            if (toTarget)
            {
                hp = context.target.GetModule<EntityHealth>();
                Bus<EnemyHealRequest>.Raise(new EnemyHealRequest(hp, value, healMode));
            }
            return true;
        }
    }
}