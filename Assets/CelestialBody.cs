using UnityEngine;

[System.Serializable]
public class CelestialBody
{
    public string Name;
    public int Level;
    public int Power;
    public int Influence;
    public int Commerce;
    public CelestialBodyType Type;

    public int EnergyOutputIncrease;
    public int MiningSpeedIncrease;

    public CelestialBody(string name, int level, int power, int influence, int commerce, CelestialBodyType type)
    {
        Name = name;
        Level = level;
        Power = power;
        Influence = influence;
        Commerce = commerce;
        Type = type;

        if (Type == CelestialBodyType.Star)
        {
            EnergyOutputIncrease = 20;
            MiningSpeedIncrease = 20;
        }
    }

    public void LevelUp()
    {
        Level++;
        Power += 100;
        if (Type == CelestialBodyType.Star)
        {
            EnergyOutputIncrease += 5;
            MiningSpeedIncrease += 5;
        }
        else if (Type == CelestialBodyType.Planet)
        {
            Influence += 50;
            Commerce += 50;
        }
    }
}

public enum CelestialBodyType
{
    Planet,
    Star
}
