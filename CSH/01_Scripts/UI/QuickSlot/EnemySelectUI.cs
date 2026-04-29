using CSH.Scripts.Items;
using PSB.Code.BattleCode.Enemies;
using PSB.Code.BattleCode.Events;
using PSB.Code.BattleCode.Players;
using PSW.Code.EventBus;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.UI.Extensions.Tweens;
using YIS.Code.Modules;

namespace CSH.Scripts.UIs
{
    public class EnemySelectUI : MonoBehaviour
    {
        public UnityEvent OnActive;
        public UnityEvent OnHide;
        private ModuleOwner player;
        public event Action<int> OnUse;
        public int slotIndex;

        [SerializeField] private float lineTilingMultiplier = 0.1f;
        [SerializeField] private int vertexCount = 30;

        public RectTransform _rectTrm { get; private set; }
        private NormalBattleEnemy _selectedEnemy;
        private bool isSelected;



        private void Awake()
        {
            _rectTrm = GetComponent<RectTransform>();
            //lineRenderer.positionCount = vertexCount;

        }
        public void Initialize(ModuleOwner player, Action<int> ItemUse, int slotIndex)
        {
            OnActive?.Invoke();
            this.player = player;
            OnUse = ItemUse;
            this.slotIndex = slotIndex;
            isSelected = false;
            Bus<SkillRangePreviewEvent>.Raise(new SkillRangePreviewEvent(1));

        }

        public void UpdateTarget(ItemUseContext useContext)
        {
            _selectedEnemy = useContext.target as NormalBattleEnemy;

        }

        private void OnDestroy()
        {
            OnUse = null;
        }

        private void Update()
        {

            if(Input.GetMouseButtonDown(1))
            {
                Hide();
                return;
            }

            if (!isSelected && Input.GetKeyDown(KeyCode.Space))
            {
                isSelected = true;
                Bus<SetItemContextEvent>.Raise(new SetItemContextEvent(new ItemUseContext(player, _selectedEnemy, false)));
                OnUse?.Invoke(slotIndex);
                Hide();
            }
        }

        public void Hide()
        {
            OnUse = null;
            OnHide?.Invoke();
            Bus<SkillRangePreviewEvent>.Raise(new SkillRangePreviewEvent(0));

            gameObject.SetActive(false);
        }





    }

    #region Events

    

    #endregion
}