using System.Collections.Generic;

[System.Serializable]
public class DialogueHistoryEntry
{
    public string Speaker;
    public string Text;
    public string Timestamp;
}

[System.Serializable]
public class DialogueHistory
{
    public List<DialogueHistoryEntry> History = new List<DialogueHistoryEntry>();
}
