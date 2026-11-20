using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Abilities
{
    public abstract class AbilityDefinition : ScriptableObject, IAbility
    {
        [SerializeField] string id;
        [SerializeField] string displayName;
        [SerializeField] Sprite icon;
        [SerializeField, Min(0f)] float cooldown = 1f;

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public float Cooldown => cooldown;

        public virtual bool CanActivate(IAbilityUser user) => user != null;
        public abstract void Activate(IAbilityUser user);
    }
}
