using UnityEngine;

namespace LostDungeon.Gameplay.Online
{
    [CreateAssetMenu(menuName = "Lost Dungeon/Online/Leaderboard Config", fileName = "LeaderboardConfig")]
    public class LeaderboardConfig : ScriptableObject
    {
        [SerializeField] string projectUrl = "https://ilcegbpzysjxeaxwbbft.supabase.co";
        [SerializeField] string publishableKey = "sb_publishable_Sw1-e1LeVK1HST3aIWMHxg_pvc-bphr";
        [SerializeField] string table = "scores";
        [SerializeField, Min(1)] int topCount = 10;
        [SerializeField, Min(1)] int timeoutSeconds = 8;

        public string ScoresEndpoint => $"{projectUrl.TrimEnd('/')}/rest/v1/{table}";
        public string PublishableKey => publishableKey;
        public int TopCount => topCount;
        public int TimeoutSeconds => timeoutSeconds;
    }
}
