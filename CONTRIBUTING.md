# Contributing to Video Game Capture - Modern UI

Thank you for your interest in contributing! This document provides guidelines and instructions for contributing to the project.

## 🤝 How to Contribute

### Reporting Bugs

If you find a bug, please create an issue with:

- Clear description of the problem
- Steps to reproduce
- Expected vs actual behavior
- Unity version and OS
- Screenshots/logs if applicable

### Suggesting Features

Feature requests are welcome! Please include:

- Clear description of the feature
- Use case and benefits
- Potential implementation approach (if known)

### Submitting Pull Requests

1. **Fork the repository**

   ```bash
   git clone https://github.com/yourusername/Captr.git
   cd Captr
   ```

2. **Create a feature branch**

   ```bash
   git checkout -b feature/YourFeatureName
   ```

3. **Make your changes**

   - Write clean, documented code
   - Follow existing code style
   - Test thoroughly

4. **Commit your changes**

   ```bash
   git add .
   git commit -m "Add: Brief description of changes"
   ```

5. **Push to your fork**

   ```bash
   git push origin feature/YourFeatureName
   ```

6. **Create Pull Request**
   - Go to the original repository
   - Click "New Pull Request"
   - Select your branch
   - Describe your changes

## 📝 Code Style Guidelines

### C# Conventions

- Use PascalCase for public members and methods
- Use camelCase for private fields
- Use meaningful variable names
- Add XML documentation comments for public APIs
- Keep methods focused and concise

### Example:

```csharp
/// <summary>
/// Sets the capture device and configures video settings
/// </summary>
/// <param name="deviceName">Name of the capture device</param>
/// <param name="resolution">Desired resolution</param>
public void SetCaptureCard(string deviceName, Vector2Int resolution)
{
    // Implementation
}
```

### UI Toolkit (UXML/USS)

- Use kebab-case for class names
- Group related styles together
- Use CSS variables for colors
- Comment complex selectors

### Example:

```css
/* Main Panel Container */
.settings-panel {
  background-color: var(--bg-primary);
  padding: 20px;
}
```

## 🧪 Testing

Before submitting a PR:

- Test with different capture cards (if possible)
- Test all keyboard shortcuts
- Verify settings persistence
- Check for console errors
- Test build process

## 📁 Project Structure

Understanding the codebase:

```
Assets/
├── Scripts/
│   ├── Core/           # Main controllers
│   ├── Managers/       # Feature managers
│   ├── Settings/       # Settings/persistence
│   └── UI/             # UI management
└── UI/                 # UXML/USS files
```

## 🎯 Priority Areas

We especially welcome contributions in:

- Cross-platform compatibility (Mac/Linux)
- Performance optimizations
- UI/UX improvements
- Documentation improvements
- Bug fixes
- Accessibility features

## ❓ Questions?

- Open a Discussion
- Check existing Issues
- Review the README.md

## 📜 Code of Conduct

- Be respectful and constructive
- Welcome newcomers
- Focus on the code, not the person
- Help others learn

Thank you for contributing! 🎉
