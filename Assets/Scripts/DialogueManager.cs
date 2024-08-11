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
    private string currentDialogueID;     // ID of the current dialogue
    private User playerCharacter;         // The player's character data
    private DialogueHistory dialogueHistory = new DialogueHistory();  // Keeps track of dialogue history
    private string historyFilePath;       // File path for saving dialogue history

    void Start()
    {
        if (dialogueJson != null)
        {
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

        playerCharacter = User.LoadUserData(); // Load or create user data

        // Set the file path for saving dialogue history
        historyFilePath = Path.Combine(Application.persistentDataPath, "DialogueHistory.json");

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
        var currentDialogue = GetDialogueByID(currentDialogueID);
        if (currentDialogue != null)
        {
            AddDialogueLine(currentDialogue.Speaker, prompt);
        }
        else
        {
            Debug.LogError("No dialogue found for the current ID.");
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
            if (currentDialogueID == "charCreate002" || currentDialogue.AcceptedInput.Any(input => input.Equals(response, System.StringComparison.OrdinalIgnoreCase)))
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
        switch (trigger)
        {
            case "SetName":
                playerCharacter.Name = response;
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

        Debug.Log("Updated Player Character: " + JsonUtility.ToJson(playerCharacter));

        if (trigger == "SetRace" || trigger == "SetClass")
        {
            CharacterRace race = gameData.Races.FirstOrDefault(r => r.Name == playerCharacter.Race);
            CharacterClass clazz = gameData.Classes.FirstOrDefault(c => c.Name == playerCharacter.CharacterClass);
            if (race != null && clazz != null)
            {
                playerCharacter.UpdateStats(race, clazz);
                Debug.Log("Stats Updated: " + JsonUtility.ToJson(playerCharacter));
            }
        }

        playerCharacter.SaveUserData();  // Save user data after each update
        Debug.Log("User data saved.");
    }

    private void SaveDialogueHistory()
    {
        string json = JsonUtility.ToJson(dialogueHistory, true);
        File.WriteAllText(historyFilePath, json);
    }

    private void LoadDialogueHistory()
    {
        if (File.Exists(historyFilePath))
        {
            string json = File.ReadAllText(historyFilePath);
            dialogueHistory = JsonUtility.FromJson<DialogueHistory>(json);
        }
    }
}
