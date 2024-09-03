using UnityEngine;
using System.Collections.Generic;

public class Profile : MonoBehaviour
{
    private static Profile _instance;

    public static Profile Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Profile>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("Profile");
                    _instance = go.AddComponent<Profile>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private List<CelestialBody> celestialBodies = new List<CelestialBody>();
    private Dictionary<string, string> celestialBodyTextures = new Dictionary<string, string>();
    private Dictionary<string, int> researchLevels = new Dictionary<string, int>();
    private Dictionary<int, bool> weeklyEventCollected = new Dictionary<int, bool>();

    private int quantumSignaturesForSol = 0;
    private long eventStartTime; // Add this variable

    public void SetGoldAmount(int amount)
    {
        PlayerPrefs.SetInt("GoldAmount", amount);
    }

    public int GetGoldAmount()
    {
        return PlayerPrefs.GetInt("GoldAmount", 1000);
    }

    public void SetLastCollectionTime(long time)
    {
        PlayerPrefs.SetString("LastCollectionTime", time.ToString());
    }

    public long GetLastCollectionTime()
    {
        long.TryParse(PlayerPrefs.GetString("LastCollectionTime", "0"), out long time);
        return time;
    }

    public void SetLastPurchaseTime(long time)
    {
        PlayerPrefs.SetString("LastPurchaseTime", time.ToString());
    }

    public long GetLastPurchaseTime()
    {
        long.TryParse(PlayerPrefs.GetString("LastPurchaseTime", "0"), out long time);
        return time;
    }

    public void SetDroneAssigned(bool assigned)
    {
        PlayerPrefs.SetInt("DroneAssigned", assigned ? 1 : 0);
    }

    public bool IsDroneAssigned()
    {
        return PlayerPrefs.GetInt("DroneAssigned", 0) == 1;
    }

    public void DeductDrone()
    {
        int currentCount = GetDroneCount();
        PlayerPrefs.SetInt("DroneCount", currentCount - 1);
    }

    public int GetDroneCount()
    {
        return PlayerPrefs.GetInt("DroneCount", 0);
    }

    public void AddDrone()
    {
        int currentCount = GetDroneCount();
        PlayerPrefs.SetInt("DroneCount", currentCount + 1);
    }

    public void SetDroneCount(int count)
    {
        PlayerPrefs.SetInt("DroneCount", count);
    }

    public void AddSpaceship(string spaceshipType)
    {
        int currentCount = GetSpaceshipCount();
        PlayerPrefs.SetInt("SpaceshipCount", currentCount + 1);
        // Add logic to save spaceship type if necessary
    }

    public int GetSpaceshipCount()
    {
        return PlayerPrefs.GetInt("SpaceshipCount", 0);
    }

    public void SaveState()
    {
        PlayerPrefs.SetString("CelestialBodies", JsonUtility.ToJson(new CelestialBodyListWrapper { CelestialBodies = celestialBodies }));

        // Serialize the celestialBodyTextures dictionary correctly
        var celestialBodyTextureList = new List<CelestialBodyTextureEntry>();
        foreach (var kvp in celestialBodyTextures)
        {
            celestialBodyTextureList.Add(new CelestialBodyTextureEntry { Name = kvp.Key, Texture = kvp.Value });
        }
        PlayerPrefs.SetString("CelestialBodyTextures", JsonUtility.ToJson(new CelestialBodyTextureListWrapper { CelestialBodyTextures = celestialBodyTextureList }));

        // Save research levels
        var researchLevelsList = new List<ResearchLevelEntry>();
        foreach (var kvp in researchLevels)
        {
            researchLevelsList.Add(new ResearchLevelEntry { Name = kvp.Key, Level = kvp.Value });
        }
        PlayerPrefs.SetString("ResearchLevels", JsonUtility.ToJson(new ResearchLevelListWrapper { ResearchLevels = researchLevelsList }));

        // Save weekly event collected
        var weeklyEventCollectedList = new List<WeeklyEventCollectedEntry>();
        foreach (var kvp in weeklyEventCollected)
        {
            weeklyEventCollectedList.Add(new WeeklyEventCollectedEntry { DayIndex = kvp.Key, Collected = kvp.Value });
        }
        PlayerPrefs.SetString("WeeklyEventCollected", JsonUtility.ToJson(new WeeklyEventCollectedListWrapper { WeeklyEventCollected = weeklyEventCollectedList }));

        PlayerPrefs.SetInt("QuantumSignaturesForSol", quantumSignaturesForSol);

        // Save event start time
        PlayerPrefs.SetString("EventStartTime", eventStartTime.ToString());

        PlayerPrefs.Save();
    }

