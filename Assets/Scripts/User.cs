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
        File.WriteAllText(Application.persistentDataPath + "/user.json", json);
    }

    public static User LoadUserData()
    {
        string path = Application.persistentDataPath + "/user.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<User>(json);
        }
        return new User(); // Return a new User if no data is found
    }

    public void UpdateStats(CharacterRace race, CharacterClass characterClass)
    {
        Strength = race.Strength + characterClass.Strength;
        Charisma = race.Charisma + characterClass.Charisma;
        Dexterity = race.Dexterity + characterClass.Dexterity;
        Intelligence = race.Intelligence + characterClass.Intelligence;
    }
}
