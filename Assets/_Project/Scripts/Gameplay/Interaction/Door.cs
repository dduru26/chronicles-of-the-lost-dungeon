using System;
using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class Door : MonoBehaviour, ISwitchable
    {
        [SerializeField] Transform visual;
        [SerializeField, Min(0f)] float openHeight = 2f;
        [SerializeField, Min(0.01f)] float openDuration = 0.6f;
        [SerializeField] bool startsOpen;

        Collider2D blocker;
        Vector3 closedPosition;
        float progress;

        public event Action<Door, bool> StateChanged;

        public bool IsOn { get; private set; }

        void Awake()
        {
            blocker = GetComponent<Collider2D>();
            if (visual == null && transform.childCount > 0) visual = transform.GetChild(0);
            if (visual != null) closedPosition = visual.localPosition;
            if (startsOpen)
            {
                SetOn(true);
                progress = 1f;
            }
        }

        public void SetOn(bool on)
        {
            if (IsOn == on) return;
            IsOn = on;
            blocker.enabled = !on;
            StateChanged?.Invoke(this, on);
        }

        public void Toggle() => SetOn(!IsOn);

        void Update()
        {
            if (visual == null) return;
            progress = Mathf.MoveTowards(progress, IsOn ? 1f : 0f, Time.deltaTime / openDuration);
            visual.localPosition = closedPosition + Vector3.up * openHeight * progress;
        }
    }
}
