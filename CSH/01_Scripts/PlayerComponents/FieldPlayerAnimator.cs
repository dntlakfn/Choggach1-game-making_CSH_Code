using CIW.Code;
using PSB.Code.BattleCode.Entities;
using UnityEngine;
using YIS.Code.Modules;

namespace Work.CSH.Code.PlayerComponents
{
    public class FieldPlayerAnimator : MonoBehaviour, IModule
    {
        private Animator animator;
        private Entity _entity;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Initialize(Entity entity)
        {
            
        }
        
        public void Initialize(ModuleOwner owner)
        {
            _entity = owner as Entity;
        }

        public void SetParam(int hash, float value) => animator.SetFloat(hash, value);
        public void SetParam(int hash, int value) => animator.SetInteger(hash, value);
        public void SetParam(int hash, bool value) => animator.SetBool(hash, value);
        public void SetParam(int hash) => animator.SetTrigger(hash);

        
    }
}
