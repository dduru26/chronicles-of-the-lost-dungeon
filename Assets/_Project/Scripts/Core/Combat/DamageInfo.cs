namespace LostDungeon.Core.Combat
{
    public readonly struct DamageInfo
    {
        public readonly int Amount;
        public readonly DamageType Type;
        public readonly float SourceX;
        public readonly float SourceY;
        public readonly bool IsCritical;

        public DamageInfo(int amount, DamageType type, float sourceX, float sourceY, bool isCritical = false)
        {
            Amount = amount;
            Type = type;
            SourceX = sourceX;
            SourceY = sourceY;
            IsCritical = isCritical;
        }

        public int DirectionFrom(float targetX) => SourceX <= targetX ? 1 : -1;
    }
}
