using CIW.Code;
using CIW.Code.FSM;
using CIW.Code.Player;
using Code.Scripts.Enemies.BT;
using UnityEngine;
using Work.CSH.Code.PlayerComponents;
using YIS.Code.Modules;

namespace Work.CSH.Code.Player.States
{
    public class AbstraticFieldPlayerState : EntityState
    {
        

        protected IMover _mover;
        protected FieldPlayer _player;

        public AbstraticFieldPlayerState(Entity agent, int animationHash) : base(agent, animationHash)
        {
            _player = agent as FieldPlayer;
            Debug.Assert(_player != null, $"{this} is not attached to player");
            _mover = agent.GetModule<IMover>();
            

        }
    }
}