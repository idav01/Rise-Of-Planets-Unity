using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleDroneScene : MonoBehaviour
{
    public Button purchaseButton;
    public Button backButton;
    public TextMeshProUGUI droneCostText;
    public Image droneImage;
    private int droneCost = 200;
    private Profile profile;

    void Start()
    {
        Debug.Log("BattleDroneScene Start");
        profile = Profile.Instance;

        if (profile == null)
        {
            Debug.LogError("Profile not found");
            return;
        }

        UpdateUI();
        purchaseButton.onClick.AddListener(() => {
            Debug.Log("Purchase Button Clicked");
            OnPurchaseButtonClick();
        });
        backButton.onClick.AddListener(() => {
            Debug.Log("Back Button Clicked");
            OnBackButtonClick();
        });

        if (!purchaseButton.interactable)
        {
            Debug.LogError("Purchase button is not interactable");
        }
        else
        {
            Debug.Log("Purchase button is interactable");
        }

        Debug.Log("Button listener added");
    }

    void UpdateUI()
    {
        Debug.Log("UpdateUI called");
        droneCostText.text = "Gold Needed: " + droneCost;
        Sprite droneSprite = Resources.Load<Sprite>("drone_image");
        if (droneSprite != null)
        {
            droneImage.sprite = droneSprite;
        }
        else
        {
            Debug.LogError("Drone image not found at specified path.");
        }
    }

    void OnPurchaseButtonClick()
    {
        Debug.Log("Purchase button clicked");

        if (profile.GetGoldAmount() >= droneCost)
        {
            profile.SetGoldAmount(profile.GetGoldAmount() - droneCost);
            profile.AddDrone();
            profile.SaveState();
            Debug.Log("Purchased drone, navigating back to MainScene");
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    void OnBackButtonClick()
    {
        Debug.Log("Navigating back to MainScene...");
        profile.SaveState();
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}
