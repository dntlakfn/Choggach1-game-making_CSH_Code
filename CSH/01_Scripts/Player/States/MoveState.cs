using CIW.Code;
using UnityEngine;

namespace Work.CSH.Code.Player.States
{
    public class MoveState : AbstraticFieldPlayerState
    {
        private readonly int MoveHash = Animator.StringToHash("MOVE");
        

        public MoveState(Entity agent, int animationHash) : base(agent, animationHash)
        {
        }

        public override void Update()
        {
            base.Update();
            Vector2 dir = _player.InputSO.Direction;
            if (dir.x < 0)
                _entityAnimator.FlipX(true);
            else if (dir.x > 0)
                _entityAnimator.FlipX(false);
            _mover.SetMovement(dir);
            
            if (dir == Vector2.zero)
            {
                _entityAnimator.SetParam(MoveHash, false);
                _player.ChangeState(FSMSystem.FieldPlayerStateEnum.IDLE);
            }
            else
            {
                _entityAnimator.SetParam(MoveHash, true);
                
            }
        }
    }
}