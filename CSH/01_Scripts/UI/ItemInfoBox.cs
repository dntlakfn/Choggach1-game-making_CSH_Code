using TMPro;
using UnityEngine;

namespace CSH.Scripts.UIs
{
    public class ItemInfoBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _Description;


        public void Initialize(string name, string descripction)
        {
            _name.text = name;
            _Description.text = descripction;
        }
    }
}