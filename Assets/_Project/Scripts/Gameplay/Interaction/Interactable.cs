using LostDungeon.Gameplay.Interfaces;
using LostDungeon.Gameplay.Player;
using UnityEngine;

namespace LostDungeon.Gameplay.Interaction
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] string promptText = "Interact";
        [SerializeField] Transform promptAnchor;

        public string PromptText => promptText;
        public Transform PromptAnchor => promptAnchor != null ? promptAnchor : transform;

        public virtual bool CanInteract(GameObject interactor) => isActiveAndEnabled;
        public abstract void Interact(GameObject interactor);

        protected static PlayerInventory InventoryOf(GameObject interactor) => interactor != null ? interactor.GetComponent<PlayerInventory>() : null;
    }
}
