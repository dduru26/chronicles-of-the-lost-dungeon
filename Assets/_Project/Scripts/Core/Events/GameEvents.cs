using System;

namespace LostDungeon.Core.Events
{
    public delegate void HealthChangedHandler(int current, int max);
    public delegate void GameStateChangedHandler(GameState previous, GameState current);
    public delegate void AbilityActivatedHandler(string abilityId, float cooldown);
    public delegate void ObjectiveUpdatedHandler(string description, bool isComplete);

    public static class GameEvents
    {
        public static event HealthChangedHandler HealthChanged;
        public static event GameStateChangedHandler GameStateChanged;
        public static event AbilityActivatedHandler AbilityActivated;
        public static event ObjectiveUpdatedHandler ObjectiveUpdated;
        public static event Action<int> GoldChanged;
        public static event Action<string, int> ItemCollected;
        public static event Action InventoryChanged;
        public static event Action<string> EnemyDefeated;
        public static event Action<int> LevelStarted;
        public static event Action<int> LevelCompleted;
        public static event Action PlayerDied;
        public static event Action<int> BossPhaseChanged;
        public static event Action<string> Interacted;

        public static void RaiseHealthChanged(int current, int max) => HealthChanged?.Invoke(current, max);
        public static void RaiseGameStateChanged(GameState previous, GameState current) => GameStateChanged?.Invoke(previous, current);
        public static void RaiseAbilityActivated(string abilityId, float cooldown) => AbilityActivated?.Invoke(abilityId, cooldown);
        public static void RaiseObjectiveUpdated(string description, bool isComplete) => ObjectiveUpdated?.Invoke(description, isComplete);
        public static void RaiseGoldChanged(int total) => GoldChanged?.Invoke(total);
        public static void RaiseItemCollected(string itemId, int newCount) => ItemCollected?.Invoke(itemId, newCount);
        public static void RaiseInventoryChanged() => InventoryChanged?.Invoke();
        public static void RaiseEnemyDefeated(string enemyId) => EnemyDefeated?.Invoke(enemyId);
        public static void RaiseLevelStarted(int levelIndex) => LevelStarted?.Invoke(levelIndex);
        public static void RaiseLevelCompleted(int levelIndex) => LevelCompleted?.Invoke(levelIndex);
        public static void RaisePlayerDied() => PlayerDied?.Invoke();
        public static void RaiseBossPhaseChanged(int phase) => BossPhaseChanged?.Invoke(phase);
        public static void RaiseInteracted(string interactableId) => Interacted?.Invoke(interactableId);

        public static void ClearAll()
        {
            HealthChanged = null;
            GameStateChanged = null;
            AbilityActivated = null;
            ObjectiveUpdated = null;
            GoldChanged = null;
            ItemCollected = null;
            InventoryChanged = null;
            EnemyDefeated = null;
            LevelStarted = null;
            LevelCompleted = null;
            PlayerDied = null;
            BossPhaseChanged = null;
            Interacted = null;
        }
    }
}
