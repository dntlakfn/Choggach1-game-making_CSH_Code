using Code.Scripts.Entities;
using PSB.Code.BattleCode.Entities;
using PSB.Code.BattleCode.Events;
using PSB.Code.BattleCode.Players;
using PSB_Lib.StatSystem;
using PSW.Code.EventBus;
using System;
using UnityEngine;
using YIS.Code.Events;
using YIS.Code.Modules;

namespace CSH.Scripts.Buffs
{

    public class BloodsuckingBuff : Buff, IDamageActionAfterBuff
    {
        private float v;
        protected override void ApplyBuff(float value, int duration)
        {
            PlayEffect();
            v = value;
            Bus<DamageActionAfterEvent>.OnEvent += ApplyBuffAfterDamageAction;
        }

        public void ApplyBuffAfterDamageAction(DamageActionAfterEvent evt)
        {
            if (owner == evt.Target) return;
            

            
            if (owner.GetComponent<BattlePlayer>())
            {
                Bus<HealRequest>.Raise(new HealRequest((int)(evt.DamageData.Damage * (v / 100)), HealMode.Flat));

            }
            else
            {
                Bus<EnemyHealRequest>.Raise(new EnemyHealRequest(owner.GetModule<EntityHealth>(), (int)(evt.DamageData.Damage * (v / 100)), HealMode.Flat));
            }

        }

        public override void UpdateBuffTime()
        {
            base.UpdateBuffTime();
        }

        protected override void PlayEffect()
        {
            base.PlayEffect();
        }

        protected override void StopEffect()
        {
            base.StopEffect();
        }

        protected override void RemoveBuff()
        {
            StopEffect();
        }

        
    }
}