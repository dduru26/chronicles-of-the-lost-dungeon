using UnityEditor;
using UnityEngine;

namespace LostDungeon.EditorTools
{
    public static class ProjectSetup
    {
        public static readonly string[] Layers = { "Ground", "Platform", "Player", "Enemy", "PlayerAttack", "EnemyAttack", "Interactable", "Pickup" };
        public static readonly string[] SortingLayers = { "Background", "Midground", "World", "Props", "Characters", "FX", "Foreground" };
        const int FirstLayer = 6;

        [MenuItem("Lost Dungeon/Setup/Create Layers and Collision Rules")]
        public static void Run()
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            for (int i = 0; i < Layers.Length; i++)
            {
                SerializedProperty slot = layers.GetArrayElementAtIndex(FirstLayer + i);
                if (string.IsNullOrEmpty(slot.stringValue)) slot.stringValue = Layers[i];
                else if (slot.stringValue != Layers[i]) Debug.LogWarning($"[Lost Dungeon] Layer {FirstLayer + i} is already '{slot.stringValue}', expected '{Layers[i]}'.");
            }

            SerializedProperty sorting = tagManager.FindProperty("m_SortingLayers");
            foreach (string name in SortingLayers) AddSortingLayer(sorting, name);
            tagManager.ApplyModifiedProperties();

            int ground = LayerMask.NameToLayer("Ground");
            int platform = LayerMask.NameToLayer("Platform");
            int player = LayerMask.NameToLayer("Player");
            int enemy = LayerMask.NameToLayer("Enemy");
            int playerAttack = LayerMask.NameToLayer("PlayerAttack");
            int enemyAttack = LayerMask.NameToLayer("EnemyAttack");
            int interactable = LayerMask.NameToLayer("Interactable");
            int pickup = LayerMask.NameToLayer("Pickup");

            Physics2D.IgnoreLayerCollision(player, enemy, true);
            Physics2D.IgnoreLayerCollision(enemy, enemy, true);
            Physics2D.IgnoreLayerCollision(player, playerAttack, true);
            Physics2D.IgnoreLayerCollision(playerAttack, playerAttack, true);
            Physics2D.IgnoreLayerCollision(playerAttack, platform, true);
            Physics2D.IgnoreLayerCollision(playerAttack, pickup, true);
            Physics2D.IgnoreLayerCollision(playerAttack, interactable, true);
            Physics2D.IgnoreLayerCollision(playerAttack, enemyAttack, true);
            Physics2D.IgnoreLayerCollision(enemy, enemyAttack, true);
            Physics2D.IgnoreLayerCollision(enemyAttack, enemyAttack, true);
            Physics2D.IgnoreLayerCollision(enemyAttack, platform, true);
            Physics2D.IgnoreLayerCollision(enemyAttack, pickup, true);
            Physics2D.IgnoreLayerCollision(enemyAttack, interactable, true);
            Physics2D.IgnoreLayerCollision(enemy, pickup, true);
            Physics2D.IgnoreLayerCollision(enemy, interactable, true);
            Physics2D.IgnoreLayerCollision(pickup, pickup, true);
            Physics2D.IgnoreLayerCollision(interactable, interactable, true);
            Physics2D.IgnoreLayerCollision(ground, ground, true);

            AssetDatabase.SaveAssets();
            Debug.Log("[Lost Dungeon] Layers, sorting layers and 2D collision rules are set up.");
        }

        static void AddSortingLayer(SerializedProperty sortingLayers, string name)
        {
            for (int i = 0; i < sortingLayers.arraySize; i++)
                if (sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == name) return;

            sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
            SerializedProperty entry = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
            entry.FindPropertyRelative("name").stringValue = name;
            entry.FindPropertyRelative("uniqueID").uintValue = (uint)(name.GetHashCode() & 0x7fffffff);
        }
    }
}
