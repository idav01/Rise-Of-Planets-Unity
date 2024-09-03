using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SpaceshipActivity : MonoBehaviour
{
    public Image spaceshipImage;
    public TextMeshProUGUI spaceshipInfoText;
    public Button purchaseButton;

    private Profile profile;
    private int droneCost = 200;

    void Start()
    {
        profile = Profile.Instance;

        if (profile == null)
        {
            Debug.LogError("Profile not found");
            return;
        }

        spaceshipImage.sprite = Resources.Load<Sprite>("transport_ship"); // Ensure this image exists in Resources folder
        spaceshipInfoText.text = "Transport Ship\nCost: 500 Gold\nPower: +500";

        purchaseButton.onClick.AddListener(OnPurchaseButtonClick);
    }

    private void OnPurchaseButtonClick()
    {
        long currentTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long lastPurchaseTime = profile.GetLastPurchaseTime();
        if (currentTime - lastPurchaseTime >= 24 * 60 * 60 * 1000) // 24 hours in milliseconds
        {
            if (profile.GetGoldAmount() >= 500)
            {
                profile.SetGoldAmount(profile.GetGoldAmount() - 500);
                profile.AddSpaceship("transport_ship_a");
                profile.SetLastPurchaseTime(currentTime);
                Debug.Log("Purchased Transport Ship!");

                SceneManager.LoadScene("MainScene");
            }
            else
            {
                Debug.Log("Not enough gold!");
            }
        }
        else
        {
            Debug.Log("You can only purchase one transport ship per day.");
        }
    }
}
