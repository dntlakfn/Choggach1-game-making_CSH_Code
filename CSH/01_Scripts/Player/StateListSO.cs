using CIW.Code.FSM;
using UnityEngine;

namespace Work.CSH.Code.Player.States
{
    [CreateAssetMenu(fileName = "StateList", menuName = "SO/FSM/StateList")]
    public class StateListSO : ScriptableObject
    {
        public string enumName;
        public StateDataSO[] states;
    }
}
