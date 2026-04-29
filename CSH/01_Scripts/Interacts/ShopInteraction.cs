using UnityEngine;

namespace Work.CSH.Code.Interacts
{
    public class ShopInteraction : MonoBehaviour, IInteractable
    {
        [field:SerializeField] public string Name { get; set; }

        public Transform Transform => transform;

        public void OnInteract()
        {
            Debug.Log($"\"{Name}\" called.");
        }


    }
}
