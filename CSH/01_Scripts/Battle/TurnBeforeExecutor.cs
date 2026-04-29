using CIW.Code;
using PSB.Code.BattleCode.Enemies;
using PSB.Code.BattleCode.Players;
using PSB_Lib.Dependencies;
using PSW.Code.EventBus;
using System;
using System.Collections.Generic;
using UnityEngine;
using Work.CSH.Code.Managers;
using Work.PSB.Code.CoreSystem;

namespace Work.CSH.Code.Battle
{
    public class TurnBeforeExecutor : MonoBehaviour
    {
        public static Action<Entity, IReadOnlyList<Entity>> OnBattleEnter;
        [Inject] private PlayerManager _playerManager;
        [Inject] private BattleEnemyManager _enemyManager;

        private void OnDestroy()
        {
            OnBattleEnter = null;
        }

        public bool Execute(BattleEnterBy battleEnterBy)
        {
            
            switch(battleEnterBy)
            {
                case BattleEnterBy.Player:
                    OnBattleEnter?.Invoke(_playerManager.BattlePlayer, _enemyManager.GetEnemies());
                    break;
                case BattleEnterBy.Enemy:
                    IReadOnlyList<Entity> playerList = new List<Entity> { _playerManager.BattlePlayer };
                    OnBattleEnter?.Invoke(_enemyManager.GetEnemies()[0], playerList);
                    break;
                default:
                    Debug.LogError("Unknown BattleEnterBy value.");
                    return false;
            }

            OnBattleEnter = null;
            return true;
        }
    }

    public struct RegisterTurnBeforeExecutorEvent : IEvent
    {
        public Action<Entity, IReadOnlyList<Entity>> Func;
        public RegisterTurnBeforeExecutorEvent(Action<Entity, IReadOnlyList<Entity>> func)
        {
            Func = func;
        }
    }
}