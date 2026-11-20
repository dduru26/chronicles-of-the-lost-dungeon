using System;
using System.Collections.Generic;
using UnityEngine;

namespace LostDungeon.Gameplay.Animation
{
    [Serializable]
    public class SpriteAnimationClip
    {
        [SerializeField] string name;
        [SerializeField] Sprite[] frames;
        [SerializeField, Min(1f)] float framesPerSecond = 12f;
        [SerializeField] bool loop = true;

        public SpriteAnimationClip(string name, Sprite[] frames, float framesPerSecond, bool loop)
        {
            this.name = name;
            this.frames = frames;
            this.framesPerSecond = Mathf.Max(1f, framesPerSecond);
            this.loop = loop;
        }

        public string Name => name;
        public IReadOnlyList<Sprite> Frames => frames;
        public int FrameCount => frames?.Length ?? 0;
        public float FramesPerSecond => framesPerSecond;
        public bool Loop => loop;
        public float Duration => FrameCount / framesPerSecond;
    }
}
