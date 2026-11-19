using UnityEngine;

namespace LostDungeon.Gameplay.Interfaces
{
    public interface IAbilityUser
    {
        Transform Transform { get; }
        Rigidbody2D Body { get; }
        int Facing { get; }
        void SetInvulnerable(float duration);
    }
}
