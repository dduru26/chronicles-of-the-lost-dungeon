using UnityEngine;

namespace LostDungeon.Gameplay.Interfaces
{
    public interface ICollectable
    {
        string ItemId { get; }
        void Collect(GameObject collector);
    }
}
