using UnityEngine;

/// <summary>
/// Manages audio capture from capture cards or microphones using Unity's Microphone API.
/// Handles audio playback, volume control, and automatic restart to prevent fuzzy sound.
/// </summary>
public class AudioCaptureManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("The AudioSource component that plays the captured audio")]
    public AudioSource audioSource;
    
    [Tooltip("Default volume level (0.0 to 1.0)")]
    [Range(0f, 1f)]
    public float defaultVolume = 0.5f;
    
    [Tooltip("Audio clip length in seconds (buffer size)")]
    public int clipLength = 10;

    // Private variables
    private string currentMicrophoneName; // Name of the current audio device
    private float volumeBeforeMute; // Stores volume when muted for unmuting
    private bool isMuted = false; // Tracks mute state

    /// <summary>
    /// Gets whether audio is currently muted
    /// </summary>
    public bool IsMuted => isMuted;

    /// <summary>
    /// Initialize audio source
    /// </summary>
    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.volume = defaultVolume;
        }
    }

    /// <summary>
    /// Returns list of all available audio input devices
    /// </summary>
    public string[] GetAvailableDevices()
    {
        return Microphone.devices;
    }

    /// <summary>
    /// Sets the audio input device to capture from
    /// </summary>
    /// <param name="deviceName">Name of the microphone/audio device</param>
    public void SetAudioDevice(string deviceName)
    {
        // Handle "No Audio Input" selection
        if (string.IsNullOrEmpty(deviceName) || deviceName == "No Audio Input")
        {
            StopAudio();
            currentMicrophoneName = null;
            Debug.Log("Audio input disabled");
            return;
        }

        // Stop any existing recording
        StopAudio();

        // Store device name
        currentMicrophoneName = deviceName;

        // Get the device capabilities (min and max frequency)
        int minFreq, maxFreq;
        Microphone.GetDeviceCaps(deviceName, out minFreq, out maxFreq);
        
        // Use half of max frequency for better quality
        // (Nyquist theorem - sample at 2x the highest frequency you want to capture)
        maxFreq = maxFreq / 2;

        Debug.Log($"Audio Device: {deviceName} | Max Frequency: {maxFreq} Hz");

        // Start recording from the microphone
        audioSource.clip = Microphone.Start(deviceName, true, clipLength, maxFreq);
        
        // Wait for microphone to start recording before playing
        // This prevents clicking/popping sounds
        while (Microphone.GetPosition(deviceName) <= 0) { }
        
        // Play the audio with a small delay for smooth playback
        audioSource.PlayDelayed(0.1f);

        Debug.Log($"Audio Capture Started: {deviceName}");
    }

    /// <summary>
    /// Stops audio capture and playback
    /// </summary>
    public void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (!string.IsNullOrEmpty(currentMicrophoneName) && Microphone.IsRecording(currentMicrophoneName))
        {
            Microphone.End(currentMicrophoneName);
        }

        Debug.Log("Audio capture stopped");
    }

    /// <summary>
    /// Sets the volume level
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
            
            // If we're setting a non-zero volume, unmute
            if (volume > 0f && isMuted)
            {
                isMuted = false;
            }
        }
    }

    /// <summary>
    /// Gets the current volume level
    /// </summary>
    public float GetVolume()
    {
        return audioSource != null ? audioSource.volume : 0f;
    }

    /// <summary>
    /// Toggles mute on/off
    /// </summary>
    public void ToggleMute()
    {
        if (audioSource == null) return;

        if (isMuted)
        {
            // Unmute - restore previous volume
            audioSource.volume = volumeBeforeMute > 0f ? volumeBeforeMute : defaultVolume;
            isMuted = false;
            Debug.Log("Audio unmuted");
        }
        else
        {
            // Mute - save current volume and set to 0
            volumeBeforeMute = audioSource.volume;
            audioSource.volume = 0f;
            isMuted = true;
            Debug.Log("Audio muted");
        }
    }

    /// <summary>
    /// Mutes the audio
    /// </summary>
    public void Mute()
    {
        if (!isMuted)
        {
            ToggleMute();
        }
    }

    /// <summary>
    /// Unmutes the audio
    /// </summary>
    public void Unmute()
    {
        if (isMuted)
        {
            ToggleMute();
        }
    }

    /// <summary>
    /// Restarts the audio capture
    /// This fixes the "fuzzy sound" bug that can occur after extended use
    /// Called automatically every 30 minutes by CaptureController
    /// </summary>
    public void RestartAudio()
    {
        if (string.IsNullOrEmpty(currentMicrophoneName)) return;

        // Only restart if currently recording
        if (Microphone.IsRecording(currentMicrophoneName))
        {
            string deviceName = currentMicrophoneName;
            float currentVolume = audioSource.volume;
            
            // Stop current recording
            StopAudio();
            
            // Start again with same device
            SetAudioDevice(deviceName);
            
            // Restore volume
            audioSource.volume = currentVolume;
            
            Debug.Log("Audio restarted successfully");
        }
    }

    /// <summary>
    /// Gets the current audio device name
    /// </summary>
    public string GetCurrentDeviceName()
    {
        return currentMicrophoneName;
    }

    /// <summary>
    /// Checks if audio is currently capturing
    /// </summary>
    public bool IsCapturing()
    {
        return !string.IsNullOrEmpty(currentMicrophoneName) && 
               Microphone.IsRecording(currentMicrophoneName);
    }

    /// <summary>
    /// Clean up when destroyed
    /// </summary>
    private void OnDestroy()
    {
        StopAudio();
    }

    /// <summary>
    /// Stop recording when application is paused/minimized
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // Pause audio when app is minimized
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
        else
        {
            // Resume audio when app is restored
            if (audioSource != null && !audioSource.isPlaying && audioSource.clip != null)
            {
                audioSource.UnPause();
            }
        }
    }
}
