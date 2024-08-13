using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<CharacterRaceData> Races;  // List of race data
    public List<CharacterClassData> Classes;  // List of class data
}

[System.Serializable]
public class CharacterRaceData
{
    public string Name;  // Race name
    public int Strength;
    public int Charisma;
    public int Dexterity;
    public int Intelligence;
}

[System.Serializable]
public class CharacterClassData
{
    public string Name;  // Class name
    public int Strength;
    public int Charisma;
    public int Dexterity;
    public int Intelligence;
}
