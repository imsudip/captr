using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages video capture from capture cards using Unity's WebCamTexture.
/// WebCamTexture is FAST and works with most capture cards - that's why we use Unity!
/// 
/// Learn more: https://docs.unity3d.com/ScriptReference/WebCamTexture.html
/// </summary>
public class VideoCaptureManager : MonoBehaviour
{
    [Header("Display Settings")]
    [Tooltip("The UI element that will display the video feed")]
    public RawImage videoDisplay;
    
    [Tooltip("Image to show when no capture card is connected")]
    public Texture2D noSignalTexture;

    [Header("Capture Settings")]
    [Tooltip("Default resolution for capture")]
    public Vector2Int defaultResolution = new Vector2Int(1920, 1080);
    
    [Tooltip("Default frames per second")]
    public int defaultFPS = 60;

    [Header("Aspect Ratio")]
    [Tooltip("Controls the aspect ratio of the display")]
    public AspectRatioFitter aspectRatioFitter;

    // Private variables
    private WebCamTexture currentWebCamTexture; // The active camera texture
    private string currentDeviceName; // Name of the current capture card
    private Vector2Int currentResolution; // Current resolution
    private int currentFPS; // Current FPS (configured/requested)
    
    // Actual FPS tracking
    private float actualFPS; // Calculated actual rendering FPS
    private float fpsUpdateInterval = 0.5f; // Update FPS every 0.5 seconds
    private float fpsAccumulator = 0.0f;
    private int fpsFrameCount = 0;
    private float fpsTimeLeft;

    /// <summary>
    /// Returns list of all available capture devices (webcams and capture cards)
    /// </summary>
    public WebCamDevice[] GetAvailableDevices()
    {
        return WebCamTexture.devices;
    }

    /// <summary>
    /// Checks if a capture card is currently active
    /// </summary>
    public bool IsCapturing()
    {
        return currentWebCamTexture != null && currentWebCamTexture.isPlaying;
    }

    /// <summary>
    /// Sets the capture card to use with specified settings
    /// </summary>
    /// <param name="deviceName">Name of the capture card/webcam</param>
    /// <param name="resolution">Resolution to capture at</param>
    /// <param name="fps">Frames per second</param>
    public void SetCaptureCard(string deviceName, Vector2Int resolution, int fps)
    {
        // Handle "No Capture Card" selection
        if (string.IsNullOrEmpty(deviceName) || deviceName == "No Capture Card")
        {
            StopCapture();
            ShowNoSignal();
            return;
        }

        // Store settings
        currentDeviceName = deviceName;
        currentResolution = resolution.x > 0 ? resolution : defaultResolution;
        currentFPS = fps > 0 ? fps : defaultFPS;

        // Stop any existing capture
        if (currentWebCamTexture != null && currentWebCamTexture.isPlaying)
        {
            currentWebCamTexture.Stop();
        }

        // Create new WebCamTexture with specified settings
        currentWebCamTexture = new WebCamTexture(
            deviceName,
            currentResolution.x,
            currentResolution.y,
            currentFPS
        );

        // Apply texture to display
        if (videoDisplay != null)
        {
            videoDisplay.texture = currentWebCamTexture;
        }

        // Start capturing
        currentWebCamTexture.Play();

        // Update aspect ratio to match video
        UpdateAspectRatio();

        Debug.Log($"Video Capture Started: {deviceName} @ {currentResolution.x}x{currentResolution.y} {currentFPS}fps");
    }

    /// <summary>
    /// Overload for setting capture card with default settings
    /// </summary>
    public void SetCaptureCard(string deviceName)
    {
        SetCaptureCard(deviceName, defaultResolution, defaultFPS);
    }

    /// <summary>
    /// Changes the resolution of the current capture
    /// </summary>
    public void SetResolution(Vector2Int resolution)
    {
        if (!string.IsNullOrEmpty(currentDeviceName))
        {
            currentResolution = resolution;
            // Restart capture with new resolution
            SetCaptureCard(currentDeviceName, currentResolution, currentFPS);
        }
    }

