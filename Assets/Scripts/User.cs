using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class User
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

    public void SaveUserData()
    {
        string json = JsonUtility.ToJson(this, true);
        string path = Application.persistentDataPath + "/user.json";
        File.WriteAllText(path, json);
        Debug.Log("User data saved to: " + path);
    }

    public static User LoadUserData(GameData data)
    {
        string path = Application.persistentDataPath + "/user.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var user = JsonUtility.FromJson<User>(json);
            user.gameData = data;
            Debug.Log("User data loaded: " + json);
            return user;
        }
        Debug.Log("No user data found, creating new user.");
        return new User { gameData = data }; // Return a new User if no data is found
    }

    public void UpdateStats()
    {
        var raceData = gameData.Races.FirstOrDefault(r => r.Name == Race.ToString());
        var classData = gameData.Classes.FirstOrDefault(c => c.Name == CharacterClass.ToString());

        if (raceData != null && classData != null)
        {
            Strength = raceData.Strength + classData.Strength;
            Charisma = raceData.Charisma + classData.Charisma;
            Dexterity = raceData.Dexterity + classData.Dexterity;
            Intelligence = raceData.Intelligence + classData.Intelligence;
            Debug.Log("Stats Updated: Strength = " + Strength + ", Charisma = " + Charisma + ", Dexterity = " + Dexterity + ", Intelligence = " + Intelligence);
        }
        else
        {
            Debug.LogError("Race or Class data not found in GameData.");
        }
    }
}
