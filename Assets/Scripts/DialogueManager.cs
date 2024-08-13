using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;  // Reference to the TextMeshProUGUI component
    public InputField responseInput;      // Input field for player responses
    public Button submitButton;           // Submit button
    public TextAsset dialogueJson;        // JSON file containing dialogue data
    public TextAsset gameDataJson;        // JSON file containing game data

    private DialogueData dialogueData;    // Container for parsed dialogue data
    private GameData gameData;            // Container for parsed game data
    private User playerCharacter;         // The player's character data
    private string currentDialogueID;     // ID of the current dialogue
    private DialogueHistory dialogueHistory = new DialogueHistory();  // Keeps track of dialogue history
    private string historyFilePath;       // File path for saving dialogue history

    void Start()
    {
        // Load Dialogue JSON
        if (dialogueJson != null)
        {
            dialogueData = JsonUtility.FromJson<DialogueData>(dialogueJson.text);
            Debug.Log("Dialogue JSON Loaded: " + dialogueData);
            currentDialogueID = dialogueData.StartDialogueID; // Initialize the current dialogue ID
        }
        else
        {
            Debug.LogError("Dialogue JSON file is not assigned!");
        }

        // Load GameData JSON
        if (gameDataJson != null)
        {
            gameData = JsonUtility.FromJson<GameData>(gameDataJson.text);
            Debug.Log("GameData JSON Loaded: " + gameData);
        }
        else
        {
            Debug.LogError("GameData JSON file is not assigned!");
        }

        // Load User Data
        playerCharacter = User.LoadUserData(gameData);
        Debug.Log("Loaded player data: " + JsonUtility.ToJson(playerCharacter));

        // Set the file path for saving dialogue history
        historyFilePath = Path.Combine(Application.persistentDataPath, "DialogueHistory.json");

        // Display the first prompt
        var startingDialogue = GetDialogueByID(currentDialogueID);
        if (startingDialogue != null)
        {
            DisplayPrompt(startingDialogue.Prompt);
        }
        else
        {
            Debug.LogError("Starting dialogue not found for ID: " + currentDialogueID);
        }

        // Set up the submit button
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
        var dialogue = dialogueData.dialogues.FirstOrDefault(d => d.ID == id);
        if (dialogue == null)
        {
            Debug.LogError("No dialogue found for ID: " + id);
        }
        return dialogue;
    }

    public void DisplayPrompt(string prompt)
    {
        var currentDialogue = GetDialogueByID(currentDialogueID);
        if (currentDialogue != null)
        {
            AddDialogueLine(currentDialogue.Speaker, prompt);
        }
        else
        {
            Debug.LogError("No dialogue found for the current ID: " + currentDialogueID);
        }
    }

    public void OnSubmitResponse()
    {
        if (responseInput != null && !string.IsNullOrWhiteSpace(responseInput.text))
        {
            string playerResponse = responseInput.text;
            AddDialogueLine("Player", playerResponse);
            responseInput.text = "";
            responseInput.ActivateInputField();
            HandleResponse(playerResponse);
        }
        else
        {
            AddSystemMessage("System", "Please enter a response.");
        }
    }

    private void AddDialogueLine(string speaker, string text)
    {
        // Combine the speaker's name and the text
        string formattedText = $"{speaker}: {text}";

        // Update the TextMeshProUGUI component with the combined text
        dialogueText.text = formattedText;

        // Save the dialogue history
        dialogueHistory.History.Add(new DialogueHistoryEntry
        {
            Speaker = speaker,
            Text = text,
            Timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        });

        SaveDialogueHistory();

        Debug.Log($"Dialogue added: {speaker}: {text}");

        // If the speaker is the player, handle the response and update the user data
        if (speaker == "Player")
        {
            var currentDialogue = GetDialogueByID(currentDialogueID);
            if (currentDialogue != null)
            {
                HandleTrigger(currentDialogue.Trigger, text);
            }
            else
            {
                Debug.LogError("Current dialogue is null for ID: " + currentDialogueID);
            }
        }

        // Save the user data after any updates
        playerCharacter.SaveUserData();
    }

    private void AddSystemMessage(string speaker, string message)
    {
        // Combine the speaker's name and the message
        string formattedText = $"{speaker}: {message}";

        // Update the TextMeshProUGUI component with the system message
        dialogueText.text = formattedText;

        // Save the system message to the dialogue history
        dialogueHistory.History.Add(new DialogueHistoryEntry
        {
            Speaker = speaker,
            Text = message,
            Timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        });

        SaveDialogueHistory();
    }

    void HandleResponse(string response)
    {
        var currentDialogue = GetDialogueByID(currentDialogueID);
        if (currentDialogue != null)
        {
            // If the trigger is SetName, directly save the name
            if (currentDialogue.Trigger == "SetName")
            {
                playerCharacter.Name = response;
                playerCharacter.SaveUserData();  // Save user data after setting name
                Debug.Log("Player name set to: " + playerCharacter.Name);
            }

            // Continue with the normal flow
            if (currentDialogue.AcceptedInput.Length == 0 || currentDialogue.AcceptedInput.Contains(response, StringComparer.OrdinalIgnoreCase))
            {
                HandleTrigger(currentDialogue.Trigger, response);
                currentDialogueID = currentDialogue.NextPromptID;
                DisplayPrompt(GetDialogueByID(currentDialogueID).Prompt);
            }
            else
            {
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
        Debug.Log($"Handling trigger: {trigger} with response: {response}");

        switch (trigger)
        {
            case "SetName":
                playerCharacter.Name = response;
                break;
            case "SetRace":
                playerCharacter.Race = (CharacterRace)Enum.Parse(typeof(CharacterRace), response);
                break;
            case "SetClass":
                playerCharacter.CharacterClass = (CharacterClass)Enum.Parse(typeof(CharacterClass), response);
                break;
            case "SetAttractiveness":
                playerCharacter.Attractiveness = int.Parse(response);
                break;
        }

        Debug.Log("Updated Player Character: " + JsonUtility.ToJson(playerCharacter));

        if (trigger == "SetRace" || trigger == "SetClass")
        {
            playerCharacter.UpdateStats();
            Debug.Log("Stats Updated: " + JsonUtility.ToJson(playerCharacter));
        }

        playerCharacter.SaveUserData();  // Save user data after each update
        Debug.Log("User data saved.");
    }

    private void SaveDialogueHistory()
    {
        string json = JsonUtility.ToJson(dialogueHistory, true);
        File.WriteAllText(historyFilePath, json);
        Debug.Log("Dialogue history saved to: " + historyFilePath);
    }

    private void LoadDialogueHistory()
    {
        if (File.Exists(historyFilePath))
        {
            string json = File.ReadAllText(historyFilePath);
            dialogueHistory = JsonUtility.FromJson<DialogueHistory>(json);
            Debug.Log("Dialogue history loaded.");
        }
        else
        {
            Debug.Log("No dialogue history found, starting a new one.");
        }
    }
}
