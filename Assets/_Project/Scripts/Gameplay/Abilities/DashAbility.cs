using LostDungeon.Gameplay.Interfaces;
using UnityEngine;

namespace LostDungeon.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Abilities/Dash", fileName = "Dash")]
    public class DashAbility : AbilityDefinition
    {
        [SerializeField, Min(0f)] float speed = 18.75f;
        [SerializeField, Min(0.01f)] float duration = 0.14f;
        [SerializeField, Min(0f)] float extraInvulnerability = 0.08f;

        public override void Activate(IAbilityUser user)
        {
            user.LockVelocity(new Vector2(user.Facing * speed, 0f), duration, true);
            user.SetInvulnerable(duration + extraInvulnerability);
        }
    }
}
