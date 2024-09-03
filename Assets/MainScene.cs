using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class MainScene : MonoBehaviour
{
    // Existing UI elements
    public TextMeshProUGUI planetNameText;
    public TextMeshProUGUI planetStatsText;
    public TextMeshProUGUI goldAmountText;
    public TextMeshProUGUI collectionTimerText;
    public TextMeshProUGUI planetsOwnedText;
    public TextMeshProUGUI celestialBodiesOwnedText; // New UI element
    public TextMeshProUGUI eventTimerText; // New UI element for event timer
    public Button collectButton;
    public Button battleDroneButton;
    public Button buildButton;
    public Button resetButton;
    public Button assignDroneButton;
    public Transform planetRepresentationTransform;
    public GameObject planetSpherePrefab;
    public Material gaiaMaterial;
    public RenderTexture planetRenderTexture;
    public RawImage planetRawImage;
    public Camera planetCamera;

    // New Missions Menu UI elements
    public GameObject missionsMenu;
    public Button mission1Button;

    // Popup UI elements
    public GameObject popupPanel;
    public Button buildPopupButton;
    public Button sendDronePopupButton;

    // Additional UI buttons
    public GameObject additionalButtonsPanel;
    public Button scienceButton;
    public Button engineeringButton;
    public Button commerceButton;
    public Button diplomacyButton;

    // Background image
    public Image backgroundImage;

    private Profile profile;
    private bool droneAssigned;
    private long lastPlunderTime = 0;
    private GameObject instantiatedPlanetSphere;
    private bool listenersAttached = false;
    private EventTrigger.Entry backgroundClickEntry;
    private EventTrigger backgroundTrigger;
    private bool isPopupVisible = false;
    private bool isZoomedIn = false;
    private float zoomSpeed = 2.0f; // Adjust zoom speed as needed
    private float targetZoomZ = -100.0f; // Adjust target zoom distance as needed
    private float initialZoomZ = -200.0f; // Starting zoom distance

    private Camera mainCamera;
    private float initialCameraX;
    private float initialCameraY;

    void Start()
    {
        Debug.Log("MainScene Start");
        profile = Profile.Instance;

        if (profile == null)
        {
            Debug.LogError("Profile not found");
            return;
        }

        profile.LoadState();

        if (profile.GetPlanets().Count == 0)
        {
            Debug.LogError("No planets found in profile.");
            return;
        }

        UpdateUI();
        StartCoroutine(UpdateTimers());

        AttachButtonListeners();

        droneAssigned = profile.IsDroneAssigned();
        UpdateCollectButtonState();

        SetupPlanetRepresentation();
        HidePopup(true); // Ensure the popup is hidden initially

        // Hide additional buttons initially
        additionalButtonsPanel.SetActive(false);

        // Store the initial camera position
        mainCamera = Camera.main;
        initialCameraX = mainCamera.transform.position.x;
        initialCameraY = mainCamera.transform.position.y;
        mainCamera.transform.position = new Vector3(initialCameraX, initialCameraY, initialZoomZ);
    }

    void SetupPlanetRepresentation()
    {
        Debug.Log("Setting up planet representation.");

        if (planetRenderTexture == null)
        {
            Debug.LogError("Planet Render Texture is not assigned.");
            return;
        }

        if (planetCamera == null)
        {
            Debug.LogError("Planet Camera is not assigned.");
            return;
        }

        int planetLayer = LayerMask.NameToLayer("Planet");
        if (planetLayer == -1)
        {
            Debug.LogError("Planet layer not defined. Please add a layer named 'Planet' in the Inspector.");
            return;
        }

        planetCamera.cullingMask = 1 << planetLayer;
        planetCamera.targetTexture = planetRenderTexture;

        if (planetRawImage != null)
        {
            planetRawImage.texture = planetRenderTexture;
        }
        else
        {
            Debug.LogError("Planet Raw Image is not assigned.");
        }

        if (instantiatedPlanetSphere != null)
        {
            Destroy(instantiatedPlanetSphere);
            instantiatedPlanetSphere = null;
        }

        if (planetSpherePrefab == null)
        {
            Debug.LogError("planetSpherePrefab is null. Please assign it in the Inspector.");
            return;
        }

        instantiatedPlanetSphere = Instantiate(planetSpherePrefab);
        instantiatedPlanetSphere.transform.position = planetCamera.transform.position + planetCamera.transform.forward * 10;
        SetLayerRecursively(instantiatedPlanetSphere, planetLayer);
        instantiatedPlanetSphere.name = planetSpherePrefab.name;

        if (!instantiatedPlanetSphere.GetComponent<RotatingSphere>())
        {
            instantiatedPlanetSphere.AddComponent<RotatingSphere>();
            Debug.Log("Added RotatingSphere component to planet sphere.");
        }

        if (!instantiatedPlanetSphere.GetComponent<Collider>())
        {
            instantiatedPlanetSphere.AddComponent<SphereCollider>();
            Debug.Log("Added SphereCollider component to planet sphere.");
        }

        UpdatePlanetRepresentation(0);

        Debug.Log("Planet sphere setup complete.");
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void UpdatePlanetRepresentation(int planetIndex)
    {
        if (planetIndex < 0 || planetIndex >= profile.GetPlanets().Count)
        {
            Debug.LogError("Invalid planet index.");
            return;
        }

        CelestialBody selectedPlanet = profile.GetPlanets()[planetIndex];
        Renderer renderer = instantiatedPlanetSphere.GetComponent<Renderer>();

        if (renderer != null)
        {
            string textureName = profile.GetCelestialBodyTexture(selectedPlanet.Name);
            if (textureName != null && textureName == "gaia_texture")
            {
                renderer.material = gaiaMaterial;
                Debug.Log("Gaia material applied to the planet sphere.");
            }
            else
            {
                Debug.LogError("Texture not found for planet: " + selectedPlanet.Name);
            }
        }
        else
        {
            Debug.LogError("Renderer not found on planet sphere.");
        }

        planetNameText.text = selectedPlanet.Name + " (Level " + selectedPlanet.Level + ")";
        planetStatsText.text = $"Power: {selectedPlanet.Power}\nInfluence: {selectedPlanet.Influence}\nCommerce: {selectedPlanet.Commerce}";
    }

    void AttachButtonListeners()
    {
        if (listenersAttached) return;

        if (battleDroneButton != null)
            battleDroneButton.onClick.AddListener(OnClickBattleDrone);
        
        if (collectButton != null)
            collectButton.onClick.AddListener(OnClickCollect);
        
        if (buildButton != null)
            buildButton.onClick.AddListener(OnClickBuild);
        
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetGame);
        
        if (assignDroneButton != null)
            assignDroneButton.onClick.AddListener(OnClickAssignDrone);
        
        if (mission1Button != null)
            mission1Button.onClick.AddListener(OnMission1ButtonClick);
        
        if (buildPopupButton != null)
            buildPopupButton.onClick.AddListener(() => { OnClickBuild(); HidePopup(false); });
        
        if (sendDronePopupButton != null)
            sendDronePopupButton.onClick.AddListener(() => { OnClickAssignDrone(); HidePopup(true); });
        
        if (scienceButton != null)
            scienceButton.onClick.AddListener(OnClickScience);
        
        if (engineeringButton != null)
            engineeringButton.onClick.AddListener(OnClickEngineering);
        
        if (commerceButton != null)
            commerceButton.onClick.AddListener(OnClickCommerce);
        
        if (diplomacyButton != null)
            diplomacyButton.onClick.AddListener(OnClickDiplomacy);

        if (eventTimerText != null) // Attach listener to event timer text
            eventTimerText.GetComponent<Button>().onClick.AddListener(OnEventTimerClick);

        listenersAttached = true;
    }

    void AddBackgroundClickListener()
    {
        if (backgroundImage == null) return;

        if (backgroundTrigger == null)
        {
            backgroundTrigger = backgroundImage.gameObject.AddComponent<EventTrigger>();
        }

        if (backgroundClickEntry == null)
        {
            backgroundClickEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };

            backgroundClickEntry.callback.AddListener((eventData) =>
            {
                Debug.Log("Background clicked.");
                if (isPopupVisible)
                {
                    HidePopup(true);
                }
                else if (isZoomedIn)
                {
                    // Reverse the transition if the popup is not visible and zoom is in
                    StartCoroutine(ReverseZoomCoroutine(initialZoomZ));
                }
            });
        }

        if (!backgroundTrigger.triggers.Contains(backgroundClickEntry))
        {
            backgroundTrigger.triggers.Add(backgroundClickEntry);
            Debug.Log("Background click listener added.");
        }
    }

    void RemoveBackgroundClickListener()
    {
        if (backgroundTrigger != null && backgroundClickEntry != null && backgroundTrigger.triggers.Contains(backgroundClickEntry))
        {
            backgroundTrigger.triggers.Remove(backgroundClickEntry);
            Debug.Log("Background click listener removed.");
        }
    }

    void UpdateUI()
    {
        Debug.Log("UpdateUI called");
        var planets = profile.GetPlanets();
        Debug.Log("Number of planets: " + planets.Count);

        if (planets.Count > 0)
        {
            CelestialBody selectedPlanet = planets[0];
            if (planetNameText != null)
            {
                planetNameText.text = selectedPlanet.Name + " (Level " + selectedPlanet.Level + ")";
            }
            else
            {
                Debug.LogError("planetNameText is not assigned in the inspector.");
            }

            if (planetStatsText != null)
            {
                planetStatsText.text = $"Power: {selectedPlanet.Power}\nInfluence: {selectedPlanet.Influence}\nCommerce: {selectedPlanet.Commerce}";
            }
            else
            {
                Debug.LogError("planetStatsText is not assigned in the inspector.");
            }
        }
        else
        {
            Debug.LogError("No planets found to update UI.");
        }

        if (goldAmountText != null)
        {
            goldAmountText.text = "Gold: " + profile.GetGoldAmount();
        }
        else
        {
            Debug.LogError("goldAmountText is not assigned in the inspector.");
        }

        if (planetsOwnedText != null)
        {
            planetsOwnedText.text = "Planets Owned: " + planets.Count;
        }
        else
        {
            Debug.LogError("planetsOwnedText is not assigned in the inspector.");
        }

        if (celestialBodiesOwnedText != null)
        {
            celestialBodiesOwnedText.text = "Celestial Bodies Owned: " + profile.GetCelestialBodies().Count;
        }
        else
        {
            Debug.LogError("celestialBodiesOwnedText is not assigned in the inspector.");
        }

        if (battleDroneButton != null)
        {
            var buttonText = battleDroneButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = "Battle Drone (" + profile.GetDroneCount() + ")";
            }
            else
            {
                Debug.LogError("battleDroneButton does not have a child TextMeshProUGUI component.");
            }
        }
        else
        {
            Debug.LogError("battleDroneButton is not assigned in the inspector.");
        }

        UpdateCollectButtonState();
    }

    IEnumerator UpdateTimers()
    {
        while (true)
        {
            collectionTimerText.text = profile.GetRemainingCollectionTime();
            eventTimerText.text = profile.GetRemainingEventTime(); // Update event timer text
            yield return new WaitForSeconds(1);
        }
    }

    void UpdateCollectButtonState()
    {
        long currentTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long lastCollectionTime = profile.GetLastCollectionTime();
        bool canCollect = (currentTime - lastCollectionTime >= 3600000);

        collectButton.interactable = canCollect;
        collectButton.GetComponentInChildren<TextMeshProUGUI>().text = canCollect ? "Collect" : "Plundering";
        Debug.Log(canCollect ? "Collect button enabled" : "Collect button disabled");
    }

    public void CollectResources()
    {
        Debug.Log("CollectResources called");
        if (droneAssigned)
        {
            long currentTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long lastCollectionTime = profile.GetLastCollectionTime();
            if (currentTime - lastCollectionTime >= 3600000)
            {
                profile.SetGoldAmount(profile.GetGoldAmount() + 200);
                profile.SetLastCollectionTime(currentTime);
                goldAmountText.text = "Gold: " + profile.GetGoldAmount();
                UpdateCollectButtonState();
                collectionTimerText.text = profile.GetRemainingCollectionTime();
                Debug.Log("Collected 200 gold");
            }
        }
        else
        {
            Debug.Log("No drone assigned to collect resources.");
        }
    }

    public void AssignDrone()
    {
        Debug.Log("AssignDrone called");
        if (profile.GetDroneCount() > 0 && !droneAssigned)
        {
            droneAssigned = true;
            profile.SetDroneAssigned(true);
            profile.DeductDrone();
            lastPlunderTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            StartCoroutine(StartPlundering());
            UpdateCollectButtonState();
            Debug.Log("Drone assigned");
        }
        else
        {
            Debug.Log("No drones available or drone already assigned.");
        }
    }

    IEnumerator StartPlundering()
    {
        while (true)
        {
            long currentTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (droneAssigned && (currentTime - lastPlunderTime >= 3600000))
            {
                profile.SetGoldAmount(profile.GetGoldAmount() + 200);
                lastPlunderTime = currentTime;
                goldAmountText.text = "Gold: " + profile.GetGoldAmount();
                Debug.Log("Plundered 200 gold");
            }
            yield return new WaitForSeconds(60);
        }
    }

    public void OnClickBattleDrone()
    {
        Debug.Log("Navigating to BattleDroneScene...");
        profile.SaveState();
        SceneManager.LoadScene("BattleDroneScene", LoadSceneMode.Single);
    }

    public void OnClickBuild()
    {
        Debug.Log("Build button clicked.");
        HidePopup(false);
        ShowAdditionalButtons();
        HideUIElementsForBuildMode();

        // Disable planet interaction
        if (instantiatedPlanetSphere != null)
        {
            instantiatedPlanetSphere.GetComponent<Collider>().enabled = false;
        }

        // Start the smooth zoom
        StartCoroutine(SmoothZoomCoroutine(targetZoomZ));

        // Enable the background click listener for reverse transition
        AddBackgroundClickListener();
    }

    void ShowAdditionalButtons()
    {
        additionalButtonsPanel.SetActive(true);
    }

    IEnumerator SmoothZoomCoroutine(float targetZoomZ)
    {
        while (Mathf.Abs(mainCamera.transform.position.z - targetZoomZ) > 0.01f)
        {
            SmoothZoom(targetZoomZ);
            yield return null;
        }
        mainCamera.transform.position = new Vector3(initialCameraX, initialCameraY, targetZoomZ); // Ensure exact final z position
        isZoomedIn = true;
    }

    IEnumerator ReverseZoomCoroutine(float initialZoomZ)
    {
        // Show main UI elements and hide additional buttons immediately
        ShowMainUIElements();
        additionalButtonsPanel.SetActive(false);

        // Enable planet interaction
        if (instantiatedPlanetSphere != null)
        {
            instantiatedPlanetSphere.GetComponent<Collider>().enabled = true;
        }

        while (Mathf.Abs(mainCamera.transform.position.z - initialZoomZ) > 0.01f)
        {
            SmoothZoom(initialZoomZ);
            yield return null;
        }
        mainCamera.transform.position = new Vector3(initialCameraX, initialCameraY, initialZoomZ); // Ensure exact final z position
        isZoomedIn = false;

        // Disable the background click listener when zooming out
        RemoveBackgroundClickListener();
    }

    void SmoothZoom(float targetZoomZ)
    {
        float newZ = Mathf.Lerp(mainCamera.transform.position.z, targetZoomZ, Time.deltaTime * zoomSpeed);
        mainCamera.transform.position = new Vector3(initialCameraX, initialCameraY, newZ);
    }

    void ShowMainUIElements()
    {
        if (collectButton != null)
        {
            collectButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("collectButton is not assigned in the inspector.");
        }

        if (collectionTimerText != null)
        {
            collectionTimerText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("collectionTimerText is not assigned in the inspector.");
        }

        if (missionsMenu != null)
        {
            missionsMenu.SetActive(true);
        }
        else
        {
            Debug.LogError("missionsMenu is not assigned in the inspector.");
        }

        if (planetsOwnedText != null)
        {
            planetsOwnedText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("planetsOwnedText is not assigned in the inspector.");
        }

        if (celestialBodiesOwnedText != null)
        {
            celestialBodiesOwnedText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("celestialBodiesOwnedText is not assigned in the inspector.");
        }
    }

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        if (profile == null)
        {
            profile = Profile.Instance;
        }

        profile.LoadState();
        AttachButtonListeners();
        SetupPlanetRepresentation();
        UpdateUI();
    }

    bool IsNewPlayer()
    {
        return PlayerPrefs.GetInt("HasSeenCutScene", 0) == 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == instantiatedPlanetSphere)
                {
                    Debug.Log("Planet clicked.");
                    ShowPopup();
                }
                else if (isPopupVisible && !IsPointerOverUIElement())
                {
                    Debug.Log("Clicked outside planet.");
                    HidePopup(true); // Hide the popup if clicking outside of the planet
                }
            }
            else if (isPopupVisible && !IsPointerOverUIElement())
            {
                Debug.Log("Clicked outside planet (no hit).");
                HidePopup(true);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isZoomedIn)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject == instantiatedPlanetSphere)
                    {
                        Debug.Log("Planet click released. Adding background listener.");
                        AddBackgroundClickListener(); // Add the background click listener on mouse up after clicking the planet
                    }
                }
            }
        }
    }

    void ShowPopup()
    {
        if (droneAssigned)
        {
            sendDronePopupButton.interactable = false;
            sendDronePopupButton.GetComponentInChildren<TextMeshProUGUI>().text = "Drone Assigned";
        }
        else
        {
            sendDronePopupButton.interactable = true;
            sendDronePopupButton.GetComponentInChildren<TextMeshProUGUI>().text = "Send Drone";
        }

        popupPanel.SetActive(true);
        isPopupVisible = true;
    }

    void HidePopup(bool removeBackgroundListener)
    {
        popupPanel.SetActive(false);
        isPopupVisible = false;
        if (removeBackgroundListener)
        {
            RemoveBackgroundClickListener();
        }
    }

    public void OnClickScience()
    {
        Debug.Log("Science button clicked. Navigating to ResearchScene...");
        profile.SaveState();
        SceneManager.LoadScene("ResearchScene", LoadSceneMode.Single);
    }

    void OnClickEngineering()
    {
        Debug.Log("Engineering button clicked.");
        // Implement engineering logic here
    }

    void OnClickCommerce()
    {
        Debug.Log("Commerce button clicked.");
        // Implement commerce logic here
    }

    void OnClickDiplomacy()
    {
        Debug.Log("Diplomacy button clicked.");
        // Implement diplomacy logic here
    }

    void HideUIElementsForBuildMode()
    {
        if (collectButton != null)
        {
            collectButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("collectButton is not assigned in the inspector.");
        }

        if (collectionTimerText != null)
        {
            collectionTimerText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("collectionTimerText is not assigned in the inspector.");
        }

        if (missionsMenu != null)
        {
            missionsMenu.SetActive(false);
        }
        else
        {
            Debug.LogError("missionsMenu is not assigned in the inspector.");
        }

        if (planetsOwnedText != null)
        {
            planetsOwnedText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("planetsOwnedText is not assigned in the inspector.");
        }

        if (celestialBodiesOwnedText != null)
        {
            celestialBodiesOwnedText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("celestialBodiesOwnedText is not assigned in the inspector.");
        }
    }

    public void OnClickCollect()
    {
        Debug.Log("OnClickCollect called");
        CollectResources();
    }

    public void OnClickAssignDrone()
    {
        Debug.Log("OnClickAssignDrone called");
        AssignDrone();
    }

    public void OnMission1ButtonClick()
    {
        Debug.Log("Mission 1 Button Clicked");
        SceneManager.LoadScene("MissionScene");
    }

    public void ResetGame()
    {
        profile.ResetProfile();
        if (instantiatedPlanetSphere != null)
        {
            Destroy(instantiatedPlanetSphere);
        }
        instantiatedPlanetSphere = null;
        SetupPlanetRepresentation();
        UpdateUI();
        Debug.Log("Game reset to initial state");
    }

    bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void OnEventTimerClick()
    {
        Debug.Log("Navigating to WeeklyEventScene...");
        SceneManager.LoadScene("WeeklyEventScene");
    }
}
