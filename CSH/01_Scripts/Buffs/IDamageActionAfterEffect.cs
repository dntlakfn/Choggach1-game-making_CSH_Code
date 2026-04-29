using UnityEngine;
using YIS.Code.Events;

namespace CSH.Scripts.Buffs
{
    public interface IDamageActionAfterBuff
    {
        public void ApplyBuffAfterDamageAction(DamageActionAfterEvent evt);
    }
}