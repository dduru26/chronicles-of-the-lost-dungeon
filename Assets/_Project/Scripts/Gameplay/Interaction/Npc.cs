using LostDungeon.Core.Events;
using UnityEngine;

namespace LostDungeon.Gameplay.Interaction
{
    public class Npc : Interactable
    {
        [SerializeField] string speakerName = "Ancient Statue";
        [SerializeField, TextArea] string[] lines = { "Few return from the Lost Dungeon." };

        int index;

        public string SpeakerName => speakerName;

        public override bool CanInteract(GameObject interactor) => base.CanInteract(interactor) && lines != null && lines.Length > 0;

        public override void Interact(GameObject interactor)
        {
            string line = lines[index];
            index = (index + 1) % lines.Length;
            GameEvents.RaiseDialogueRequested(speakerName, line);
            Debug.Log($"[{speakerName}] {line}");
        }

        public void SetDialogue(string speaker, string[] newLines)
        {
            speakerName = speaker;
            lines = newLines;
            index = 0;
        }
    }
}
