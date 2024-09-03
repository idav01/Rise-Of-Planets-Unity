[System.Serializable]
public class Planet
{
    public string Name;
    public int Level;
    public int Power;
    public int Influence;
    public int Commerce;

    public Planet(string name, int level, int power, int influence, int commerce)
    {
        Name = name;
        Level = level;
        Power = power;
        Influence = influence;
        Commerce = commerce;
    }
}
