using UnityEngine;

/// <summary>
/// Main controller for the Video Game Capture application.
/// This is the central hub that coordinates all managers and handles user input.
/// Think of this as the "brain" of your application.
/// </summary>
public class CaptureController : MonoBehaviour
{
    [Header("Manager References")]
    [Tooltip("Manages video capture from capture cards")]
    public VideoCaptureManager videoCaptureManager;
    
    [Tooltip("Manages audio capture from capture cards")]
    public AudioCaptureManager audioCaptureManager;
    
    [Tooltip("Manages UI display and interactions (UI Toolkit)")]
    public UIManagerToolkit uiManager;
    
    [Tooltip("Manages application settings (save/load)")]
    public SettingsManager settingsManager;

    [Header("Application Settings")]
    [Tooltip("Target frame rate for the application")]
    public int targetFrameRate = 60;

    // Private variables
    private float currentVolume = 0.5f; // Stores current volume level
    private bool isSettingsOpen = false; // Tracks if settings menu is open

    /// <summary>
    /// Called when the script instance is first loaded (before Start)
    /// Good place to initialize references
    /// </summary>
    private void Awake()
    {
        // Set the target frame rate for smooth playback
        Application.targetFrameRate = targetFrameRate;
        
        Debug.Log("CaptureController initialized");
    }

    /// <summary>
    /// Called before the first frame update
    /// Good place to set up initial state
    /// </summary>
    private void Start()
    {
        // Load saved settings from previous session
        LoadSettings();
        
        // Show settings menu on startup so user can select capture card
        ShowSettings(true);
        
        // Make sure cursor is visible for UI interaction
        Cursor.visible = true;
    }

    /// <summary>
    /// Called once per frame
    /// Good place to check for user input
    /// </summary>
    private void Update()
    {
        HandleKeyboardInput();
    }

    /// <summary>
    /// Handles all keyboard shortcuts for the application
    /// </summary>
    private void HandleKeyboardInput()
    {
        // ESC - Toggle settings menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }

        // F5 - Toggle fullscreen/windowed mode
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ToggleFullscreen();
        }

