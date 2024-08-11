using System.IO;
using UnityEngine;

[System.Serializable]
public class User
{
    public string Name;
    public string Race;
    public string CharacterClass;
    public string Pronouns;
    public int Attractiveness;
    public int Strength;
    public int Charisma;
    public int Dexterity;
    public int Intelligence;

    public void SaveUserData()
    {
        string json = JsonUtility.ToJson(this, true);
        string path = Application.persistentDataPath + "/user.json";
        File.WriteAllText(path, json);
        Debug.Log("User data saved to: " + path);
    }

    public static User LoadUserData()
    {
        string path = Application.persistentDataPath + "/user.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("User data loaded: " + json);
            return JsonUtility.FromJson<User>(json);
        }
        Debug.Log("No user data found, creating new user.");
        return new User(); // Return a new User if no data is found
    }

    public void UpdateStats(CharacterRace race, CharacterClass characterClass)
    {
        Strength = race.Strength + characterClass.Strength;
        Charisma = race.Charisma + characterClass.Charisma;
        Dexterity = race.Dexterity + characterClass.Dexterity;
        Intelligence = race.Intelligence + characterClass.Intelligence;
        Debug.Log("Stats Updated: Strength = " + Strength + ", Charisma = " + Charisma + ", Dexterity = " + Dexterity + ", Intelligence = " + Intelligence);
    }
}
