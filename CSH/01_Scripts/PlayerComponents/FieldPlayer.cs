using CIW.Code;
using CIW.Code.FSM;
using CIW.Code.Player.Field;
using CSH.Scripts.Items;
using PSW.Code.EventBus;
using UnityEngine;
using Work.CSH.Code.FSMSystem;
using Work.CSH.Code.Interacts;
using Work.CSH.Code.Player.States;

namespace Work.CSH.Code.PlayerComponents
{
    public class FieldPlayer : Entity
    {
        [field : SerializeField] public PlayerFieldInputSO InputSO { get; private set; }
        [SerializeField] private StateListSO stateList;

        private EntityStateMachine _stateMachine;
        private PlayerFieldMovement _movement;
        private InteractComponent _interact;
        
        protected override void InitializeModules()
        {
            base.InitializeModules();
            _stateMachine = new EntityStateMachine(this, stateList.states);
            ChangeState(FieldPlayerStateEnum.IDLE);
            _movement = GetModule<PlayerFieldMovement>();
            _interact = GetModule<InteractComponent>();
        }
        protected override void AfterInitializeModules()
        {
            base.AfterInitializeModules();
            
        }

        protected override void Start()
        {
            base.Start();
            Bus<SetItemContextEvent>.Raise(new SetItemContextEvent() { context = { user = this, target = null, isField = true } });
        }

        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }
        
        private void OnEnable()
        {
            if (InputSO == null) return;
            InputSO.OnAttackPressed += HandleAttackPressed;
            InputSO.OnInteractPressed += HandleInteractPressed;
            InputSO.OnChangeSelectedIndexPressed += HandleIncreaseSelectedIndexPressed;
            

        }

        private void OnDisable()
        {
            if (InputSO == null) return;
            InputSO.OnAttackPressed -= HandleAttackPressed;
            InputSO.OnInteractPressed -= HandleInteractPressed;
            InputSO.OnChangeSelectedIndexPressed -= HandleIncreaseSelectedIndexPressed;
            
        }

        private void OnDestroy()
        {
            InputSO.OnInteractPressed -= HandleInteractPressed;
            InputSO.OnAttackPressed -= HandleAttackPressed;
            InputSO.OnChangeSelectedIndexPressed -= HandleIncreaseSelectedIndexPressed;
        }

        private void HandleAttackPressed()
        {
            ChangeState(FieldPlayerStateEnum.ATTACK);
        }
        private void HandleInteractPressed()
        {
            _interact.HandleInteractPressed();
        }
        private void HandleIncreaseSelectedIndexPressed(int idx)
        {
            _interact.HandleIncreaseSelectedIndexPressed(idx);
        }


        public void ChangeState(FieldPlayerStateEnum newState)
        {
            _stateMachine.ChangeState((int)newState);
        }

        

    }
}