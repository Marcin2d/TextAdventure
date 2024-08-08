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
    public string Trigger; // Add Trigger field
}

[System.Serializable]
public class DialogueData
{
    public string StartDialogueID; // Add starting dialogue ID
    public List<DialogueEntry> dialogues;
}

public class DialogueManager : MonoBehaviour
{
    public Text dialogueHistoryText;
    public InputField responseInput;
    public Button submitButton;
    public TextAsset dialogueJson; // Attach your dialogue JSON file in the Unity Inspector
    public TextAsset gameDataJson; // Attach your GameData JSON file in the Unity Inspector

    private DialogueData dialogueData;
    private GameData gameData;
    private string currentDialogueID;
    private string dialogueHistory = "";
    private User playerCharacter;  // Instance of User class

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

        playerCharacter = User.LoadUserData();  // Load user data or create new user

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
        dialogueHistory += "Narrator: " + prompt + "\n";
        dialogueHistoryText.text = dialogueHistory;
    }

    public void OnSubmitResponse()
    {
        if (responseInput != null && !string.IsNullOrWhiteSpace(responseInput.text))
        {
            string playerResponse = responseInput.text;
            UpdateDialogueHistory(playerResponse); // Update history with player response
            HandleResponse(playerResponse);
            responseInput.text = "";  // Clear the input field
            responseInput.ActivateInputField();  // Refocus on the input field
        }
        else
        {
            UpdateSystemMessage("Please enter a response."); // System message without speaker
        }
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
                DisplayPrompt(GetDialogueByID(currentDialogueID).Prompt); // Display the next prompt
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
                playerCharacter.Attractiveness = int.Parse(response); // Ensure the response is a valid number
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

        playerCharacter.SaveUserData(); // Save user data after each update
    }

    void UpdateSystemMessage(string message)
    {
        dialogueHistory += message + "\n"; // Add the message without a speaker prefix
        dialogueHistoryText.text = dialogueHistory;
    }

    void UpdateDialogueHistory(string playerResponse)
    {
        dialogueHistory += "Player: " + playerResponse + "\n";
        dialogueHistoryText.text = dialogueHistory;
    }
}
