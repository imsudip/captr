# Captr

A professional Unity-based capture card software with modern dark UI, designed for seamless video and audio capture from external devices.

> 💡 **Related Project**: This is a modern UI rewrite. For the original project, see [VideoGameCapture](https://github.com/ImmerNochNoah/VideoGameCapture) by ImmerNochNoah.

![Unity Version](https://img.shields.io/badge/Unity-2022.3.62f3-black)

## ✨ Features

### 🎥 Video Capture

- Real-time capture using Unity's WebCamTexture API
- Multiple resolution support (1080p, 720p, 1440p, 4K)
- Frame rate options (30/60/120 FPS)
- Aspect ratio control with multiple fit modes
- Real-time FPS counter display

### 🎵 Audio Management

- Real-time audio capture from capture cards
- Volume control with keyboard shortcuts (↑/↓)
- Mute toggle (M key)
- Auto audio sync with configurable intervals (1-15 minutes)
- Manual audio sync (R key) to fix audio drift instantly

### 🎨 Modern UI

- Shadcn/ui-inspired dark theme with zinc color palette
- Smooth 300ms slide animations for panels
- Left-side settings panel
- Right-side keyboard shortcuts reference panel
- Persistent settings saved to JSON
- Clean, professional interface

### ⌨️ Keyboard Shortcuts

| Key   | Action                |
| ----- | --------------------- |
| `ESC` | Toggle Settings Panel |
| `F5`  | Toggle Fullscreen     |
| `↑`   | Volume Up (+10%)      |
| `↓`   | Volume Down (-10%)    |
| `M`   | Mute/Unmute Audio     |
| `R`   | Manual Audio Sync     |

## 📦 Installation

### Prerequisites

- Unity 2022.3.62f3 or later
- Capture card device (or webcam for testing)
- Windows OS

### Setup Steps

1. Clone the repository:

```bash
git clone https://github.com/imsudip/Captr.git
cd Captr
```

2. Open the project in Unity 2022.3.62f3+

3. Open the main scene: `Assets/Scenes/MainScene.unity`

4. Configure your capture devices:
   - Press `ESC` to open settings
   - Select your capture card and audio device
   - Adjust resolution and FPS as needed
   - Click "Save Preferences"

## 🏗️ Project Structure

```
Assets/
├── Resources/              # Runtime-loaded assets
│   ├── logo_zoom.png
│   ├── save.png
│   ├── keyboard.png
│   └── Please Select a Video Source.png
├── Scenes/
│   └── MainScene.unity
├── Scripts/
│   ├── Core/
│   │   └── CaptureController.cs
│   ├── Managers/
│   │   ├── VideoCaptureManager.cs
│   │   ├── AudioCaptureManager.cs
│   │   └── SettingsManager.cs
│   └── UI/
│       └── UIManagerToolkit.cs
└── UI/                     # UI Toolkit files
    ├── SettingsPanel.uxml/.uss
    ├── KeymapsPanel.uxml/.uss
    ├── VolumeIndicator.uxml/.uss
    └── Common.uss
```

## 🚀 Building

### Create Executable

1. **File → Build Settings**
2. Select **Windows, Mac, Linux**
3. Choose **Windows** as Target Platform
4. Set Architecture to **x86_64**
5. Click **Build** and select output folder

### Create Installer (Optional)

Use [Inno Setup](https://jrsoftware.org/isinfo.php) to create a Windows installer from your built executable.

## ⚙️ Configuration

Settings are automatically saved to `Application.persistentDataPath/CaptureSettings.json`:

```json
{
  "captureCardName": "Your Capture Card",
  "resolution": { "x": 1920, "y": 1080 },
  "fps": 60,
  "aspectRatioMode": 3,
  "audioDeviceName": "Your Audio Device",
  "volume": 0.5,
  "audioSyncEnabled": true,
  "audioSyncFrequencyMinutes": 5
}
```

## 🔧 Troubleshooting

**Audio becomes fuzzy after extended use**

- Enable "Auto Audio Sync" in settings
- Set sync frequency to 5-15 minutes
- Use R key to manually sync anytime

**Capture card not detected**

- Ensure device is connected before launching
- Check Windows Device Manager
- Try restarting the application

**Performance issues**

- Lower capture resolution
- Reduce target FPS
- Close background applications

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/NewFeature`)
3. Commit changes (`git commit -m 'Add NewFeature'`)
4. Push to branch (`git push origin feature/NewFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

---

**Made for content creators and streamers** 🎮✨
