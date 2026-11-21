using System.Text;
using LostDungeon.Core.Leaderboard;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LostDungeon.Gameplay.Online
{
    public class LeaderboardProbe : MonoBehaviour
    {
        [SerializeField] LeaderboardConfig config;
        [SerializeField] string testPlayerName = "Tester";

        LeaderboardService service;

        void Start()
        {
            service = LeaderboardService.HasInstance ? LeaderboardService.Instance : new GameObject(nameof(LeaderboardService)).AddComponent<LeaderboardService>();
            service.Configure(config);
            service.Updated += LogEntries;
            service.RefreshTop();
            Debug.Log("[Leaderboard Probe] Fetching top scores. Press F9 to submit a random test score.");
        }

        void OnDestroy()
        {
            if (service != null) service.Updated -= LogEntries;
        }

        void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.f9Key.wasPressedThisFrame) return;
            int levels = Random.Range(1, 6);
            int score = ScoreCalculator.Calculate(levels, Random.Range(0, 80), Random.Range(0, 20), Random.Range(0, 3), Random.Range(60f, 600f));
            service.Submit(new LeaderboardEntry(testPlayerName, score, levels, Random.Range(60f, 600f)),
                success => Debug.Log(success ? $"[Leaderboard Probe] Submitted {score}." : $"[Leaderboard Probe] Submit failed, queued offline: {service.LastError}"));
        }

        static void LogEntries(System.Collections.Generic.IReadOnlyList<LeaderboardEntry> entries, LeaderboardStatus status)
        {
            var text = new StringBuilder($"[Leaderboard Probe] {status}, {entries.Count} entries");
            for (int i = 0; i < entries.Count; i++) text.Append($"\n{i + 1}. {entries[i].player_name}  {entries[i].score}");
            Debug.Log(text.ToString());
        }
    }
}
