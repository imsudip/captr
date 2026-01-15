using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// Modern UI Manager using UI Toolkit (VisualElements) instead of legacy Unity UI.
/// Manages all UI interactions for the capture card software.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class UIManagerToolkit : MonoBehaviour
{
    [Header("UI Document")]
    [Tooltip("UI Document for settings panel")]
    public UIDocument settingsDocument;
    
    [Tooltip("UI Document for volume indicator")]
    public UIDocument volumeDocument;

    [Tooltip("UI Document for keymaps panel")]
    public UIDocument keymapsDocument;

    [Header("Manager References")]
    public CaptureController captureController;
    public VideoCaptureManager videoCaptureManager;
    public AudioCaptureManager audioCaptureManager;
    public SettingsManager settingsManager;

    // UI Elements - Settings Panel
    private VisualElement settingsPanel;
    private DropdownField captureCardDropdown;
    private DropdownField audioDeviceDropdown;
    private DropdownField resolutionDropdown;
    private DropdownField fpsDropdown;
    private DropdownField aspectRatioDropdown;
    private Toggle audioSyncToggle;
    private DropdownField audioSyncFrequencyDropdown;
    private Toggle fpsCounterToggle;
    private Button closeButton;
    private Button saveButton;
    private Button showKeymapsButton;

    // UI Elements - Keymaps Panel
    private VisualElement keymapsPanel;
    private Button keymapsCloseButton;

    // UI Elements - Volume Indicator
    private VisualElement volumePanel;
    public Label volumeText; // Public so CaptureController can access for notifications

    // UI Elements - FPS Counter
    private Label fpsCounterLabel;
    private bool showFpsCounter = false;

    // State
    private bool isSettingsPanelVisible = false;
    private bool isKeymapsPanelVisible = false;
    private float volumeIndicatorTimer = 0f;
    private const float VOLUME_INDICATOR_DURATION = 2f;

    /// <summary>
    /// Initialize UI elements
    /// </summary>
    private void OnEnable()
    {
        InitializeSettingsPanel();
        InitializeKeymapsPanel();
        InitializeVolumeIndicator();
    }

    /// <summary>
    /// Initialize settings panel UI elements
    /// </summary>
    private void InitializeSettingsPanel()
    {
        if (settingsDocument == null)
        {
            Debug.LogError("Settings UIDocument is not assigned!");
            return;
        }

        var root = settingsDocument.rootVisualElement;
        
        // Get main panel
        settingsPanel = root.Q<VisualElement>("settings-panel");
        
        // Get dropdowns
        captureCardDropdown = root.Q<DropdownField>("captureCardDropdown");
        audioDeviceDropdown = root.Q<DropdownField>("audioDeviceDropdown");
        resolutionDropdown = root.Q<DropdownField>("resolutionDropdown");
        fpsDropdown = root.Q<DropdownField>("fpsDropdown");
        aspectRatioDropdown = root.Q<DropdownField>("aspectRatioDropdown");
        
        // Get audio sync controls
        audioSyncToggle = root.Q<Toggle>("audioSyncToggle");
        audioSyncFrequencyDropdown = root.Q<DropdownField>("audioSyncFrequencyDropdown");
        
        // Get FPS counter toggle
        fpsCounterToggle = root.Q<Toggle>("fpsCounterToggle");
        
        // Get buttons
        closeButton = root.Q<Button>("closeButton");
        saveButton = root.Q<Button>("saveButton");
        showKeymapsButton = root.Q<Button>("showKeymapsButton");

        // Initialize dropdowns with data
        InitializeDropdowns();
        
        // Register callbacks
        RegisterCallbacks();
        
        // Start hidden with animation class
        settingsPanel.AddToClassList("panel-hidden");
        settingsPanel.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Initialize keymaps panel UI elements
    /// </summary>
    private void InitializeKeymapsPanel()
    {
        if (keymapsDocument == null)
        {
            Debug.LogWarning("Keymaps UIDocument is not assigned! Keymaps panel will not be available.");
            return;
        }

        var root = keymapsDocument.rootVisualElement;
        
        // Get panel
        keymapsPanel = root.Q<VisualElement>("keymaps-panel");
        
        if (keymapsPanel == null)
        {
            Debug.LogError("Could not find 'keymaps-panel' element in KeymapsPanel.uxml!");
            return;
        }
        
        // Get close button
        keymapsCloseButton = root.Q<Button>("keymapsCloseButton");
        
        if (keymapsCloseButton != null)
        {
            keymapsCloseButton.clicked += OnKeymapsCloseButtonClicked;
        }
        else
        {
            Debug.LogWarning("Could not find keymapsCloseButton!");
        }
        
        // Start hidden with animation class
        keymapsPanel.AddToClassList("keymaps-panel-hidden");
        keymapsPanel.style.display = DisplayStyle.None;
        
        Debug.Log("Keymaps panel initialized successfully");
    }

    /// <summary>
    /// Initialize volume indicator UI elements
    /// </summary>
    private void InitializeVolumeIndicator()
    {
        if (volumeDocument == null)
        {
            Debug.LogError("Volume UIDocument is not assigned!");
            return;
        }

        var root = volumeDocument.rootVisualElement;
        
        volumePanel = root.Q<VisualElement>("volume-indicator");
        volumeText = root.Q<Label>("volumeText");
        
        // Start hidden
        volumePanel.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Initialize all dropdowns with options
    /// </summary>
    private void InitializeDropdowns()
    {
        // Capture Card Dropdown
        if (captureCardDropdown != null && videoCaptureManager != null)
        {
            var captureOptions = new List<string> { "No Capture Card" };
            WebCamDevice[] devices = videoCaptureManager.GetAvailableDevices();
            foreach (var device in devices)
            {
                captureOptions.Add(device.name);
            }
            captureCardDropdown.choices = captureOptions;
            captureCardDropdown.index = 0;
        }

        // Audio Device Dropdown
        if (audioDeviceDropdown != null && audioCaptureManager != null)
        {
            var audioOptions = new List<string> { "No Audio Input" };
            string[] devices = audioCaptureManager.GetAvailableDevices();
            foreach (var device in devices)
            {
                audioOptions.Add(device);
            }
            audioDeviceDropdown.choices = audioOptions;
            audioDeviceDropdown.index = 0;
        }

        // Resolution Dropdown
        if (resolutionDropdown != null)
        {
            resolutionDropdown.choices = new List<string>
            {
                "1920 x 1080",
                "1280 x 720",
                "2560 x 1440",
                "3840 x 2160"
            };
            resolutionDropdown.index = 0;
        }

        // FPS Dropdown
        if (fpsDropdown != null)
        {
            fpsDropdown.choices = new List<string>
            {
                "30 fps",
                "60 fps",
                "120 fps"
            };
            fpsDropdown.index = 1; // Default 60 fps
        }

        // Aspect Ratio Dropdown
        if (aspectRatioDropdown != null)
        {
            aspectRatioDropdown.choices = new List<string>
            {
                "None",
                "Width Controls Height",
                "Height Controls Width",
                "Fit In Parent",
                "Envelope Parent"
            };
            aspectRatioDropdown.index = 3; // Default Fit In Parent
        }
        
        // Audio Sync Frequency Dropdown
        if (audioSyncFrequencyDropdown != null)
        {
            audioSyncFrequencyDropdown.choices = new List<string>
            {
                "1 min",
                "2 min",
                "5 min",
                "10 min",
                "15 min"
            };
            audioSyncFrequencyDropdown.index = 2; // Default 5 min
        }
    }

    /// <summary>
    /// Register all UI callbacks
    /// </summary>
    private void RegisterCallbacks()
    {
        // Dropdown callbacks
        if (captureCardDropdown != null)
            captureCardDropdown.RegisterValueChangedCallback(OnCaptureCardChanged);
        
        if (audioDeviceDropdown != null)
            audioDeviceDropdown.RegisterValueChangedCallback(OnAudioDeviceChanged);
        
        if (resolutionDropdown != null)
            resolutionDropdown.RegisterValueChangedCallback(OnResolutionChanged);
        
        if (fpsDropdown != null)
            fpsDropdown.RegisterValueChangedCallback(OnFPSChanged);
        
        if (aspectRatioDropdown != null)
            aspectRatioDropdown.RegisterValueChangedCallback(OnAspectRatioChanged);

        // Audio sync callbacks
        if (audioSyncToggle != null)
            audioSyncToggle.RegisterValueChangedCallback(OnAudioSyncToggleChanged);
            
        if (audioSyncFrequencyDropdown != null)
            audioSyncFrequencyDropdown.RegisterValueChangedCallback(OnAudioSyncFrequencyChanged);
        
        // FPS counter callback
        if (fpsCounterToggle != null)
            fpsCounterToggle.RegisterValueChangedCallback(OnFpsCounterToggleChanged);
        
        // Button callbacks
        if (closeButton != null)
            closeButton.clicked += OnCloseButtonClicked;
            
        if (saveButton != null)
            saveButton.clicked += OnSaveButtonClicked;

        if (showKeymapsButton != null)
            showKeymapsButton.clicked += OnShowKeymapsButtonClicked;
    }

    /// <summary>
    /// Update loop for volume indicator timer and FPS counter
    /// </summary>
    private void Update()
    {
        // Handle volume indicator timer
        if (volumePanel != null && volumePanel.style.display == DisplayStyle.Flex)
        {
            volumeIndicatorTimer -= Time.deltaTime;
            if (volumeIndicatorTimer <= 0f)
            {
                HideVolumeIndicator();
            }
        }
        
        // Update FPS counter
        if (showFpsCounter && fpsCounterLabel != null && videoCaptureManager != null)
        {
            if (videoCaptureManager.IsCapturing())
            {
                float actualFps = videoCaptureManager.GetActualFPS();
                fpsCounterLabel.text = $"FPS: {actualFps:F1}";
                fpsCounterLabel.style.display = DisplayStyle.Flex;
            }
            else
            {
                fpsCounterLabel.style.display = DisplayStyle.None;
            }
        }
    }

    #region Panel Management

    /// <summary>
    /// Shows or hides the settings panel with animation
    /// </summary>
    public void ShowSettingsPanel(bool show)
    {
        if (settingsPanel == null) return;

        isSettingsPanelVisible = show;

        if (show)
        {
            // Show panel
            settingsPanel.style.display = DisplayStyle.Flex;
            settingsPanel.AddToClassList("panel-animating");
            
            // Remove hidden class on next frame to trigger animation
            settingsPanel.schedule.Execute(() =>
            {
                settingsPanel.RemoveFromClassList("panel-hidden");
                settingsPanel.AddToClassList("panel-visible");
            }).StartingIn(10); // Small delay for transition
        }
        else
        {
            // Hide panel
            settingsPanel.AddToClassList("panel-animating");
            settingsPanel.RemoveFromClassList("panel-visible");
            settingsPanel.AddToClassList("panel-hidden");
            
            // Hide after animation completes
            settingsPanel.schedule.Execute(() =>
            {
                settingsPanel.style.display = DisplayStyle.None;
                settingsPanel.RemoveFromClassList("panel-animating");
            }).StartingIn(150); // Match CSS transition duration
        }
    }

    /// <summary>
    /// Shows or hides the keymaps panel with animation
    /// </summary>
    public void ShowKeymapsPanel(bool show)
    {
        if (keymapsPanel == null) return;

        isKeymapsPanelVisible = show;

        if (show)
        {
            // Show panel
            keymapsPanel.style.display = DisplayStyle.Flex;
            keymapsPanel.AddToClassList("keymaps-panel-animating");
            
            // Remove hidden class on next frame to trigger animation
            keymapsPanel.schedule.Execute(() =>
            {
                keymapsPanel.RemoveFromClassList("keymaps-panel-hidden");
                keymapsPanel.AddToClassList("keymaps-panel-visible");
            }).StartingIn(10); // Small delay for transition
        }
        else
        {
            // Hide panel
            keymapsPanel.AddToClassList("keymaps-panel-animating");
            keymapsPanel.RemoveFromClassList("keymaps-panel-visible");
            keymapsPanel.AddToClassList("keymaps-panel-hidden");
            
            // Hide after animation completes
            keymapsPanel.schedule.Execute(() =>
            {
                keymapsPanel.style.display = DisplayStyle.None;
                keymapsPanel.RemoveFromClassList("keymaps-panel-animating");
            }).StartingIn(300); // Match CSS transition duration (300ms)
        }
    }

    /// <summary>
    /// Toggles the settings panel visibility
    /// </summary>
    public void ToggleSettingsPanel()
    {
        ShowSettingsPanel(!isSettingsPanelVisible);
    }

    /// <summary>
    /// Shows the volume indicator with current volume
    /// </summary>
    public void ShowVolumeIndicator(float volume)
    {
        if (volumePanel == null || volumeText == null) return;

        int volumePercent = Mathf.RoundToInt(volume * 100);
        volumeText.text = volume > 0 ? $"Volume: {volumePercent}%" : "Muted";

        volumePanel.style.display = DisplayStyle.Flex;
        volumePanel.AddToClassList("volume-panel-visible");
        volumeIndicatorTimer = VOLUME_INDICATOR_DURATION;
    }

    /// <summary>
    /// Hides the volume indicator
    /// </summary>
    private void HideVolumeIndicator()
    {
        if (volumePanel == null) return;

        volumePanel.RemoveFromClassList("volume-panel-visible");
        
        // Hide after fade animation
        volumePanel.schedule.Execute(() =>
        {
            volumePanel.style.display = DisplayStyle.None;
        }).StartingIn(200);
    }

    #endregion

    #region Dropdown Event Handlers

    private void OnCaptureCardChanged(ChangeEvent<string> evt)
    {
        if (captureController != null)
        {
            captureController.OnCaptureCardChanged(evt.newValue);
        }
    }

    private void OnAudioDeviceChanged(ChangeEvent<string> evt)
    {
        if (captureController != null)
        {
            captureController.OnAudioDeviceChanged(evt.newValue);
        }
    }

    private void OnResolutionChanged(ChangeEvent<string> evt)
    {
        if (captureController == null) return;

        int index = resolutionDropdown.index;
        Vector2Int resolution = index switch
        {
            0 => new Vector2Int(1920, 1080),
            1 => new Vector2Int(1280, 720),
            2 => new Vector2Int(2560, 1440),
            3 => new Vector2Int(3840, 2160),
            _ => new Vector2Int(1920, 1080)
        };

        captureController.OnResolutionChanged(resolution);
    }

    private void OnFPSChanged(ChangeEvent<string> evt)
    {
        if (captureController == null) return;

        int index = fpsDropdown.index;
        int fps = index switch
        {
            0 => 30,
            1 => 60,
            2 => 120,
            _ => 60
        };

        captureController.OnFPSChanged(fps);
    }

    private void OnAspectRatioChanged(ChangeEvent<string> evt)
    {
        if (videoCaptureManager == null) return;

        var mode = (UnityEngine.UI.AspectRatioFitter.AspectMode)aspectRatioDropdown.index;
        videoCaptureManager.SetAspectRatioMode(mode);
    }

    private void OnAudioSyncToggleChanged(ChangeEvent<bool> evt)
    {
        bool isEnabled = evt.newValue;
        
        // Enable/disable the frequency dropdown based on toggle state
        if (audioSyncFrequencyDropdown != null)
        {
            audioSyncFrequencyDropdown.SetEnabled(isEnabled);
        }
        
        if (captureController != null)
        {
            if (isEnabled)
            {
                // Start auto sync with selected frequency
                int frequencyMinutes = GetFrequencyMinutes();
                captureController.StartAutoAudioSync(frequencyMinutes);
                Debug.Log($"Audio auto-sync enabled: every {frequencyMinutes} minutes");
            }
            else
            {
                captureController.StopAutoAudioSync();
                Debug.Log("Audio auto-sync disabled");
            }
        }

        // Save setting
        if (settingsManager != null)
        {
            settingsManager.UpdateAudioSyncEnabled(isEnabled);
        }
    }

    private void OnAudioSyncFrequencyChanged(ChangeEvent<string> evt)
    {
        if (audioSyncToggle != null && audioSyncToggle.value && captureController != null)
        {
            int frequencyMinutes = GetFrequencyMinutes();
            captureController.StartAutoAudioSync(frequencyMinutes);
            Debug.Log($"Audio sync frequency changed to: {frequencyMinutes} minutes");

            // Save setting
            if (settingsManager != null)
            {
                settingsManager.UpdateAudioSyncFrequency(frequencyMinutes);
            }
        }
    }

    private int GetFrequencyMinutes()
    {
        if (audioSyncFrequencyDropdown == null) return 5;
        
        return audioSyncFrequencyDropdown.index switch
        {
            0 => 1,   // 1 min
            1 => 2,   // 2 min
            2 => 5,   // 5 min
            3 => 10,  // 10 min
            4 => 15,  // 15 min
            _ => 5    // default 5 min
        };
    }

    private void OnFpsCounterToggleChanged(ChangeEvent<bool> evt)
    {
        showFpsCounter = evt.newValue;
        
        if (showFpsCounter)
        {
            // Create FPS counter label if it doesn't exist
            if (fpsCounterLabel == null && settingsDocument != null)
            {
                var root = settingsDocument.rootVisualElement.parent;
                fpsCounterLabel = new Label();
                fpsCounterLabel.name = "fpsCounter";
                fpsCounterLabel.AddToClassList("fps-counter");
                fpsCounterLabel.style.position = Position.Absolute;
                fpsCounterLabel.style.top = 16;
                fpsCounterLabel.style.right = 16;
                fpsCounterLabel.style.color = new StyleColor(Color.white);
                fpsCounterLabel.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.7f));
                fpsCounterLabel.style.paddingTop = 8;
                fpsCounterLabel.style.paddingBottom = 8;
                fpsCounterLabel.style.paddingLeft = 12;
                fpsCounterLabel.style.paddingRight = 12;
                fpsCounterLabel.style.borderTopLeftRadius = 6;
                fpsCounterLabel.style.borderTopRightRadius = 6;
                fpsCounterLabel.style.borderBottomLeftRadius = 6;
                fpsCounterLabel.style.borderBottomRightRadius = 6;
                fpsCounterLabel.style.fontSize = 16;
                fpsCounterLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                root.Add(fpsCounterLabel);
            }
            
            Debug.Log("FPS Counter enabled");
        }
        else
        {
            if (fpsCounterLabel != null)
            {
                fpsCounterLabel.style.display = DisplayStyle.None;
            }
            Debug.Log("FPS Counter disabled");
        }
    }

    private void OnCloseButtonClicked()
    {
        // Notify CaptureController to update its state
        if (captureController != null)
        {
            captureController.ShowSettings(false);
        }
        else
        {
            ShowSettingsPanel(false);
        }
        Debug.Log("Settings panel closed via close button");
    }

    private void OnSaveButtonClicked()
    {
        if (captureController != null)
        {
            captureController.SaveSettings();
            Debug.Log("Settings saved!");
            
            // Show brief confirmation (you could add a toast notification here)
            ShowVolumeIndicator(100); // Reusing volume indicator temporarily
            if (volumeText != null)
            {
                volumeText.text = "Settings Saved!";
            }
        }
    }

    private void OnShowKeymapsButtonClicked()
    {
        // Toggle the keymaps panel
        ShowKeymapsPanel(!isKeymapsPanelVisible);
        Debug.Log(isKeymapsPanelVisible ? "Keymaps panel opened" : "Keymaps panel closed");
    }

    private void OnKeymapsCloseButtonClicked()
    {
        ShowKeymapsPanel(false);
        Debug.Log("Keymaps panel closed");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Updates UI to reflect loaded settings
    /// </summary>
    public void UpdateUIFromSettings(CaptureSettings settings)
    {
        // Update capture card dropdown
        if (captureCardDropdown != null && !string.IsNullOrEmpty(settings.captureCardName))
        {
            int index = captureCardDropdown.choices.IndexOf(settings.captureCardName);
            if (index >= 0)
            {
                captureCardDropdown.index = index;
            }
        }

        // Update audio device dropdown
        if (audioDeviceDropdown != null && !string.IsNullOrEmpty(settings.audioDeviceName))
        {
            int index = audioDeviceDropdown.choices.IndexOf(settings.audioDeviceName);
            if (index >= 0)
            {
                audioDeviceDropdown.index = index;
            }
        }

        // Update aspect ratio dropdown
        if (aspectRatioDropdown != null)
        {
            aspectRatioDropdown.index = settings.aspectRatioMode;
        }

        // Update audio sync settings
        if (audioSyncToggle != null)
        {
            audioSyncToggle.value = settings.audioSyncEnabled;
        }

        if (audioSyncFrequencyDropdown != null)
        {
            // Map minutes to dropdown index: 1min=0, 2min=1, 5min=2, 10min=3, 15min=4
            int index = settings.audioSyncFrequencyMinutes switch
            {
                1 => 0,
                2 => 1,
                5 => 2,
                10 => 3,
                15 => 4,
                _ => 2 // Default to 5 minutes
            };
            audioSyncFrequencyDropdown.index = index;
        }

        // Apply audio sync settings if enabled
        if (settings.audioSyncEnabled && captureController != null)
        {
            captureController.StartAutoAudioSync(settings.audioSyncFrequencyMinutes);
        }
    }

    #endregion

    /// <summary>
    /// Clean up callbacks
    /// </summary>
    private void OnDisable()
    {
        if (captureCardDropdown != null)
            captureCardDropdown.UnregisterValueChangedCallback(OnCaptureCardChanged);
        
        if (audioDeviceDropdown != null)
            audioDeviceDropdown.UnregisterValueChangedCallback(OnAudioDeviceChanged);
        
        if (resolutionDropdown != null)
            resolutionDropdown.UnregisterValueChangedCallback(OnResolutionChanged);
        
        if (fpsDropdown != null)
            fpsDropdown.UnregisterValueChangedCallback(OnFPSChanged);
        
        if (aspectRatioDropdown != null)
            aspectRatioDropdown.UnregisterValueChangedCallback(OnAspectRatioChanged);

        if (audioSyncToggle != null)
            audioSyncToggle.UnregisterValueChangedCallback(OnAudioSyncToggleChanged);
            
        if (audioSyncFrequencyDropdown != null)
            audioSyncFrequencyDropdown.UnregisterValueChangedCallback(OnAudioSyncFrequencyChanged);

        if (closeButton != null)
            closeButton.clicked -= OnCloseButtonClicked;

        if (saveButton != null)
            saveButton.clicked -= OnSaveButtonClicked;

        if (showKeymapsButton != null)
            showKeymapsButton.clicked -= OnShowKeymapsButtonClicked;

        if (keymapsCloseButton != null)
            keymapsCloseButton.clicked -= OnKeymapsCloseButtonClicked;

        if (fpsCounterToggle != null)
            fpsCounterToggle.UnregisterValueChangedCallback(OnFpsCounterToggleChanged);
    }
}
