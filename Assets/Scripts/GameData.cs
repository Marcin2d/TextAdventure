using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<CharacterRaceData> Races;
    public List<CharacterClassData> Classes;
}

[System.Serializable]
public class CharacterRaceData
{
    public CharacterRace Race;
    public int Strength;
    public int Charisma;
    public int Dexterity;
    public int Intelligence;
}

[System.Serializable]
public class CharacterClassData
{
    public CharacterClass Class;
    public int Strength;
    public int Charisma;
    public int Dexterity;
    public int Intelligence;
}