    public void LoadState()
    {
        string celestialBodiesJson = PlayerPrefs.GetString("CelestialBodies", "");
        if (!string.IsNullOrEmpty(celestialBodiesJson))
        {
            celestialBodies = JsonUtility.FromJson<CelestialBodyListWrapper>(celestialBodiesJson).CelestialBodies;
        }

        string celestialBodyTexturesJson = PlayerPrefs.GetString("CelestialBodyTextures", "");
        if (!string.IsNullOrEmpty(celestialBodyTexturesJson))
        {
            celestialBodyTextures = new Dictionary<string, string>();
            var celestialBodyTextureList = JsonUtility.FromJson<CelestialBodyTextureListWrapper>(celestialBodyTexturesJson).CelestialBodyTextures;
            foreach (var entry in celestialBodyTextureList)
            {
                celestialBodyTextures[entry.Name] = entry.Texture;
            }
        }
        else
        {
            celestialBodyTextures = new Dictionary<string, string>();
        }

        // Load research levels
        string researchLevelsJson = PlayerPrefs.GetString("ResearchLevels", "");
        if (!string.IsNullOrEmpty(researchLevelsJson))
        {
            researchLevels = new Dictionary<string, int>();
            var researchLevelsList = JsonUtility.FromJson<ResearchLevelListWrapper>(researchLevelsJson).ResearchLevels;
            foreach (var entry in researchLevelsList)
            {
                researchLevels[entry.Name] = entry.Level;
            }
        }

        // Load weekly event collected
        string weeklyEventCollectedJson = PlayerPrefs.GetString("WeeklyEventCollected", "");
        if (!string.IsNullOrEmpty(weeklyEventCollectedJson))
        {
            weeklyEventCollected = new Dictionary<int, bool>();
            var weeklyEventCollectedList = JsonUtility.FromJson<WeeklyEventCollectedListWrapper>(weeklyEventCollectedJson).WeeklyEventCollected;
            foreach (var entry in weeklyEventCollectedList)
            {
                weeklyEventCollected[entry.DayIndex] = entry.Collected;
            }
        }

        quantumSignaturesForSol = PlayerPrefs.GetInt("QuantumSignaturesForSol", 0);

        // Load event start time
        long.TryParse(PlayerPrefs.GetString("EventStartTime", "0"), out eventStartTime);
    }

    public void ResetProfile()
    {
        SetGoldAmount(1000);
        SetDroneCount(0);
        SetDroneAssigned(false);
        SetLastCollectionTime(0);
        celestialBodies.Clear();
        celestialBodyTextures.Clear();
        researchLevels.Clear();
        weeklyEventCollected.Clear();
        quantumSignaturesForSol = 0;
        eventStartTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds(); // Initialize event start time
        AddCelestialBody(new CelestialBody("Gaia", 1, 500, 500, 500, CelestialBodyType.Planet));
        SaveState();
    }

    public void AddCelestialBody(CelestialBody celestialBody)
    {
        celestialBodies.Add(celestialBody);
        celestialBodyTextures[celestialBody.Name] = "gaia_texture"; // Assuming the default texture for the planet is "gaia_texture"
        SaveState();
    }

    public List<CelestialBody> GetCelestialBodies()
    {
        return celestialBodies;
    }

