using System.Collections.Generic;

[System.Serializable]
public class DialogueEntry
{
    public string ID;                             // Unique identifier for each dialogue entry
    public string Speaker;                        // The name of the speaker for this dialogue entry
    public string Prompt;                         // The text that will be displayed to the player
    public List<string> AcceptedInput;            // List of accepted player responses
    public List<string> NegativeInput;            // List of negative responses that lead to a negative outcome
    public List<string> AlternativeInputs;        // List of alternative responses that might lead to different dialogue paths
    public string NextPromptID;                   // ID of the next dialogue entry to display
    public string NegativePrompt;                 // ID of the negative prompt to display if a negative input is detected
    public string AlternativePrompt;              // ID of the alternative prompt to display if an alternative input is detected
    public string UnacceptedInputResponse;        // Response given when the player's input is not recognized
    public List<string> Tags;                     // Tags for additional context or conditions
    public List<string> Conditions;               // Conditions required for this dialogue entry to be active
    public string Trigger;                        // Trigger to execute after this dialogue entry (e.g., set character name)
}
