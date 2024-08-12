using System.Collections.Generic;

[System.Serializable]
public class DialogueData
{
    public string StartDialogueID;                // ID of the first dialogue entry to display when starting a dialogue sequence
    public List<DialogueEntry> dialogues;         // List of all dialogue entries
}
