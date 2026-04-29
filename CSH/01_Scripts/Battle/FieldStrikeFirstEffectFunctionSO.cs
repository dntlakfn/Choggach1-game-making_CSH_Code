using CIW.Code;
using System.Collections.Generic;
using UnityEngine;


namespace Work.CSH.Code.Battle
{
    public abstract class FieldStrikeFirstEffectFunctionSO : ScriptableObject
    {
        [Header("Apply")]
        public bool ToUser;
        public bool ToTargets;


        public void Register()
        {
            TurnBeforeExecutor.OnBattleEnter += ApplyEffect;
        }

        public abstract void ApplyEffect(Entity user, IReadOnlyList<Entity> targets);
    }
}