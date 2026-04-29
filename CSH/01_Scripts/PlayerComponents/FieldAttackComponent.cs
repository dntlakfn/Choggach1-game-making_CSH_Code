using System;
using CIW.Code.Entities;
using DG.Tweening;
using UnityEngine;
using Work.CSH.Code.Battle;
using Work.PSB.Code.CoreSystem;
using Work.PSB.Code.FieldCode;
using Work.PSB.Code.FieldCode.MiniGames.MonsterHunt;
using YIS.Code.Modules;

namespace Work.CSH.Code.PlayerComponents
{
    public class FieldAttackComponent : MonoBehaviour, IModule, IAttackModule, IAfterInitModule
    {
        [SerializeField] private GameObject testEffect;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private FieldStrikeFirstEffectFunctionSO strikeFirstEffectFunction;
        
        [SerializeField] private BattleEnterContextSO enterContext;
        
        private CameraEffectComponent _cameraComponent;

        private int _lastDirX = 1;
        private Collider2D _target;
        
        public ModuleOwner Owner { get; private set; }
        public FieldPlayer Player { get; private set; }
        public event Action OnAttackEnd;
        

        public void Initialize(ModuleOwner owner)
        {
            Owner = owner;
            Player = owner as FieldPlayer;
            _cameraComponent = owner.GetModule<CameraEffectComponent>();
        }

        public void AfterInitialize()
        {
        }

        private void Update()
        {
            
            if (Player.InputSO.Direction.x != 0)
            {
                _lastDirX = Player.InputSO.Direction.x < 0 ? -1 : 1;
            }
                
        }

        public void Attack(GameObject target = null) // �̰� �ٲܿ���
        {
            var effect = Instantiate(testEffect, transform.position + new Vector3(_lastDirX * 1.5f, 0.4f), Quaternion.Euler(0, 0, -60f));


            _target = Physics2D.OverlapBox((Vector2)transform.position + new Vector2(_lastDirX, -0.5f),
                new Vector2(1.3f, 0.5f), 0f, enemyLayer);

            if (_lastDirX < 0) effect.transform.rotation = Quaternion.Euler(0, 0, -150f);


            if (_target != null && _target.GetComponent<FieldMonsterData>() != null)
            {
                _target.GetComponent<FieldMonsterData>().OnCaptured();
                effect.transform.position = _target.transform.position + Vector3.up * 0.5f;
            }
            else if (_target != null)
            {
                effect.transform.position = _target.transform.position + Vector3.up * 0.5f;

                _cameraComponent.EnemyAttack();
                enterContext.Set(BattleEnterBy.Player);
                if(strikeFirstEffectFunction != null) 
                    strikeFirstEffectFunction.Register();
                _target.GetComponent<FieldEnemyData>().StartBattle();
            }
        }

        public void OnAttack()
        {
            Attack();
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(_lastDirX, -0.5f),
                new Vector2(1.3f, 0.5f));
        }
#endif
        
        
    }
}