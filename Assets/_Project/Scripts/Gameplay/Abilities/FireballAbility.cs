using LostDungeon.Core.Combat;
using LostDungeon.Gameplay.Combat;
using LostDungeon.Gameplay.Interfaces;
using LostDungeon.Gameplay.Pooling;
using UnityEngine;

namespace LostDungeon.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Abilities/Fireball", fileName = "Fireball")]
    public class FireballAbility : AbilityDefinition
    {
        [SerializeField] Projectile projectilePrefab;
        [SerializeField, Min(0f)] float speed = 13f;
        [SerializeField, Min(1)] int damage = 3;
        [SerializeField, Min(0.1f)] float lifetime = 1.6f;
        [SerializeField] LayerMask targetMask;
        [SerializeField] Vector2 spawnOffset = new Vector2(0.8f, 1.1f);

        public override bool CanActivate(IAbilityUser user) => base.CanActivate(user) && projectilePrefab != null;

        public override void Activate(IAbilityUser user)
        {
            Vector2 origin = (Vector2)user.Transform.position + new Vector2(spawnOffset.x * user.Facing, spawnOffset.y);
            GameObject instance = PoolManager.GetOrCreate().Spawn(projectilePrefab.gameObject, origin, Quaternion.identity);
            instance.GetComponent<Projectile>().Launch(new Vector2(user.Facing * speed, 0f), damage, DamageType.Magic, targetMask, lifetime, user.Transform);
        }
    }
}
