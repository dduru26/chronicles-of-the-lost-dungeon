using UnityEngine;

namespace LostDungeon.Gameplay.Pooling
{
    public class PooledObject : MonoBehaviour
    {
        public PrefabPool Owner { get; set; }

        public void ReturnToPool()
        {
            if (Owner != null) Owner.Despawn(gameObject);
            else Destroy(gameObject);
        }
    }
}