    public List<CelestialBody> GetPlanets()
    {
        return celestialBodies.FindAll(cb => cb.Type == CelestialBodyType.Planet);
    }

    public string GetCelestialBodyTexture(string celestialBodyName)
    {
        if (celestialBodyTextures == null)
        {
            Debug.LogError("celestialBodyTextures is null.");
        }
        else if (celestialBodyTextures.TryGetValue(celestialBodyName, out string texture))
        {
            return texture;
        }
        return null;
    }

    public void SetCelestialBodyTexture(string celestialBodyName, string textureName)
    {
        if (celestialBodyTextures != null)
        {
            celestialBodyTextures[celestialBodyName] = textureName;
        }
    }

    public string GetRemainingCollectionTime()
    {
        long currentTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long lastCollectionTime = GetLastCollectionTime();
        long remainingTime = 3600000 - (currentTime - lastCollectionTime); // 1 hour in milliseconds

        if (remainingTime <= 0)
        {
            return "00:00";
        }
        else
        {
            long minutes = remainingTime / 60000;
            long seconds = (remainingTime % 60000) / 1000;
            return string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }
    }

    // Research level methods
    public int GetResearchLevel(string researchName)
    {
        if (researchLevels.TryGetValue(researchName, out int level))
        {
            return level;
        }
        return 0;
    }

    public void SetResearchLevel(string researchName, int level)
    {
        researchLevels[researchName] = level;
    }

    // Weekly event methods
    public bool IsWeeklyEventCollected(int dayIndex)
    {
        return weeklyEventCollected.ContainsKey(dayIndex) && weeklyEventCollected[dayIndex];
    }

    public void SetWeeklyEventCollected(int dayIndex, bool collected)
    {
        weeklyEventCollected[dayIndex] = collected;
        SaveState();
    }

    public int GetQuantumSignaturesCollectedForSol()
    {
        return quantumSignaturesForSol;
    }

    public void AddQuantumSignaturesForSol(int amount)
    {
        quantumSignaturesForSol += amount;
        SaveState();
    }

    public long GetEventStartTime() // Add this method
    {
        return eventStartTime;
    }

    public void SetEventStartTime(long time) // Add this method
    {
        eventStartTime = time;
        SaveState();
    }

    public string GetRemainingEventTime()
    {
        long currentTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long eventDuration = 7 * 24 * 60 * 60 * 1000; // 7 days in milliseconds
        long remainingTime = eventStartTime + eventDuration - currentTime;

        if (remainingTime <= 0)
        {
            return "00:00:00";
        }
        else
        {
            long days = remainingTime / (24 * 60 * 60 * 1000);
            long hours = (remainingTime % (24 * 60 * 60 * 1000)) / (60 * 60 * 1000);
            long minutes = (remainingTime % (60 * 60 * 1000)) / (60 * 1000);
            long seconds = (remainingTime % (60 * 1000)) / 1000;
            return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", days, hours, minutes, seconds);
        }
    }

    [System.Serializable]
    private class CelestialBodyListWrapper
    {
        public List<CelestialBody> CelestialBodies;
    }

    [System.Serializable]
    private class CelestialBodyTextureListWrapper
    {
        public List<CelestialBodyTextureEntry> CelestialBodyTextures;
    }

    [System.Serializable]
    private class CelestialBodyTextureEntry
    {
        public string Name;
        public string Texture;
    }

    [System.Serializable]
    private class ResearchLevelListWrapper
    {
        public List<ResearchLevelEntry> ResearchLevels;
    }

    [System.Serializable]
    private class ResearchLevelEntry
    {
        public string Name;
        public int Level;
    }

    [System.Serializable]
    private class WeeklyEventCollectedListWrapper
    {
        public List<WeeklyEventCollectedEntry> WeeklyEventCollected;
    }

    [System.Serializable]
    private class WeeklyEventCollectedEntry
    {
        public int DayIndex;
        public bool Collected;
    }
}
