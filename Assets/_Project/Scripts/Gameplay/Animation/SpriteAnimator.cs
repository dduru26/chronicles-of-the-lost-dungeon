using System;
using UnityEngine;

namespace LostDungeon.Gameplay.Animation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] SpriteAnimationSet animationSet;
        [SerializeField] string startingClip = "Idle";
        [SerializeField, Min(0f)] float speed = 1f;
        [SerializeField] bool useUnscaledTime;

        SpriteRenderer spriteRenderer;
        SpriteAnimationClip current;
        float timer;
        int frame;
        bool finished;

        public event Action<string> ClipFinished;
        public event Action<string, int> FrameChanged;

        public string CurrentClipName => current?.Name;
        public int CurrentFrame => frame;
        public bool IsFinished => finished;
        public float Speed { get => speed; set => speed = Mathf.Max(0f, value); }

        public SpriteAnimationSet AnimationSet
        {
            get => animationSet;
            set { animationSet = value; current = null; }
        }

        SpriteRenderer Renderer => spriteRenderer != null ? spriteRenderer : spriteRenderer = GetComponent<SpriteRenderer>();

        void OnEnable()
        {
            if (current == null && !string.IsNullOrEmpty(startingClip)) Play(startingClip);
        }

        public bool HasClip(string clipName) => animationSet != null && animationSet.Find(clipName) != null;

        public bool Play(string clipName, bool restart = false)
        {
            if (animationSet == null) return false;
            if (!restart && current != null && string.Equals(current.Name, clipName, StringComparison.OrdinalIgnoreCase)) return true;

            SpriteAnimationClip clip = animationSet.Find(clipName);
            if (clip == null || clip.FrameCount == 0) return false;

            current = clip;
            timer = 0f;
            frame = 0;
            finished = false;
            ApplyFrame();
            return true;
        }

        void Update()
        {
            if (current == null || finished) return;

            timer += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * speed;
            float frameTime = 1f / current.FramesPerSecond;

            while (timer >= frameTime)
            {
                timer -= frameTime;
                frame++;

                if (frame >= current.FrameCount)
                {
                    if (current.Loop)
                    {
                        frame = 0;
                    }
                    else
                    {
                        frame = current.FrameCount - 1;
                        finished = true;
                        ApplyFrame();
                        ClipFinished?.Invoke(current.Name);
                        return;
                    }
                }

                ApplyFrame();
            }
        }

        void ApplyFrame()
        {
            Renderer.sprite = current.Frames[frame];
            FrameChanged?.Invoke(current.Name, frame);
        }
    }
}
