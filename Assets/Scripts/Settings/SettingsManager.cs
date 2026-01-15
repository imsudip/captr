using System;
using UnityEngine;

/// <summary>
/// Data class that holds all application settings.
/// This is serializable so it can be saved to JSON.
/// </summary>
[Serializable]
public class CaptureSettings
{
    // Video settings
    public string captureCardName = "";
    public Vector2Int resolution = new Vector2Int(1920, 1080);
    public int fps = 60;
    public int aspectRatioMode = 0; // 0=None, 1=WidthControlsHeight, 2=HeightControlsWidth, 3=FitInParent, 4=EnvelopeParent

    // Audio settings
    public string audioDeviceName = "";
    public float volume = 0.5f;
    public bool audioSyncEnabled = false;
    public int audioSyncFrequencyMinutes = 5; // Default 5 minutes

    // Window settings
    public bool fullscreen = false;
    public Vector2Int windowSize = new Vector2Int(1280, 720);

    /// <summary>
    /// Creates a default settings object
    /// </summary>
    public static CaptureSettings CreateDefault()
    {
        return new CaptureSettings
        {
            captureCardName = "No Capture Card",
            resolution = new Vector2Int(1920, 1080),
            fps = 60,
            aspectRatioMode = 3, // FitInParent
            audioDeviceName = "No Audio Input",
            volume = 0.5f,
            audioSyncEnabled = false,
            audioSyncFrequencyMinutes = 5,
            fullscreen = false,
            windowSize = new Vector2Int(1280, 720)
        };
    }
}

/// <summary>
/// Manages saving and loading of application settings to/from disk.
/// Settings are stored as JSON in the Application.persistentDataPath.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Current application settings")]
    public CaptureSettings currentSettings;

    // File path for saving settings
    private string settingsFilePath;
    private const string SETTINGS_FILENAME = "CaptureSettings.json";

    /// <summary>
    /// Public property to access current settings
    /// </summary>
    public CaptureSettings CurrentSettings => currentSettings;

    /// <summary>
    /// Initialize settings
    /// </summary>
    private void Awake()
    {
        // Set the file path for settings
        settingsFilePath = System.IO.Path.Combine(Application.persistentDataPath, SETTINGS_FILENAME);
        
        Debug.Log($"Settings file location: {settingsFilePath}");

        // Load existing settings or create default
        LoadSettings();
    }

    /// <summary>
    /// Loads settings from disk. If no file exists, creates default settings.
    /// </summary>
    public CaptureSettings LoadSettings()
    {
        try
        {
            // Check if settings file exists
            if (System.IO.File.Exists(settingsFilePath))
            {
                // Read JSON from file
                string json = System.IO.File.ReadAllText(settingsFilePath);
                
                // Deserialize JSON to settings object
                currentSettings = JsonUtility.FromJson<CaptureSettings>(json);
                
                Debug.Log("Settings loaded successfully");
                return currentSettings;
            }
            else
            {
                // No saved settings, create default
                Debug.Log("No saved settings found, creating default settings");
                currentSettings = CaptureSettings.CreateDefault();
                
                // Save default settings
                SaveSettings();
                
                return currentSettings;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading settings: {e.Message}");
            
            // On error, use default settings
            currentSettings = CaptureSettings.CreateDefault();
            return currentSettings;
        }
    }

    /// <summary>
    /// Saves current settings to disk as JSON
    /// </summary>
    public void SaveSettings()
    {
        try
        {
            // Convert settings to JSON
            string json = JsonUtility.ToJson(currentSettings, true); // true = pretty print
            
            // Write to file
            System.IO.File.WriteAllText(settingsFilePath, json);
            
            Debug.Log("Settings saved successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving settings: {e.Message}");
        }
    }

    /// <summary>
    /// Updates a specific setting and saves
    /// </summary>
    public void UpdateCaptureCard(string deviceName, Vector2Int resolution, int fps)
    {
        currentSettings.captureCardName = deviceName;
        currentSettings.resolution = resolution;
        currentSettings.fps = fps;
        SaveSettings();
    }

    /// <summary>
    /// Updates audio device and saves
    /// </summary>
    public void UpdateAudioDevice(string deviceName)
    {
        currentSettings.audioDeviceName = deviceName;
        SaveSettings();
    }

    /// <summary>
    /// Updates volume and saves
    /// </summary>
    public void UpdateVolume(float volume)
    {
        currentSettings.volume = Mathf.Clamp01(volume);
        SaveSettings();
    }

    /// <summary>
    /// Updates resolution and saves
    /// </summary>
    public void UpdateResolution(Vector2Int resolution)
    {
        currentSettings.resolution = resolution;
        SaveSettings();
    }

    /// <summary>
    /// Updates FPS and saves
    /// </summary>
    public void UpdateFPS(int fps)
    {
        currentSettings.fps = fps;
        SaveSettings();
    }

    /// <summary>
    /// Updates aspect ratio mode and saves
    /// </summary>
    public void UpdateAspectRatioMode(int mode)
    {
        currentSettings.aspectRatioMode = mode;
        SaveSettings();
    }

    /// <summary>
    /// Updates audio sync enabled state and saves
    /// </summary>
    public void UpdateAudioSyncEnabled(bool enabled)
    {
        currentSettings.audioSyncEnabled = enabled;
        SaveSettings();
    }

    /// <summary>
    /// Updates audio sync frequency and saves
    /// </summary>
    public void UpdateAudioSyncFrequency(int frequencyMinutes)
    {
        currentSettings.audioSyncFrequencyMinutes = frequencyMinutes;
        SaveSettings();
    }

    /// <summary>
    /// Updates fullscreen setting and saves
    /// </summary>
    public void UpdateFullscreen(bool fullscreen)
    {
        currentSettings.fullscreen = fullscreen;
        SaveSettings();
    }

    /// <summary>
    /// Resets settings to default
    /// </summary>
    public void ResetToDefaults()
    {
        currentSettings = CaptureSettings.CreateDefault();
        SaveSettings();
        Debug.Log("Settings reset to defaults");
    }

    /// <summary>
    /// Deletes the settings file (useful for debugging)
    /// </summary>
    public void DeleteSettingsFile()
    {
        try
        {
            if (System.IO.File.Exists(settingsFilePath))
            {
                System.IO.File.Delete(settingsFilePath);
                Debug.Log("Settings file deleted");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deleting settings file: {e.Message}");
        }
    }
}
