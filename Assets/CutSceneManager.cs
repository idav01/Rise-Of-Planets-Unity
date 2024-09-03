using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CutSceneManager : MonoBehaviour
{
    public Transform comicImageTransform; // Transform to hold the 3D planet sphere
    public Image comicImage; // Reference to the UI Image component for 2D images
    public TextMeshProUGUI comicText; // Reference to the TextMeshProUGUI component
    public Button nextButton; // Reference to the UI Button component
    public Button collectButton; // Reference to the Collect Button component for the final scene
    public GameObject planetSpherePrefab; // Reference to the planet sphere prefab
    public Material gaiaMaterial; // Reference to the Gaia material

    private int currentStep = 0;

    // Array or List of texts for the cut scene
    private string[] texts = new string[]
    {
        "In the age of quantumverse technology, Planet Legions have emerged to harness the power of the universe...",
        "With the ability to harvest mass amounts of power, our world faces new threats and opportunities...",
        "Now, it's time to mount defenses and expand through the ever-evolving quantumverse. Your journey begins here...",
        "Quantum signature has been locked for your home planet. Press collect to add signature to planetary matrix...."
    };

    // Array or List of Sprites for the cut scene
    private Sprite[] images;

    void Start()
    {
        // Load your images into the array
        images = new Sprite[]
        {
            Resources.Load<Sprite>("Images/comic1"),
            Resources.Load<Sprite>("Images/comic2"),
            Resources.Load<Sprite>("Images/comic3"),
            null // The final image will be a 3D sphere
        };

        nextButton.onClick.AddListener(OnNextButtonClicked);
        collectButton.onClick.AddListener(OnCollectButtonClicked);
        UpdateCutScene();
    }

    void OnNextButtonClicked()
    {
        currentStep++;
        if (currentStep < texts.Length - 1)
        {
            UpdateCutScene();
        }
        else
        {
            ShowFinalScene();
        }
    }

    void OnCollectButtonClicked()
    {
        AddCelestialBody();
        PlayerPrefs.SetInt("HasSeenCutScene", 1); // Set flag to indicate cut scene has been seen
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainScene"); // Transition to main scene
    }

    void UpdateCutScene()
    {
        if (images[currentStep] != null)
        {
            comicImage.sprite = images[currentStep];
            comicImage.gameObject.SetActive(true);
            comicImageTransform.gameObject.SetActive(false);
        }
        else
        {
            comicImage.gameObject.SetActive(false);
            comicImageTransform.gameObject.SetActive(true);
        }

        comicText.text = texts[currentStep];
        nextButton.gameObject.SetActive(true);
        collectButton.gameObject.SetActive(false);
    }

    void ShowFinalScene()
    {
        comicImage.gameObject.SetActive(false); // Hide the comic image
        if (planetSpherePrefab != null && comicImageTransform != null)
        {
            GameObject planetSphere = Instantiate(planetSpherePrefab, comicImageTransform.position, Quaternion.identity, comicImageTransform);
            planetSphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Adjust scale as needed
            Renderer renderer = planetSphere.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = gaiaMaterial;
            }
        }

        comicText.text = texts[currentStep];
        nextButton.gameObject.SetActive(false);
        collectButton.gameObject.SetActive(true);
    }

    void AddCelestialBody()
    {
        // Add planet functionality
        Profile profile = Profile.Instance;
        if (profile != null)
        {
            profile.AddCelestialBody(new CelestialBody("Gaia", 1, 500, 500, 500, CelestialBodyType.Planet)); // Add planet with specific attributes
            profile.SetCelestialBodyTexture("Gaia", "gaia_texture"); // Track the texture
            profile.SaveState();
        }
        Debug.Log("Celestial body added!");
    }
}
