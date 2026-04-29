using CIW.Code;
using DG.Tweening;
using UnityEngine;
using Work.CSH.Code.PlayerComponents;

namespace Work.CSH.Code.Player.States
{
    public class AttackState : AbstraticFieldPlayerState
    {
        private readonly int AttackHash = Animator.StringToHash("ATTACK");
        private readonly int AttackAnimStateHash = Animator.StringToHash("field_atk");
        private FieldAttackComponent _attackCompo;
        private Animator _currentAnimator;
        public AttackState(Entity agent, int animationHash) : base(agent, animationHash)
        {
            _currentAnimator = _entityAnimator.GetComponent<Animator>();
        }

        public override void Enter()
        {
            base.Enter();
            _mover.StopImmediately(true, true);

            _attackCompo = _player.GetModule(typeof(FieldAttackComponent)) as FieldAttackComponent;
            _entityAnimator.SetParam(AttackHash, true);

            _animatorTrigger.OnAttackTrigger += _attackCompo.OnAttack;
            _animatorTrigger.OnAnimationEndTrigger += () =>
            {
                _player.ChangeState(FSMSystem.FieldPlayerStateEnum.IDLE);
            };

            var stateInfo = _currentAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.shortNameHash == AttackAnimStateHash && stateInfo.normalizedTime > 0f)
            {
                _player.ChangeState(FSMSystem.FieldPlayerStateEnum.IDLE);
                return;
            }

            

            
            
            
        }
        public override void Update()
        {
            base.Update();
            _mover.StopImmediately(true, true);

        }

        public override void Exit()
        {
            base.Exit();
            _animatorTrigger.OnAttackTrigger -= _attackCompo.OnAttack;
            _animatorTrigger.OnAnimationEndTrigger -= () =>
            {
                _player.ChangeState(FSMSystem.FieldPlayerStateEnum.IDLE);
            };
        }
    }
}