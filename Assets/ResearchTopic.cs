using System.Collections.Generic;

[System.Serializable]
public class ResearchDetail
{
    public string name;
    public float researchTime;

    public ResearchDetail(string name, float researchTime)
    {
        this.name = name;
        this.researchTime = researchTime;
    }
}

public class ResearchTopic
{
    public string name;
    public int currentLevel;
    public int maxLevel;
    public List<ResearchDetail> levels;
    public List<string> dependencyNames; // Store dependency names instead of references

    public ResearchTopic(string name, List<ResearchDetail> levels, int currentLevel = 0, List<string> dependencyNames = null, int maxLevel = 5)
    {
        this.name = name;
        this.levels = levels;
        this.currentLevel = currentLevel;
        this.dependencyNames = dependencyNames ?? new List<string>();
        this.maxLevel = maxLevel;
    }

    public ResearchDetail GetCurrentLevelDetail()
    {
        return currentLevel < levels.Count ? levels[currentLevel] : null;
    }

    public void AdvanceLevel()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
        }
    }

    public bool IsMaxLevel()
    {
        return currentLevel >= maxLevel;
    }

    public bool AreDependenciesMet(Dictionary<string, ResearchTopic> allTopics)
    {
        foreach (var dependencyName in dependencyNames)
        {
            if (!allTopics.ContainsKey(dependencyName) || allTopics[dependencyName].currentLevel < allTopics[dependencyName].maxLevel)
            {
                return false;
            }
        }
        return true;
    }
}
