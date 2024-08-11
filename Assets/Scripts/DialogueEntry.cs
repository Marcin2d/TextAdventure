using System.Collections.Generic;

[System.Serializable]
public class DialogueEntry
{
    public string ID;
    public string Speaker;
    public string Prompt;
    public List<string> AcceptedInput;
    public List<string> NegativeInput;
    public List<string> AlternativeInputs;
    public string NextPromptID;
    public string NegativePrompt;
    public string AlternativePrompt;
    public string UnacceptedInputResponse;
    public List<string> Tags;
    public List<string> Conditions;
    public string Trigger;
}
