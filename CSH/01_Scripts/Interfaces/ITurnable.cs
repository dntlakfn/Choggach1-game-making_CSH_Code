using System.Collections.Generic;
using UnityEngine;
using Work.CSH.Code.Managers;

namespace Work.CSH.Code.Interfaces
{
    public interface ITurnable
    {
        public TurnManagerSO TurnManager { get; set; }

        /// <summary>
        /// �ڽ��� ���� ������ �� �����
        /// </summary>
        public void OnStartTurn(bool isPlayerTurn);

        /// <summary>
        /// �ڽ��� ���� ���� �� �����
        /// </summary>
        public void OnEndTurn(bool isPlayerTurn);
    }

}