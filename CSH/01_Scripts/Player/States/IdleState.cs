using CIW.Code;
using CIW.Code.FSM;
using CIW.Code.Player.Field;
using UnityEngine;
using Work.CSH.Code.FSMSystem;

namespace Work.CSH.Code.Player.States
{
    public class IdleState : AbstraticFieldPlayerState
    {
        private readonly int IdleHash = Animator.StringToHash("IDLE");
        public IdleState(Entity entity, int animationHash) : base(entity, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _entityAnimator.SetParam(IdleHash, true);
            _mover.StopImmediately(true, true);
        }

        public override void Update()
        {
            base.Update();
            Vector2 dir = _player.InputSO.Direction;
            if (dir != Vector2.zero)
            {
                _entityAnimator.SetParam(IdleHash, false);

                _player.ChangeState(FieldPlayerStateEnum.MOVE);
            }

        }
    }
}