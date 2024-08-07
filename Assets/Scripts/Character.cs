using System.Linq;  // Include this namespace to use LINQ methods

[System.Serializable]
public class CharacterClass
{
    public string Name;
    public int Strength;
    public int Charisma;
    public int Dexterity;
    public int Intelligence;
}

[System.Serializable]
public class CharacterRace
{
    public string Name;
    public int Strength;
    public int Charisma;
    public int Dexterity;
    public int Intelligence;
}

[System.Serializable]
public class GameData
{
    public CharacterClass[] Classes;
    public CharacterRace[] Races;
}

public class Character
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

    private GameData gameData;

    public Character(GameData data)
    {
        gameData = data;
    }

    public void SetRaceAndClass(string raceName, string className)
    {
        CharacterRace race = gameData.Races.FirstOrDefault(r => r.Name == raceName);
        CharacterClass clazz = gameData.Classes.FirstOrDefault(c => c.Name == className);

        if (race != null && clazz != null)
        {
            Race = raceName;
            CharacterClass = className;
            Strength = race.Strength + clazz.Strength;
            Charisma = race.Charisma + clazz.Charisma;
            Dexterity = race.Dexterity + clazz.Dexterity;
            Intelligence = race.Intelligence + clazz.Intelligence;
        }
    }
}
