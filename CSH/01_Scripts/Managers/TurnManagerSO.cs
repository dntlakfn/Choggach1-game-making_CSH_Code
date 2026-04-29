using PSB.Code.BattleCode.Enemies;
using System;
using System.Collections.Generic;
using UnityEngine;
using Work.CSH.Code.Interfaces;


namespace Work.CSH.Code.Managers
{
    [CreateAssetMenu(menuName = "SO/TurnManager")]
    public class TurnManagerSO : ScriptableObject
    {
        private bool turn;
        public bool Turn => turn;
        //{
        //    get
        //    {
        //        return turn;
        //    }
        //    set
        //    {
        //        OnTurnEnded?.Invoke(!turn);
        //        turn = value;
        //        OnTurnStarted?.Invoke(turn);
        //    }
        //}

        public Action<bool> OnTurnStarted;
        public Action OnTurnDelayed;
        public Action<bool> OnTurnEnded;

        public Func<bool> OnBattleEndedCondition;

        private List<ITurnable> turnables = new List<ITurnable>();

        private void OnDestroy()
        {
            OnTurnStarted = null;
            OnTurnEnded = null;
            OnTurnDelayed = null;
            OnBattleEndedCondition = null;
        }

        public bool SetPlayerTurn()
        {
            if (OnBattleEndedCondition != null && OnBattleEndedCondition.Invoke())
                return turn;
            turn = true;
            OnTurnStarted?.Invoke(true);
            return turn;
        }

        public bool SetEnemyTurn()
        {
            if (OnBattleEndedCondition != null && OnBattleEndedCondition.Invoke())
                return turn;
            turn = false;
            OnTurnStarted?.Invoke(false);
            return turn;
        }

        public void AddITurnableList(ITurnable turnable)
        {
            turnables.Add(turnable);
            OnTurnStarted += turnable.OnStartTurn;
            OnTurnEnded += turnable.OnEndTurn;
        }
        
        public void RemoveITurnableList(ITurnable turnable)
        {
            turnables.Remove(turnable);
            OnTurnStarted -= turnable.OnStartTurn;
            OnTurnEnded -= turnable.OnEndTurn;
        }

        /// <summary>
        /// �� �ٲ��ִ� �Լ�
        /// </summary>
        public void NextTurn()
        {
            if (OnBattleEndedCondition != null && OnBattleEndedCondition.Invoke())
                return;

            Debug.Log("어 들어왔다");
            OnTurnEnded?.Invoke(turn);

            if (OnTurnDelayed != null)
                OnTurnDelayed.Invoke();
            else
                ChangeTurnActual();

            //Turn = !Turn;
        }

        public void ChangeTurnActual()
        {
            turn = !turn;
            OnTurnStarted?.Invoke(turn);
        }
    }
}