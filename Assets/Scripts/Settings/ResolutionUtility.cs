using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utility class for common resolution presets.
/// Makes it easy to work with standard resolutions throughout the app.
/// </summary>
public static class ResolutionPresets
{
    public static readonly Vector2Int HD_720 = new Vector2Int(1280, 720);
    public static readonly Vector2Int HD_1080 = new Vector2Int(1920, 1080);
    public static readonly Vector2Int QHD = new Vector2Int(2560, 1440);
    public static readonly Vector2Int UHD_4K = new Vector2Int(3840, 2160);

    /// <summary>
    /// Gets a list of all available resolution presets
    /// </summary>
    public static List<Vector2Int> GetAllPresets()
    {
        return new List<Vector2Int>
        {
            HD_720,
            HD_1080,
            QHD,
            UHD_4K
        };
    }

    /// <summary>
    /// Converts a resolution to a friendly string
    /// </summary>
    public static string ToString(Vector2Int resolution)
    {
        return $"{resolution.x} x {resolution.y}";
    }

    /// <summary>
    /// Parses a resolution string (e.g., "1920 x 1080") back to Vector2Int
    /// </summary>
    public static Vector2Int Parse(string resolutionString)
    {
        string[] parts = resolutionString.Split('x');
        if (parts.Length == 2)
        {
            if (int.TryParse(parts[0].Trim(), out int width) && 
                int.TryParse(parts[1].Trim(), out int height))
            {
                return new Vector2Int(width, height);
            }
        }
        
        // Default to HD 1080 if parsing fails
        return HD_1080;
    }
}

/// <summary>
/// Utility class for common FPS presets
/// </summary>
public static class FPSPresets
{
    public const int FPS_30 = 30;
    public const int FPS_60 = 60;
    public const int FPS_120 = 120;

    /// <summary>
    /// Gets a list of all available FPS presets
    /// </summary>
    public static List<int> GetAllPresets()
    {
        return new List<int> { FPS_30, FPS_60, FPS_120 };
    }

    /// <summary>
    /// Converts FPS to friendly string
    /// </summary>
    public static string ToString(int fps)
    {
        return $"{fps} fps";
    }

    /// <summary>
    /// Parses an FPS string (e.g., "60 fps") back to int
    /// </summary>
    public static int Parse(string fpsString)
    {
        string[] parts = fpsString.Split(' ');
        if (parts.Length > 0)
        {
            if (int.TryParse(parts[0].Trim(), out int fps))
            {
                return fps;
            }
        }
        
        // Default to 60 fps if parsing fails
        return FPS_60;
    }
}

/// <summary>
/// Extension methods for Vector2Int to make working with resolutions easier
/// </summary>
public static class Vector2IntExtensions
{
    /// <summary>
    /// Gets the aspect ratio of a resolution
    /// </summary>
    public static float GetAspectRatio(this Vector2Int resolution)
    {
        return resolution.y > 0 ? (float)resolution.x / resolution.y : 1f;
    }

    /// <summary>
    /// Checks if this is a 16:9 aspect ratio
    /// </summary>
    public static bool Is16By9(this Vector2Int resolution)
    {
        float aspectRatio = resolution.GetAspectRatio();
        return Mathf.Approximately(aspectRatio, 16f / 9f);
    }

    /// <summary>
    /// Checks if this is a 4:3 aspect ratio
    /// </summary>
    public static bool Is4By3(this Vector2Int resolution)
    {
        float aspectRatio = resolution.GetAspectRatio();
        return Mathf.Approximately(aspectRatio, 4f / 3f);
    }
}
