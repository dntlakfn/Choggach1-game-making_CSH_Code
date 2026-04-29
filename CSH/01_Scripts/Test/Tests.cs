using UnityEngine;
using UnityEngine.Serialization;
using Work.CSH.Code.Interfaces;
using Work.CSH.Code.Managers;

namespace Work.CSH.Code.Interfaces
{
    public class Tests : MonoBehaviour, ITurnable
    {
        [field: FormerlySerializedAs("<turnManager>k__BackingField")] [field:SerializeField] public TurnManagerSO TurnManager { get; set; }
        
        private void Start()
        {
            TurnManager.SetPlayerTurn();
        }

        private void Awake()
        {
            TurnManager.AddITurnableList(this);
        }

        private void OnDestroy()
        {
            TurnManager.RemoveITurnableList(this);
        }

        public void OnEndTurn(bool isPlayerTurn)
        {
            if (!isPlayerTurn) return;
            Debug.Log("End Player Turn");
        }

        public void OnStartTurn(bool isPlayerTurn)
        {
            if (!isPlayerTurn) return;
            Debug.Log("Start Player Turn");
        }
        
    }
}