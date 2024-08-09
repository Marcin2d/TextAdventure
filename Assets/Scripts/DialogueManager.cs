using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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

[System.Serializable]
public class DialogueData
{
    public string StartDialogueID;
    public List<DialogueEntry> dialogues;
}

public class DialogueManager : MonoBehaviour
{
    public ScrollRect dialogueScrollView; // Ensure this is connected in the Unity Inspector
    public Text dialogueHistoryText;      // Connect this in the Unity Inspector
    public InputField responseInput;
    public Button submitButton;
    public TextAsset dialogueJson;
    public TextAsset gameDataJson;

    private DialogueData dialogueData;
    private GameData gameData;
    private string currentDialogueID;
    private User playerCharacter;

    void Start()
    {
        if (dialogueJson != null)
        {
            Debug.Log("Dialogue JSON: " + dialogueJson.text);
            dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJson.text);
            currentDialogueID = dialogueData.StartDialogueID;
        }
        else
        {
            Debug.LogError("Dialogue JSON file is not assigned!");
        }

        if (gameDataJson != null)
        {
            gameData = JsonUtility.FromJson<GameData>(gameDataJson.text);
        }
        else
        {
            Debug.LogError("GameData JSON file is not assigned!");
        }

        playerCharacter = User.LoadUserData();

        DisplayPrompt(GetDialogueByID(currentDialogueID).Prompt);

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitResponse);
        }
        else
        {
            Debug.LogError("Submit Button is not assigned!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSubmitResponse();
        }
    }

    DialogueEntry GetDialogueByID(string id)
    {
        return dialogueData.dialogues.FirstOrDefault(d => d.ID == id);
    }

    public void DisplayPrompt(string prompt)
    {
        AddDialogueLine("Narrator: " + prompt);
    }

    public void OnSubmitResponse()
    {
        if (responseInput != null && !string.IsNullOrWhiteSpace(responseInput.text))
        {
            string playerResponse = responseInput.text;
            AddDialogueLine("Player: " + playerResponse); // Update the display with the player's response
            responseInput.text = "";  // Clear the input field
            responseInput.ActivateInputField();  // Refocus on the input field
            HandleResponse(playerResponse);
        }
        else
        {
            AddSystemMessage("Please enter a response.");
        }
    }

    private void AddDialogueLine(string text)
    {
        // Update the dialogue history with the new line of text
        dialogueHistoryText.text += text + "\n";

        // Force Canvas update to recalculate sizes
        Canvas.ForceUpdateCanvases();

        // Scroll to the bottom
        dialogueScrollView.verticalNormalizedPosition = 0f;
    }

    private void AddSystemMessage(string message)
    {
        // Update the dialogue history with a system message
        dialogueHistoryText.text += message + "\n";

        // Force Canvas update to recalculate sizes
        Canvas.ForceUpdateCanvases();

        // Scroll to the bottom
        dialogueScrollView.verticalNormalizedPosition = 0f;
    }

    void HandleResponse(string response)
    {
        var currentDialogue = GetDialogueByID(currentDialogueID);
        if (currentDialogue != null)
        {
            Debug.Log("Current Dialogue ID: " + currentDialogueID);
            Debug.Log("Player Response: " + response);

            if (currentDialogueID == "charCreate002" || currentDialogue.AcceptedInput.Any(input => input.Equals(response, System.StringComparison.OrdinalIgnoreCase)))
            {
                HandleTrigger(currentDialogue.Trigger, response);
                currentDialogueID = currentDialogue.NextPromptID;
                Debug.Log("Next Dialogue ID: " + currentDialogueID);
                DisplayPrompt(GetDialogueByID(currentDialogueID).Prompt);
            }
            else
            {
                Debug.LogWarning("AcceptedInput does not contain the response: " + response);
                DisplayPrompt(currentDialogue.UnacceptedInputResponse);
            }
        }
        else
        {
            Debug.LogError("Current dialogue is null for ID: " + currentDialogueID);
        }
    }

    void HandleTrigger(string trigger, string response)
    {
        switch (trigger)
        {
            case "SetName":
                playerCharacter.Name = response;
                Debug.Log("Player Name Set: " + playerCharacter.Name);
                break;
            case "SetRace":
                playerCharacter.Race = response;
                break;
            case "SetClass":
                playerCharacter.CharacterClass = response;
                break;
            case "SetPronouns":
                playerCharacter.Pronouns = response;
                break;
            case "SetAttractiveness":
                playerCharacter.Attractiveness = int.Parse(response);
                break;
        }

        if (trigger == "SetRace" || trigger == "SetClass")
        {
            CharacterRace race = gameData.Races.FirstOrDefault(r => r.Name == playerCharacter.Race);
            CharacterClass clazz = gameData.Classes.FirstOrDefault(c => c.Name == playerCharacter.CharacterClass);
            if (race != null && clazz != null)
            {
                playerCharacter.UpdateStats(race, clazz);
            }
        }

        playerCharacter.SaveUserData();
    }
}
