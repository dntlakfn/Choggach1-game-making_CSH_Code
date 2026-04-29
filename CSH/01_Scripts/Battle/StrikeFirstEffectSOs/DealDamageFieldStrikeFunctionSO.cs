using CIW.Code;
using Code.Scripts.Enemies.BT;
using PSB.Code.BattleCode.Entities;
using PSB_Lib.Dependencies;
using PSB_Lib.ObjectPool.RunTime;
using PSW.Code.EventBus;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using Work.CSH.Code.Battle;
using Work.PSB.Code.CoreSystem.Sounds;
using YIS.Code.Combat;
using YIS.Code.Defines;
using YIS.Code.Effects;
using YIS.Code.Skills;

namespace CSH.Scripts.Battle.StrikeFirstEffectSOs
{
    public struct BattleVFXData
    {
        public PoolItemSO animatorEffect;  //이펙트 so
        public AnimParamSO effectParam;  //이펙트 파람 so
        public bool playEffectOnFlip;  //플립해서 플레이 하는 이펙트인가
        public SoundSO attackSound;  // 공격 시 사운드
    }

    [CreateAssetMenu(fileName = "DamageEffectSO", menuName = "SO/FieldStrikeFunction/DealDamage")]
    public class DealDamageFieldStrikeFunctionSO : FieldStrikeFirstEffectFunctionSO
    {
        [Inject] private PoolManagerMono poolManager;

        [Header("Damage Data")]
        public Elemental elemental;
        public float damage;
        [Header("VFX")]
        public PoolItemSO animatorEffect;  //이펙트 so
        public AnimParamSO effectParam;  //이펙트 파람 so
        public bool playEffectOnFlip;  //플립해서 플레이 하는 이펙트인가
        public SoundSO attackSound;  // 공격 시 사운드

        public override void ApplyEffect(Entity user, IReadOnlyList<Entity> targets)
        {
            EntityHealth hp;

            DamageData data = new DamageData()
            {
                ElementalType = elemental,
                Damage = damage
            };

            if (ToUser)
            {
                hp = user.GetModule<EntityHealth>();
                PlayEffectForTarget(user);
                hp.ApplyDamage(data);
            }

            if (ToTargets)
            {
                foreach (var target in targets)
                {
                    hp = target.GetModule<EntityHealth>();
    
                    PlayEffectForTarget(target);
                    hp.ApplyDamage(data);
                }
            }
        }

        private void PlayEffectForTarget(Entity target)
        {
            if(poolManager == null) poolManager = FindAnyObjectByType<PoolManagerMono>();

            if (target == null)
                return;

            BattleVFXData vfxData = new BattleVFXData 
            { 
                animatorEffect = animatorEffect,
                effectParam = effectParam,
                playEffectOnFlip = playEffectOnFlip,
                attackSound = attackSound,
            };

            if (vfxData.animatorEffect != null)
            {
                var p = poolManager.Pop<PoolAnimatorEffect>(vfxData.animatorEffect);
                var evt = p.GetComponentInChildren<EffectTrigger>();
                if (evt != null) evt.OnEndTrigger += () => p.DestroyObj();
                Vector3 effectPos = target.transform.position;
                Quaternion rot = vfxData.playEffectOnFlip
                    ? Quaternion.Euler(0f, 180f, 0f)
                    : Quaternion.identity;

                int hash = vfxData.effectParam != null ? vfxData.effectParam.paramHash : 0;
                p.PlayClipEffect(effectPos, rot, hash);

                if (vfxData.attackSound != null)
                {
                    PlaySFXEvent soundEvt = SoundEvents.PlaySFXEvent.
                        Initialize(target.transform.position, vfxData.attackSound);
                    Bus<PlaySFXEvent>.Raise(soundEvt);
                }
                else
                {
                    Debug.LogWarning("사운드 데이터가 없습니다.");
                }
            }
        }
    }
}
