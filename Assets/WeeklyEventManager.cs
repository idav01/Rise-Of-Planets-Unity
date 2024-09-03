using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeeklyEventManager : MonoBehaviour
{
    public Button[] chestButtons; // Array of buttons representing chests for each day
    public TextMeshProUGUI[] timerTexts; // Array of texts to display timers for each chest
    public int[] quantumSignaturesPerDay = new int[7]; // Array to store quantum signatures for each day

    public bool isTestingMode = true; // Toggle this to switch between testing and normal modes
    public int testUnlockTimeInSeconds = 10; // Time in seconds for testing

    private Profile profile;
    private System.DateTime[] chestUnlockTimes = new System.DateTime[7];

    void Start()
    {
        profile = Profile.Instance;
        InitializeChestUnlockTimes();
        InitializeChestButtons();
        StartCoroutine(UpdateTimers());
    }

    void InitializeChestUnlockTimes()
    {
        long eventStartTime = profile.GetEventStartTime();
        if (eventStartTime == 0)
        {
            eventStartTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            profile.SetEventStartTime(eventStartTime);
        }

        for (int i = 0; i < chestUnlockTimes.Length; i++)
        {
            if (isTestingMode)
            {
                chestUnlockTimes[i] = System.DateTimeOffset.FromUnixTimeMilliseconds(eventStartTime).AddSeconds(testUnlockTimeInSeconds * (i + 1)).DateTime;
            }
            else
            {
                chestUnlockTimes[i] = System.DateTimeOffset.FromUnixTimeMilliseconds(eventStartTime).AddDays(i).AddHours(24).DateTime;
            }
        }
    }

    void InitializeChestButtons()
    {
        for (int i = 0; i < chestButtons.Length; i++)
        {
            int index = i; // Capture the index for the lambda
            chestButtons[i].onClick.AddListener(() => OnChestButtonClick(index));
            if (profile.IsWeeklyEventCollected(index + 1))
            {
                chestButtons[i].interactable = false;
            }
        }
    }

    void OnChestButtonClick(int dayIndex)
    {
        CollectReward(dayIndex);
    }

    public void CollectReward(int dayIndex)
    {
        if (!profile.IsWeeklyEventCollected(dayIndex + 1))
        {
            int signaturesCollected = quantumSignaturesPerDay[dayIndex];
            profile.AddQuantumSignaturesForSol(signaturesCollected);
            profile.SetWeeklyEventCollected(dayIndex + 1, true);
            chestButtons[dayIndex].interactable = false;
            Debug.Log("Collected " + signaturesCollected + " quantum signatures for Sol.");
        }
    }

    IEnumerator UpdateTimers()
    {
        while (true)
        {
            for (int i = 0; i < chestUnlockTimes.Length; i++)
            {
                if (!profile.IsWeeklyEventCollected(i + 1))
                {
                    System.TimeSpan timeRemaining = chestUnlockTimes[i] - System.DateTime.Now;
                    if (timeRemaining.TotalSeconds > 0)
                    {
                        timerTexts[i].text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds);
                    }
                    else
                    {
                        timerTexts[i].text = "Collect Now!";
                        chestButtons[i].interactable = true;
                    }
                }
                else
                {
                    timerTexts[i].text = "Collected";
                }
            }
            yield return new WaitForSeconds(1);
        }
    }
}