    /// <summary>
    /// Changes the FPS of the current capture
    /// </summary>
    public void SetFPS(int fps)
    {
        if (!string.IsNullOrEmpty(currentDeviceName))
        {
            currentFPS = fps;
            // Restart capture with new FPS
            SetCaptureCard(currentDeviceName, currentResolution, currentFPS);
        }
    }

    /// <summary>
    /// Sets the aspect ratio mode
    /// </summary>
    /// <param name="mode">AspectRatioFitter.AspectMode to use</param>
    public void SetAspectRatioMode(AspectRatioFitter.AspectMode mode)
    {
        if (aspectRatioFitter != null)
        {
            aspectRatioFitter.aspectMode = mode;
            
            // If using FitInParent or EnvelopeParent, calculate aspect ratio
            if (mode == AspectRatioFitter.AspectMode.FitInParent || 
                mode == AspectRatioFitter.AspectMode.EnvelopeParent)
            {
                UpdateAspectRatio();
            }
        }
    }

    /// <summary>
    /// Updates the aspect ratio based on current video dimensions
    /// </summary>
    private void UpdateAspectRatio()
    {
        if (aspectRatioFitter != null && currentWebCamTexture != null)
        {
            float aspectRatio = (float)currentWebCamTexture.width / currentWebCamTexture.height;
            aspectRatioFitter.aspectRatio = aspectRatio;
            
            Debug.Log($"Aspect Ratio Updated: {aspectRatio:F2} ({currentWebCamTexture.width}x{currentWebCamTexture.height})");
        }
    }

    /// <summary>
    /// Stops the current capture
    /// </summary>
    public void StopCapture()
    {
        if (currentWebCamTexture != null && currentWebCamTexture.isPlaying)
        {
            currentWebCamTexture.Stop();
            Debug.Log("Video capture stopped");
        }
    }

    /// <summary>
    /// Shows the "No Signal" texture
    /// </summary>
    private void ShowNoSignal()
    {
        if (videoDisplay != null && noSignalTexture != null)
        {
            videoDisplay.texture = noSignalTexture;
            
            // Set aspect ratio for the no-signal image (16:9)
            if (aspectRatioFitter != null)
            {
                float aspectRatio = 1.77f; // 16:9
                aspectRatioFitter.aspectRatio = aspectRatio;
                Debug.Log($"No Signal - Aspect Ratio: {aspectRatio:F2} ({noSignalTexture.width}x{noSignalTexture.height})");
            }
        }
    }

    /// <summary>
    /// Gets the current capture device name
    /// </summary>
    public string GetCurrentDeviceName()
    {
        return currentDeviceName;
    }

    /// <summary>
    /// Gets the current resolution
    /// </summary>
    public Vector2Int GetCurrentResolution()
    {
        return currentResolution;
    }

    /// <summary>
    /// Gets the configured/requested FPS
    /// </summary>
    public int GetCurrentFPS()
    {
        return currentFPS;
    }
    
    /// <summary>
    /// Gets the actual rendering FPS (calculated from frame times)
    /// </summary>
    public float GetActualFPS()
    {
        return actualFPS;
    }
    
    /// <summary>
    /// Update called every frame to calculate actual FPS
    /// </summary>
    private void Update()
    {
        // Calculate actual FPS
        fpsTimeLeft -= Time.deltaTime;
        fpsAccumulator += Time.timeScale / Time.deltaTime;
        fpsFrameCount++;
        
        // Update FPS display every interval
        if (fpsTimeLeft <= 0.0f)
        {
            actualFPS = fpsAccumulator / fpsFrameCount;
            fpsTimeLeft = fpsUpdateInterval;
            fpsAccumulator = 0.0f;
            fpsFrameCount = 0;
        }
    }

    /// <summary>
    /// Clean up when destroyed
    /// </summary>
    private void OnDestroy()
    {
        StopCapture();
    }
}
