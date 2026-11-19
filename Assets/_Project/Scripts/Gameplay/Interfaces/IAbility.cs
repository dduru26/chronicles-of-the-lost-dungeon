namespace LostDungeon.Gameplay.Interfaces
{
    public interface IAbility
    {
        string Id { get; }
        string DisplayName { get; }
        float Cooldown { get; }
        bool CanActivate(IAbilityUser user);
        void Activate(IAbilityUser user);
    }
}
