namespace LostDungeon.Gameplay.Interfaces
{
    public interface ISwitchable
    {
        bool IsOn { get; }
        void SetOn(bool on);
        void Toggle();
    }
}
