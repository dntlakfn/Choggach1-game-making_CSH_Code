using CIW.Code.Player.Field;
using PSB.Code.BattleCode.Players;
using System;
using UnityEngine;

namespace Work.CSH.Code.Interacts
{
    public interface IInteractable
    {
        /// <summary>
        /// InteractPanel에 표시될 상호작용 오브젝트 이름.
        /// </summary>
        public string Name { get; set; }

        public Transform Transform { get; }

        ///<summary>
        /// 상호작용시 실행되는 함수.
        ///</summary>
        public void OnInteract();

    }

}