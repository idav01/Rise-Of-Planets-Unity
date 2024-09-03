using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeeklyEventUI : MonoBehaviour
{
    public WeeklyEventManager eventManager;
    public Button[] chestButtons; // Array of buttons representing chests for each day
    public TextMeshProUGUI[] timerTexts; // Array of texts to display timers for each chest

    void Start()
    {
        for (int i = 0; i < chestButtons.Length; i++)
        {
            int index = i; // Capture the index for the lambda
            chestButtons[i].onClick.AddListener(() => eventManager.CollectReward(index));
        }
    }
}
