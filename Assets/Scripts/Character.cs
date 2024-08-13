using System.IO;  
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string Name;
    public CharacterRace Race;
    public CharacterClass CharacterClass;
    public int Attractiveness;

    public int Strength;
    public int Charisma;
    public int Dexterity;
    public int Intelligence;

    private GameData gameData;

    public Character(GameData data)
    {
        gameData = data;
    }

    public void SetRaceAndClass(CharacterRace race, CharacterClass characterClass)
    {
        // Corrected references: using "Name" instead of "Race" and "Class"
        var raceData = gameData.Races.FirstOrDefault(r => r.Name == race.ToString());
        var classData = gameData.Classes.FirstOrDefault(c => c.Name == characterClass.ToString());

        if (raceData != null && classData != null)
        {
            Race = race;
            CharacterClass = characterClass;
            Strength = raceData.Strength + classData.Strength;
            Charisma = raceData.Charisma + classData.Charisma;
            Dexterity = raceData.Dexterity + classData.Dexterity;
            Intelligence = raceData.Intelligence + classData.Intelligence;
        }
    }

    public void SaveUserData()
    {
        string json = JsonUtility.ToJson(this, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/user.json", json);
        Debug.Log("User data saved to: " + Application.persistentDataPath + "/user.json");
    }

    public static Character LoadUserData(GameData data)
    {
        string path = Application.persistentDataPath + "/user.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            var character = JsonUtility.FromJson<Character>(json);
            character.gameData = data;
            return character;
        }
        Debug.Log("No user data found, creating new user.");
        return new Character(data); // Return a new Character if no data is found
    }

    public void UpdateStats()
    {
        // Corrected references: using "Name" instead of "Race" and "Class"
        var raceData = gameData.Races.FirstOrDefault(r => r.Name == Race.ToString());
        var classData = gameData.Classes.FirstOrDefault(c => c.Name == CharacterClass.ToString());

        if (raceData != null && classData != null)
        {
            Strength = raceData.Strength + classData.Strength;
            Charisma = raceData.Charisma + classData.Charisma;
            Dexterity = raceData.Dexterity + classData.Dexterity;
            Intelligence = raceData.Intelligence + classData.Intelligence;
        }
    }
}
