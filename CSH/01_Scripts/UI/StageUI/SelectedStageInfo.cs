using PSW.Code.EventBus;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

namespace Work.CSH.Code.StageUIs
{
    public class SelectedStageInfo : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI typeName;
        [SerializeField] private TextMeshProUGUI typeDescription;

        public void SetInfo(Sprite icon, string typeName, string typeDescription)
        {
            this.icon.sprite = icon;
            this.typeName.text = typeName;
            this.typeDescription.text = typeDescription;
        }
    }

    
}