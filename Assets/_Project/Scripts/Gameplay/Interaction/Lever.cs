using System;
using System.Collections.Generic;
using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Interaction
{
    public class Lever : Interactable
    {
        [SerializeField] List<MonoBehaviour> targets = new List<MonoBehaviour>();
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Sprite offSprite;
        [SerializeField] Sprite onSprite;
        [SerializeField] bool oneShot = true;

        public event Action<Lever, bool> Toggled;

        public bool IsOn { get; private set; }

        public override bool CanInteract(GameObject interactor) => base.CanInteract(interactor) && !(oneShot && IsOn);

        public override void Interact(GameObject interactor)
        {
            IsOn = !IsOn;
            if (spriteRenderer != null) spriteRenderer.sprite = IsOn ? onSprite : offSprite;

            foreach (MonoBehaviour target in targets)
                if (target is ISwitchable switchable) switchable.SetOn(IsOn);

            Toggled?.Invoke(this, IsOn);
        }

        void OnValidate() => targets.RemoveAll(target => target != null && !(target is ISwitchable));
    }
}
