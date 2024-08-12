using System.Collections.Generic;

[System.Serializable]
public class CharacterRaceData
{
    public CharacterRace Race; // Change from string to CharacterRace
    public int Strength;
    public int Charisma;
    public int Dexterity;
    public int Intelligence;
}

[System.Serializable]
public class CharacterClassData
{
    public CharacterClass Class; // Change from string to CharacterClass
    public int Strength;
    public int Charisma;
    public int Dexterity;
    public int Intelligence;
}
