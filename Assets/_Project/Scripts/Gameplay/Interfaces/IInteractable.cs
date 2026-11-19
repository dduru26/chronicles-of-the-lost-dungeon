using UnityEngine;

namespace LostDungeon.Gameplay.Interfaces
{
    public interface IInteractable
    {
        string PromptText { get; }
        Transform PromptAnchor { get; }
        bool CanInteract(GameObject interactor);
        void Interact(GameObject interactor);
    }
}
