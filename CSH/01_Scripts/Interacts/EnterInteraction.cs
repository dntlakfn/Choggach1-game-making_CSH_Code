using PSB.Code.BattleCode.Players;
using PSB.Code.CoreSystem.SaveSystem;
using UnityEngine;
using Work.PSB.Code.FieldCode.MapSaves;
using Work.Scripts.UI;

namespace Work.CSH.Code.Interacts
{
    public class EnterInteraction : MonoBehaviour, IInteractable
    {
        [field:SerializeField] private GameObject ui;
        [SerializeField] private StageUIDataSO stageDataSO;
        [field:SerializeField] public string Name { get; set; }

        public Transform Transform => transform;

        [SerializeField] private int mapCount;
        [SerializeField] private InventoryCode inven;

        private void Start()
        {
            ui.gameObject.SetActive(false);
        }

        public void OnInteract()
        {
            /*PlayerHealthSave.Reset();
            SceneSaveSystem.DeleteAllSaves();
            inven.ClearInventory();
            CurrencyContainer.ResetAll();
            stageDataSO.DeleteJson();*/
            
            ui.gameObject.SetActive(true);
        }
        
    }
}