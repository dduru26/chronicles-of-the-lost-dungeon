using System;
using System.Collections.Generic;
using UnityEngine;

namespace LostDungeon.Gameplay.Animation
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Animation/Sprite Animation Set", fileName = "AnimationSet")]
    public class SpriteAnimationSet : ScriptableObject
    {
        [SerializeField] List<SpriteAnimationClip> clips = new List<SpriteAnimationClip>();

        Dictionary<string, SpriteAnimationClip> lookup;

        public IReadOnlyList<SpriteAnimationClip> Clips => clips;

        public SpriteAnimationClip Find(string clipName)
        {
            if (string.IsNullOrEmpty(clipName)) return null;
            if (lookup == null) BuildLookup();
            return lookup.TryGetValue(clipName, out SpriteAnimationClip clip) ? clip : null;
        }

        public void SetClips(List<SpriteAnimationClip> newClips)
        {
            clips = newClips ?? new List<SpriteAnimationClip>();
            lookup = null;
        }

        void OnValidate() => lookup = null;

        void BuildLookup()
        {
            lookup = new Dictionary<string, SpriteAnimationClip>(StringComparer.OrdinalIgnoreCase);
            foreach (SpriteAnimationClip clip in clips)
                if (clip != null && !string.IsNullOrEmpty(clip.Name)) lookup[clip.Name] = clip;
        }
    }
}
