using UnityEngine;

namespace LostDungeon.Gameplay.Interfaces
{
    public interface IAbilityUser
    {
        Transform Transform { get; }
        Rigidbody2D Body { get; }
        int Facing { get; }
        bool IsGrounded { get; }
        void SetInvulnerable(float duration);
        void LockVelocity(Vector2 velocity, float duration, bool ignoreGravity);
    }
}
