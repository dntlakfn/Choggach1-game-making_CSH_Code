using Code.Scripts.Entities;
using PSB.Code.BattleCode.Entities;
using PSB_Lib.StatSystem;
using System;
using UnityEngine;
using Work.YIS.Code.Buffs;
using YIS.Code.Combat;
using YIS.Code.Defines;
using YIS.Code.Modules;

namespace CSH.Scripts.Buffs
{
    public class DefenseDownDebuff : Buff
    {
        [SerializeField] private StatSO defenseStat;

        protected override void ApplyBuff(float value, int duration)
        {
            var statModule = owner.GetModule<EntityStat>();
            StatSO _defenseStat = statModule.GetStat(defenseStat);

            Debug.Assert(_defenseStat != null, $"Defense Stat is defined In {owner.name}");

            statModule.AddModifier(_defenseStat, this, -value);
            PlayEffect();
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
            var statModule = owner.GetModule<EntityStat>();
            statModule.RemoveModifier(defenseStat, this);
            StopEffect();
        }
    }
}