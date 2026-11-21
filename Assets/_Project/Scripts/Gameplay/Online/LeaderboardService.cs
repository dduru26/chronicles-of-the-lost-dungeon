using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LostDungeon.Core.Algorithms;
using LostDungeon.Core.Leaderboard;
using LostDungeon.Gameplay.Architecture;
using UnityEngine;
using UnityEngine.Networking;

namespace LostDungeon.Gameplay.Online
{
    public class LeaderboardService : Singleton<LeaderboardService>
    {
        [Serializable]
        class EntryList
        {
            public List<LeaderboardEntry> items = new List<LeaderboardEntry>();
        }

        [SerializeField] LeaderboardConfig config;

        readonly List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
        EntryList pending = new EntryList();

        public event Action<IReadOnlyList<LeaderboardEntry>, LeaderboardStatus> Updated;
        public event Action<LeaderboardStatus> StatusChanged;

        public IReadOnlyList<LeaderboardEntry> Entries => entries;
        public LeaderboardStatus Status { get; private set; } = LeaderboardStatus.Idle;
        public string LastError { get; private set; }
        public int PendingCount => pending.items.Count;

        string CachePath => Path.Combine(Application.persistentDataPath, "leaderboard_cache.json");
        string PendingPath => Path.Combine(Application.persistentDataPath, "leaderboard_pending.json");

        protected override void OnSingletonAwake()
        {
            pending = ReadFile<EntryList>(PendingPath) ?? new EntryList();
        }

        public void Configure(LeaderboardConfig leaderboardConfig) => config = leaderboardConfig;

        public void RefreshTop() => StartCoroutine(FetchTopRoutine());

        public void Submit(LeaderboardEntry entry, Action<bool> onComplete = null)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.player_name)) { onComplete?.Invoke(false); return; }
            entry.player_name = entry.player_name.Trim();
            if (entry.player_name.Length > 16) entry.player_name = entry.player_name.Substring(0, 16);
            StartCoroutine(SubmitRoutine(entry, onComplete));
        }

        IEnumerator FetchTopRoutine()
        {
            if (config == null) { Fail("No leaderboard config assigned."); yield break; }

            SetStatus(LeaderboardStatus.Loading);
            yield return FlushPendingRoutine();

            string url = $"{config.ScoresEndpoint}?select=player_name,score,levels_cleared,time_seconds&order=score.desc&limit={config.TopCount}";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                Prepare(request);
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    LoadCacheAfterFailure($"{request.responseCode} {request.error}");
                    yield break;
                }

                EntryList parsed;
                try
                {
                    parsed = JsonUtility.FromJson<EntryList>("{\"items\":" + request.downloadHandler.text + "}");
                }
                catch (Exception exception)
                {
                    LoadCacheAfterFailure("Could not read leaderboard data: " + exception.Message);
                    yield break;
                }

                ReplaceEntries(parsed?.items);
                WriteFile(CachePath, new EntryList { items = new List<LeaderboardEntry>(entries) });
                LastError = null;
                SetStatus(LeaderboardStatus.Online);
                Updated?.Invoke(entries, Status);
            }
        }

        IEnumerator SubmitRoutine(LeaderboardEntry entry, Action<bool> onComplete)
        {
            if (config == null) { Fail("No leaderboard config assigned."); onComplete?.Invoke(false); yield break; }

            bool sent = false;
            yield return PostRoutine(entry, success => sent = success);

            if (!sent)
            {
                pending.items.Add(entry);
                WriteFile(PendingPath, pending);
                InsertLocally(entry);
                SetStatus(LeaderboardStatus.OfflineCache);
                Updated?.Invoke(entries, Status);
            }

            onComplete?.Invoke(sent);
            if (sent) yield return FetchTopRoutine();
        }

        IEnumerator FlushPendingRoutine()
        {
            if (pending.items.Count == 0) yield break;

            var stillPending = new List<LeaderboardEntry>();
            foreach (LeaderboardEntry entry in pending.items)
            {
                bool sent = false;
                yield return PostRoutine(entry, success => sent = success);
                if (!sent) stillPending.Add(entry);
            }

            pending.items = stillPending;
            WriteFile(PendingPath, pending);
        }

        IEnumerator PostRoutine(LeaderboardEntry entry, Action<bool> onComplete)
        {
            byte[] body = Encoding.UTF8.GetBytes(JsonUtility.ToJson(entry));
            using (var request = new UnityWebRequest(config.ScoresEndpoint, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(body);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Prefer", "return=minimal");
                Prepare(request);
                yield return request.SendWebRequest();

                bool success = request.result == UnityWebRequest.Result.Success;
                if (!success) LastError = $"{request.responseCode} {request.error}";
                onComplete(success);
            }
        }

        void Prepare(UnityWebRequest request)
        {
            request.timeout = config.TimeoutSeconds;
            request.SetRequestHeader("apikey", config.PublishableKey);
        }

        void LoadCacheAfterFailure(string error)
        {
            LastError = error;
            EntryList cached = ReadFile<EntryList>(CachePath);
            if (cached != null && cached.items.Count > 0)
            {
                ReplaceEntries(cached.items);
                foreach (LeaderboardEntry entry in pending.items) InsertLocally(entry);
                SetStatus(LeaderboardStatus.OfflineCache);
            }
            else
            {
                entries.Clear();
                foreach (LeaderboardEntry entry in pending.items) InsertLocally(entry);
                SetStatus(LeaderboardStatus.Unavailable);
            }

            Debug.LogWarning($"[Leaderboard] Falling back to {Status}: {error}");
            Updated?.Invoke(entries, Status);
        }

        void ReplaceEntries(List<LeaderboardEntry> source)
        {
            entries.Clear();
            if (source != null) entries.AddRange(source);
            MergeSort.Sort(entries, LeaderboardEntry.CompareByRank);
        }

        void InsertLocally(LeaderboardEntry entry)
        {
            entries.Add(entry);
            MergeSort.Sort(entries, LeaderboardEntry.CompareByRank);
            if (config != null && entries.Count > config.TopCount) entries.RemoveRange(config.TopCount, entries.Count - config.TopCount);
        }

        void Fail(string error)
        {
            LastError = error;
            SetStatus(LeaderboardStatus.Unavailable);
            Debug.LogWarning("[Leaderboard] " + error);
        }

        void SetStatus(LeaderboardStatus status)
        {
            if (Status == status) return;
            Status = status;
            StatusChanged?.Invoke(status);
        }

        static T ReadFile<T>(string path) where T : class
        {
            try
            {
                return File.Exists(path) ? JsonUtility.FromJson<T>(File.ReadAllText(path)) : null;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[Leaderboard] Could not read {path}: {exception.Message}");
                return null;
            }
        }

        static void WriteFile(string path, object data)
        {
            try
            {
                File.WriteAllText(path, JsonUtility.ToJson(data));
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[Leaderboard] Could not write {path}: {exception.Message}");
            }
        }
    }
}
