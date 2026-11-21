using System;
using System.Collections.Generic;
using LostDungeon.Core.Algorithms;
using LostDungeon.Core.Events;
using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] LayerMask interactableMask;
        [SerializeField, Min(0.1f)] float radius = 1.6f;
        [SerializeField] Vector2 searchOffset = new Vector2(0f, 1f);
        [SerializeField] Transform promptMarker;
        [SerializeField] Vector3 promptOffset = new Vector3(0f, 1.6f, 0f);

        readonly Collider2D[] hits = new Collider2D[12];
        readonly List<IInteractable> candidates = new List<IInteractable>();
        PlayerController controller;
        ContactFilter2D filter;

        public event Action<IInteractable> FocusChanged;

        public IInteractable Current { get; private set; }

        void Awake()
        {
            controller = GetComponent<PlayerController>();
            filter = new ContactFilter2D { useLayerMask = true, layerMask = interactableMask, useTriggers = true };
            if (promptMarker != null) promptMarker.SetParent(null, true);
        }

        void OnDestroy()
        {
            if (promptMarker != null) Destroy(promptMarker.gameObject);
        }

        void Update()
        {
            IInteractable next = controller.ControlsEnabled ? FindNearest() : null;
            if (next != Current)
            {
                Current = next;
                FocusChanged?.Invoke(Current);
            }

            UpdatePrompt();

            if (Current != null && controller.ControlsEnabled && controller.InputReader.Pressed(PlayerButton.Interact))
            {
                string label = Current.PromptText;
                Current.Interact(gameObject);
                GameEvents.RaiseInteracted(label);
            }
        }

        IInteractable FindNearest()
        {
            Vector2 origin = (Vector2)transform.position + searchOffset;
            int count = Physics2D.OverlapCircle(origin, radius, filter, hits);

            candidates.Clear();
            for (int i = 0; i < count; i++)
            {
                IInteractable interactable = hits[i].GetComponentInParent<IInteractable>();
                if (interactable != null && interactable.CanInteract(gameObject) && !candidates.Contains(interactable))
                    candidates.Add(interactable);
            }

            int index = NearestSearch.FindNearest(candidates, c => (c.PromptAnchor.position.x, c.PromptAnchor.position.y), origin.x, origin.y, radius * 2f);
            return index >= 0 ? candidates[index] : null;
        }

        void UpdatePrompt()
        {
            if (promptMarker == null) return;
            bool visible = Current != null;
            if (promptMarker.gameObject.activeSelf != visible) promptMarker.gameObject.SetActive(visible);
            if (!visible) return;

            float bob = Mathf.Sin(Time.time * 5f) * 0.08f;
            promptMarker.position = Current.PromptAnchor.position + promptOffset + Vector3.up * bob;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere((Vector2)transform.position + searchOffset, radius);
        }
    }
}
