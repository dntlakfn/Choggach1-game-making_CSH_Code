using CIW.Code;
using System.Collections.Generic;
using YIS.Code.Skills;
using YIS.Code.Skills.Interfaces;
using YIS.Code.Skills.Sequences;

namespace CSH.Code.Skills
{
    public class PoisionSkill : BaseSkill, IBuffOrDeBuffSkill
    {
        protected override IReadOnlyList<ISkillAction> ChainSkillGenerateAction(Entity user, IReadOnlyList<Entity> targets)
        {
            throw new System.NotImplementedException();
        }

        protected override IReadOnlyList<ISkillAction> NormalSkillGenerateAction(Entity user, IReadOnlyList<Entity> targets)
        {
            throw new System.NotImplementedException();
        }

        public void UseBuffOrDeBuffSkill()
        {
            throw new System.NotImplementedException();
        }
    }
}