using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResearchManager : MonoBehaviour
{
    public GameObject researchButtonPrefab;
    public Transform researchGrid;
    public TextMeshProUGUI selectedResearchText;
    public GameObject researchPopup;
    public TextMeshProUGUI researchDetailsText;
    public Button startResearchButton;
    public Button closePopupButton;
    public Button resetButton;
    public Transform lineContainer;
    public Material lineMaterial;

    private Dictionary<string, ResearchTopic> researchTopics;
    private ResearchTopic selectedTopic;
    private Dictionary<string, Button> topicButtons = new Dictionary<string, Button>();
    private Coroutine currentResearchCoroutine;

    void Start()
    {
        researchPopup.SetActive(false);

        researchTopics = new Dictionary<string, ResearchTopic>
        {
            { "Quantum Theory", new ResearchTopic("Basic Quantum Theory", GenerateResearchDetails("Quantum Theory", new float[] { 5f, 5f, 5f, 5f, 10f }), maxLevel: 5) },
            { "Quantum Entanglement", new ResearchTopic("Quantum Entanglement", GenerateResearchDetails("Quantum Entanglement", new float[] { 10f, 10f, 10f, 10f, 10f }), 0, new List<string> { "Basic Quantum Theory" }, maxLevel: 5) },
            { "Advanced Quantum Physics", new ResearchTopic("Advanced Quantum Physics", GenerateResearchDetails("Advanced Quantum Physics", new float[] { 15f, 15f, 15f, 15f, 15f }), 0, new List<string> { "Quantum Entanglement" }, maxLevel: 5) },
            { "Time Dilation", new ResearchTopic("Time Dilation", GenerateResearchDetails("Time Dilation", new float[] { 5f, 5f, 5f, 5f, 10f }), 0, new List<string> { "Advanced Quantum Physics" }, maxLevel: 5) },
            { "Spacetime Manipulation", new ResearchTopic("Spacetime Manipulation", GenerateResearchDetails("Spacetime Manipulation", new float[] { 10f, 10f, 10f, 10f, 10f }), 0, new List<string> { "Time Dilation" }, maxLevel: 5) },
            { "Black Hole Creation", new ResearchTopic("Black Hole Creation", GenerateResearchDetails("Black Hole Creation", new float[] { 15f, 15f, 15f, 15f, 15f }), 0, new List<string> { "Bending Space" }, maxLevel: 5) },
            { "Black Hole Navigation", new ResearchTopic("Black Hole Navigation", GenerateResearchDetails("Black Hole Navigation", new float[] { 15f, 15f, 15f, 15f, 15f }), 0, new List<string> { "Black Hole Creation" }, maxLevel: 5) },
            { "Gravitation Field Theory", new ResearchTopic("Gravitation Field Theory", GenerateResearchDetails("Gravitation Field Theory", new float[] { 5f, 5f, 5f, 5f, 10f }), maxLevel: 5) },
            { "Energy Conversion", new ResearchTopic("Energy Conversion", GenerateResearchDetails("Energy Conversion", new float[] { 10f, 10f, 10f, 10f, 10f }), 0, new List<string> { "Gravitation Field Theory" }, maxLevel: 5) },
            { "Gravity Distortion", new ResearchTopic("Gravity Distortion", GenerateResearchDetails("Gravity Distortion", new float[] { 15f, 15f, 15f, 15f, 15f }), 0, new List<string> { "Energy Conversion" }, maxLevel: 5) },
            { "Bending Space", new ResearchTopic("Bending Space", GenerateResearchDetails("Bending Space", new float[] { 20f, 20f, 20f, 20f, 20f }), 0, new List<string> { "Mass Acceleration" }, maxLevel: 5) },
            { "Wormhole Creation", new ResearchTopic("Wormhole Creation", GenerateResearchDetails("Wormhole Creation", new float[] { 20f, 20f, 20f, 20f, 20f }), 0, new List<string> { "Spacetime Manipulation" }, maxLevel: 5) },
            { "Space Jump", new ResearchTopic("Space Jump", GenerateResearchDetails("Space Jump", new float[] { 25f, 25f, 25f, 25f, 25f }), 0, new List<string> { "Wormhole Creation", "Black Hole Navigation" }, maxLevel: 5) },            
            { "Wormhole Navigation", new ResearchTopic("Wormhole Navigation", GenerateResearchDetails("Multiverse Travel", new float[] { 25f, 25f, 25f, 25f, 25f }), 0, new List<string> { "Space Jump" }, maxLevel: 5) },
            { "Multiverse Travel", new ResearchTopic("Muliverse Travel", GenerateResearchDetails("Time Jump", new float[] { 25f, 25f, 25f, 25f, 25f }), 0, new List<string> { "Wormhole Navigation" }, maxLevel: 5) },
            { "Time Jump", new ResearchTopic("Time Jump", GenerateResearchDetails("Time Jump", new float[] { 25f, 25f, 25f, 25f, 25f }), 0, new List<string> { "Multiverse Travel" }, maxLevel: 5) },
            { "Controlled Fusion", new ResearchTopic("Controlled Fusion", GenerateResearchDetails("Controlled Fusion", new float[] { 5f, 5f, 5f, 5f, 10f }), maxLevel: 5) },
            { "Magnetohydrodynamic Power Conversion", new ResearchTopic("Magnetohydrodynamic Power Conversion", GenerateResearchDetails("Magnetohydrodynamic Power Conversion", new float[] { 10f, 10f, 10f, 10f, 10f }), 0, new List<string> { "Controlled Fusion" }, maxLevel: 5) },
            { "Plasma Conversion", new ResearchTopic("Plasma Conversion", GenerateResearchDetails("Plasma Conversion", new float[] { 15f, 15f, 15f, 15f, 15f }), 0, new List<string> { "Magnetohydrodynamic Power Conversion" }, maxLevel: 5) },
        };

        LoadResearchLevels();
        PopulateResearchGrid();
        closePopupButton.onClick.AddListener(CloseResearchPopup);
        startResearchButton.onClick.AddListener(StartResearch);
        resetButton.onClick.AddListener(ResetResearch);
    }

    List<ResearchDetail> GenerateResearchDetails(string name, float[] times)
    {
        var details = new List<ResearchDetail>();
        for (int i = 0; i < times.Length; i++)
        {
            details.Add(new ResearchDetail(name, times[i]));
        }
        return details;
    }

    void LoadResearchLevels()
    {
        foreach (var topic in researchTopics.Values)
        {
            topic.currentLevel = Profile.Instance.GetResearchLevel(topic.name);
        }
    }

    void PopulateResearchGrid()
    {
        foreach (Transform child in researchGrid)
        {
            Destroy(child.gameObject);
        }
        topicButtons.Clear();

        // Custom positioning based on levels and dependencies
        Vector2 startPosition = new Vector2(50, -50); // Adjusted for better scaling
        Vector2 offset = new Vector2(100, -100); // Adjusted for closer spacing

        // Manually set positions for each research topic
        Dictionary<string, Vector2> topicPositions = new Dictionary<string, Vector2>
        {
            { "Basic Quantum Theory", startPosition },
            { "Quantum Entanglement", startPosition + new Vector2(1, 0) * offset },
            { "Advanced Quantum Physics", startPosition + new Vector2(2, 0) * offset },
            { "Time Dilation", startPosition + new Vector2(3, 0) * offset },
            { "Spacetime Manipulation", startPosition + new Vector2(4, 0) * offset },
            { "Wormhole Creation", startPosition + new Vector2(5, 0) * offset },

            { "Gravitation Field Theory", startPosition + new Vector2(0, -1) * offset },
            { "Energy Conversion", startPosition + new Vector2(1, -1) * offset },
            { "Gravity Distortion", startPosition + new Vector2(2, -1) * offset },
            { "Bending Space", startPosition + new Vector2(3, -1) * offset },
            { "Black Hole Creation", startPosition + new Vector2(4, -1) * offset },
            { "Black Hole Navigation", startPosition + new Vector2(5, -1) * offset },

            { "Controlled Fusion", startPosition + new Vector2(0, -2) * offset },
            { "Magnetohydrodynamic Power Conversion", startPosition + new Vector2(1, -2) * offset },
            { "Plasma Conversion", startPosition + new Vector2(2, -2) * offset },

            { "Space Jump", startPosition + new Vector2(5.5f, -0.5f) * offset + new Vector2(50, 0) },
            { "Wormhole Navigation", startPosition + new Vector2(6.5f, -0.5f) * offset + new Vector2(50, 0) },
            { "Multiverse Travel", startPosition + new Vector2(7.5f, -0.5f) * offset + new Vector2(50, 0) },
            { "Time Jump", startPosition + new Vector2(8.5f, -0.5f) * offset + new Vector2(50, 0) }, // Centered between the lines and moved right half step and down whole step
        };

        foreach (var topic in researchTopics.Values)
        {
            if (!topicPositions.ContainsKey(topic.name)) continue;

            GameObject buttonObj = Instantiate(researchButtonPrefab, researchGrid);
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = topicPositions[topic.name];
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.SetParent(researchGrid, false);
            rectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f); // Scale down the button

            Debug.Log($"Setting position for {topic.name} to {topicPositions[topic.name]}");
            Button button = buttonObj.GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = $"{topic.name}\n{topic.currentLevel}/{topic.maxLevel}";
            button.interactable = topic.AreDependenciesMet(researchTopics);
            button.onClick.AddListener(() => ShowResearchDetails(topic));
            topicButtons[topic.name] = button;
        }

        DrawDependencyLines();
    }

    void ShowResearchDetails(ResearchTopic topic)
    {
        selectedTopic = topic;
        if (!topic.IsMaxLevel())
        {
            var nextLevelDetail = topic.GetCurrentLevelDetail();
            if (nextLevelDetail != null)
            {
                researchDetailsText.text = $"Research: {topic.name}\nTime: {nextLevelDetail.researchTime} seconds\nLevel: {topic.currentLevel + 1}/{topic.maxLevel}";
                startResearchButton.interactable = true;
            }
            else
            {
                researchDetailsText.text = "No research details available for the next level.";
                startResearchButton.interactable = false;
            }
        }
        else
        {
            researchDetailsText.text = "This research is already at max level.";
            startResearchButton.interactable = false;
        }
        researchPopup.SetActive(true);
    }

    void StartResearch()
    {
        if (selectedTopic != null && !selectedTopic.IsMaxLevel())
        {
            if (currentResearchCoroutine != null)
            {
                StopCoroutine(currentResearchCoroutine);
            }
            currentResearchCoroutine = StartCoroutine(ResearchCountdown(selectedTopic));
            CloseResearchPopup();
        }
    }

    IEnumerator ResearchCountdown(ResearchTopic topic)
    {
        var nextLevelDetail = topic.GetCurrentLevelDetail();
        if (nextLevelDetail == null)
        {
            Debug.LogError("No research detail available for the current level.");
            yield break;
        }

        float researchTime = nextLevelDetail.researchTime;
        float elapsedTime = 0f;

        while (elapsedTime < researchTime)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = researchTime - elapsedTime;
            topicButtons[topic.name].GetComponentInChildren<TextMeshProUGUI>().text = $"{topic.name}\n{topic.currentLevel}/{topic.maxLevel}\n{remainingTime:F1} sec";
            yield return null;
        }

        topic.AdvanceLevel();
        Profile.Instance.SetResearchLevel(topic.name, topic.currentLevel);
        Profile.Instance.SaveState();
        PopulateResearchGrid();
    }

    void CloseResearchPopup()
    {
        researchPopup.SetActive(false);
    }

    void ResetResearch()
    {
        if (currentResearchCoroutine != null)
        {
            StopCoroutine(currentResearchCoroutine);
            currentResearchCoroutine = null;
        }

        foreach (var topic in researchTopics.Values)
        {
            topic.currentLevel = 0;
            Profile.Instance.SetResearchLevel(topic.name, 0);
        }
        Profile.Instance.SaveState();
        PopulateResearchGrid();
    }

    void DrawDependencyLines()
    {
        foreach (Transform child in lineContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var topic in researchTopics.Values)
        {
            foreach (var dependencyName in topic.dependencyNames)
            {
                if (topicButtons.ContainsKey(topic.name) && topicButtons.ContainsKey(dependencyName))
                {
                    Button topicButton = topicButtons[topic.name];
                    Button dependencyButton = topicButtons[dependencyName];

                    GameObject lineObj = new GameObject("Line", typeof(LineRenderer));
                    lineObj.transform.SetParent(lineContainer, false);
                    LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
                    lineRenderer.material = lineMaterial;
                    lineRenderer.startWidth = 0.05f;
                    lineRenderer.endWidth = 0.05f;
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, topicButton.GetComponent<RectTransform>().anchoredPosition);
                    lineRenderer.SetPosition(1, dependencyButton.GetComponent<RectTransform>().anchoredPosition);
                }
            }
        }
    }
}
