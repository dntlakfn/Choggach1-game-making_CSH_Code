using PSB.Code.BattleCode.Entities;
using UnityEngine;
using YIS.Code.Combat;
using YIS.Code.Defines;

namespace CSH.Scripts.Items
{
    [CreateAssetMenu(fileName = "DealDamageEffect", menuName = "SO/ItemEffect/DealDamage")]
    public class DealDamageEffectSO : ItemUseFunctionSO
    {
        [SerializeField] private Elemental elemental;
        [Min(0)][SerializeField] private float damage;

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
            if(toUser)
            {
                hp = context.user.GetModule<EntityHealth>();
                DamageData data = new DamageData()
                {
                    ElementalType = elemental,
                    Damage = damage
                };

                hp.ApplyDamage(data);
            }
            if(toTarget)
            {
                hp = context.target.GetModule<EntityHealth>();
                DamageData data = new DamageData()
                {
                    ElementalType = elemental,
                    Damage = damage
                };

                hp.ApplyDamage(data);
            }
            return true;
        }
    }
}