        // Up Arrow - Increase volume by 10%
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeVolume(0.1f);
        }

        // Down Arrow - Decrease volume by 10%
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeVolume(-0.1f);
        }

        // M - Mute/unmute audio
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }

        // R - Manual audio sync (restart audio)
        if (Input.GetKeyDown(KeyCode.R))
        {
            ManualAudioSync();
        }
    }

    #region Settings Management

    /// <summary>
    /// Shows or hides the settings menu
    /// </summary>
    public void ShowSettings(bool show)
    {
        isSettingsOpen = show;
        
        if (uiManager != null)
        {
            uiManager.ShowSettingsPanel(show);
        }
        
        // Show cursor when settings are open, hide when playing
        Cursor.visible = show;
    }

    /// <summary>
    /// Toggles the settings menu on/off
    /// </summary>
    public void ToggleSettings()
    {
        ShowSettings(!isSettingsOpen);
    }

    /// <summary>
    /// Loads saved settings from SettingsManager
    /// </summary>
    private void LoadSettings()
    {
        if (settingsManager != null)
        {
            var settings = settingsManager.LoadSettings();
            
            // Apply saved settings to managers
            if (videoCaptureManager != null && !string.IsNullOrEmpty(settings.captureCardName))
            {
                videoCaptureManager.SetCaptureCard(settings.captureCardName, settings.resolution, settings.fps);
            }
            
            if (audioCaptureManager != null && !string.IsNullOrEmpty(settings.audioDeviceName))
            {
                audioCaptureManager.SetAudioDevice(settings.audioDeviceName);
            }
            
            // Restore volume
            currentVolume = settings.volume;
            if (audioCaptureManager != null)
            {
                audioCaptureManager.SetVolume(currentVolume);
            }
        }
    }

    /// <summary>
    /// Saves current settings using SettingsManager
    /// </summary>
    public void SaveSettings()
    {
        if (settingsManager != null)
        {
            settingsManager.SaveSettings();
        }
    }

    #endregion

    #region Video Controls

    /// <summary>
    /// Changes the capture card being used
    /// </summary>
    public void OnCaptureCardChanged(string deviceName)
    {
        if (videoCaptureManager != null)
        {
            videoCaptureManager.SetCaptureCard(deviceName, settingsManager.CurrentSettings.resolution, settingsManager.CurrentSettings.fps);
            SaveSettings();
        }
    }

    /// <summary>
    /// Changes the resolution setting
    /// </summary>
    public void OnResolutionChanged(Vector2Int resolution)
    {
        if (videoCaptureManager != null)
        {
            videoCaptureManager.SetResolution(resolution);
            SaveSettings();
        }
    }

    /// <summary>
    /// Changes the FPS setting
    /// </summary>
    public void OnFPSChanged(int fps)
    {
        if (videoCaptureManager != null)
        {
            videoCaptureManager.SetFPS(fps);
            SaveSettings();
        }
    }

    /// <summary>
    /// Toggles between fullscreen and windowed mode
    /// </summary>
    public void ToggleFullscreen()
    {
        if (Screen.fullScreen)
        {
            // Switch to windowed mode (1280x720)
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
        else
        {
            // Switch to fullscreen (1920x1080)
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        }
    }

    #endregion

    #region Audio Controls

    /// <summary>
    /// Changes the audio input device
    /// </summary>
    public void OnAudioDeviceChanged(string deviceName)
    {
        if (audioCaptureManager != null)
        {
            audioCaptureManager.SetAudioDevice(deviceName);
            SaveSettings();
        }
    }

    /// <summary>
    /// Changes the volume by a delta amount
    /// </summary>
    public void ChangeVolume(float delta)
    {
        if (audioCaptureManager != null)
        {
            currentVolume = Mathf.Clamp01(currentVolume + delta);
            audioCaptureManager.SetVolume(currentVolume);
            
            // Update UI to show volume change
            if (uiManager != null)
            {
                uiManager.ShowVolumeIndicator(currentVolume);
            }
            
            SaveSettings();
        }
    }

    /// <summary>
    /// Sets the volume to a specific value
    /// </summary>
    public void SetVolume(float volume)
    {
        currentVolume = Mathf.Clamp01(volume);
        
        if (audioCaptureManager != null)
        {
            audioCaptureManager.SetVolume(currentVolume);
        }
        
        SaveSettings();
    }

    /// <summary>
    /// Toggles mute on/off
    /// </summary>
    public void ToggleMute()
    {
        if (audioCaptureManager != null)
        {
            audioCaptureManager.ToggleMute();
            
            // Update UI
            if (uiManager != null)
            {
                uiManager.ShowVolumeIndicator(audioCaptureManager.IsMuted ? 0f : currentVolume);
            }
        }
    }

    /// <summary>
    /// Restarts the audio (fixes fuzzy sound bug)
    /// Called automatically every 30 minutes
    /// </summary>
    private void RestartAudio()
    {
        if (audioCaptureManager != null)
        {
            audioCaptureManager.RestartAudio();
            Debug.Log("Audio restarted automatically");
        }
    }

    /// <summary>
    /// Manually triggers audio sync/restart
    /// </summary>
    public void ManualAudioSync()
    {
        if (audioCaptureManager != null)
        {
            audioCaptureManager.RestartAudio();
            Debug.Log("Audio synced manually");
            
            // Show notification to user
            if (uiManager != null)
            {
                uiManager.ShowVolumeIndicator(currentVolume);
                if (uiManager.volumeText != null)
                {
                    uiManager.volumeText.text = "Audio Synced";
                }
            }
        }
    }

    /// <summary>
    /// Starts auto audio sync with specified frequency in minutes
    /// </summary>
    public void StartAutoAudioSync(int frequencyMinutes)
    {
        StopAutoAudioSync();
        float frequencySeconds = frequencyMinutes * 60f;
        InvokeRepeating(nameof(RestartAudio), frequencySeconds, frequencySeconds);
        Debug.Log($"Auto audio sync started: every {frequencyMinutes} minutes");
    }

    /// <summary>
    /// Stops auto audio sync
    /// </summary>
    public void StopAutoAudioSync()
    {
        CancelInvoke(nameof(RestartAudio));
        Debug.Log("Auto audio sync stopped");
    }

    #endregion

    /// <summary>
    /// Called when the application quits
    /// Good place to clean up
    /// </summary>
    private void OnApplicationQuit()
    {
        // Save settings before quitting
        SaveSettings();
    }
}
