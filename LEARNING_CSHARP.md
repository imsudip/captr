# Learning C# Through This Project

## 🎓 C# Concepts Used in This Project

This project is designed to teach you C# fundamentals through practical application. Here's what you'll learn:

## 1. Classes and Objects

**What are they?**
- A **class** is a blueprint (like a recipe)
- An **object** is an instance of that class (the actual cake)

**Example from our code:**
```csharp
public class CaptureController : MonoBehaviour
{
    // This is a class definition
}

// In Unity, an object is created when you attach this script to a GameObject
```

## 2. Variables and Data Types

**Common types in our project:**

```csharp
// Numbers
int fps = 60;                    // Whole numbers
float volume = 0.5f;             // Decimal numbers
bool isPlaying = true;           // True or false

// Text
string deviceName = "Capture Card";

// Unity specific
Vector2Int resolution = new Vector2Int(1920, 1080);
GameObject settingsPanel;
```

**Access Modifiers:**
- `public` - Can be accessed from anywhere
- `private` - Only accessible within the class
- No modifier - Default is `private`

## 3. Functions (Methods)

Functions are reusable blocks of code.

**Anatomy of a function:**
```csharp
// access-modifier return-type FunctionName(parameters)
public void ShowSettings(bool show)
{
    // Code to execute
    settingsPanel.SetActive(show);
}
```

**Types of functions:**
```csharp
// Returns nothing
void PlayVideo() { }

// Returns a value
int GetFPS() { return 60; }

// Takes parameters
void SetVolume(float newVolume) { }

// Both
string GetDeviceName(int index) { return "Device"; }
```

## 4. Unity Lifecycle Methods

Special functions that Unity calls automatically:

```csharp
void Awake()    // Called first, before Start
void Start()    // Called once when script starts
void Update()   // Called every frame (60 times/second)
void OnDestroy() // Called when object is destroyed
```

**When to use each:**
- `Awake()` - Set up references
- `Start()` - Initialize values, start processes
- `Update()` - Handle continuous input, animations
- `OnDestroy()` - Clean up, stop processes

## 5. Conditionals (If Statements)

Making decisions in code:

```csharp
if (volume > 0.5f)
{
    // Volume is loud
}
else if (volume > 0.2f)
{
    // Volume is medium
}
else
{
    // Volume is quiet
}
```

**Comparison operators:**
- `==` Equal to
- `!=` Not equal to
- `>` Greater than
- `<` Less than
- `>=` Greater than or equal
- `<=` Less than or equal

**Logical operators:**
- `&&` AND (both must be true)
- `||` OR (either can be true)
- `!` NOT (opposite)

```csharp
if (volume > 0 && !isMuted)
{
    // Volume is up AND not muted
}
```

## 6. Switch Statements

Better than multiple if-else for checking one value:

```csharp
switch (index)
{
    case 0:
        fps = 30;
        break;
    case 1:
        fps = 60;
        break;
    case 2:
        fps = 120;
        break;
    default:
        fps = 60;
        break;
}
```

**Modern switch expression (C# 8.0+):**
```csharp
int fps = index switch
{
    0 => 30,
    1 => 60,
    2 => 120,
    _ => 60  // default
};
```

## 7. Lists and Arrays

Storing multiple values:

```csharp
// Array - fixed size
string[] devices = new string[3];
devices[0] = "Device 1";

// List - dynamic size (can grow)
List<string> deviceList = new List<string>();
deviceList.Add("Device 1");
deviceList.Add("Device 2");
```

**Common List operations:**
```csharp
list.Add(item);      // Add to end
list.Remove(item);   // Remove specific item
list.Count;          // Number of items
list.Clear();        // Remove all items
list[0];             // Access by index
```

## 8. Loops

Repeating code:

```csharp
// For loop - when you know how many times
for (int i = 0; i < 10; i++)
{
    Debug.Log(i); // Prints 0 to 9
}

// Foreach loop - for collections
foreach (var device in devices)
{
    Debug.Log(device.name);
}

// While loop - when condition based
while (isPlaying)
{
    // Keep doing something
}
```

## 9. Properties

Smart variables with get/set logic:

```csharp
// Auto-property (simple)
public int FPS { get; set; }

// Read-only property
public bool IsMuted => audioVolume == 0;

// Property with backing field
private float _volume;
public float Volume
{
    get { return _volume; }
    set { _volume = Mathf.Clamp01(value); } // Always between 0-1
}
```

## 10. Null Checking

Preventing errors when references aren't set:

```csharp
// Old way
if (audioSource != null)
{
    audioSource.Play();
}

// Null-conditional operator (modern)
audioSource?.Play();  // Only calls Play if not null

// Null-coalescing operator
string name = deviceName ?? "Default";  // Use "Default" if null
```

## 11. String Interpolation

Building strings with variables:

```csharp
// Old way
Debug.Log("FPS: " + fps + " Resolution: " + width + "x" + height);

// String interpolation (better!)
Debug.Log($"FPS: {fps} Resolution: {width}x{height}");

// With formatting
Debug.Log($"Volume: {volume:F2}%");  // 2 decimal places
```

## 12. Access Modifiers & Encapsulation

Controlling who can access what:

```csharp
public class Example
{
    public int publicVar;        // Anyone can access
    private int privateVar;      // Only this class
    protected int protectedVar;  // This class and children
    
    [SerializeField] 
    private int inspectorVar;    // Private but shows in Unity Inspector
}
```

## 13. Static vs Instance

```csharp
public class Utility
{
    // Static - shared by all, no instance needed
    public static int Add(int a, int b)
    {
        return a + b;
    }
    
    // Instance - needs an object
    public int Multiply(int a, int b)
    {
        return a * b;
    }
}

// Usage
int sum = Utility.Add(5, 3);           // Static - no object needed
Utility util = new Utility();          // Create instance
int product = util.Multiply(5, 3);     // Instance method
```

## 14. Enums

Named constants:

```csharp
public enum CaptureState
{
    Stopped,
    Playing,
    Paused
}

// Usage
CaptureState state = CaptureState.Playing;

if (state == CaptureState.Playing)
{
    // Do something
}
```

## 15. Regions

Organizing code:

```csharp
public class Example
{
    #region Variables
    private int value;
    #endregion
    
    #region Unity Lifecycle
    void Start() { }
    void Update() { }
    #endregion
    
    #region Public Methods
    public void DoSomething() { }
    #endregion
}
```

## 📚 Learning Path

**Beginner → Intermediate:**

1. ✅ Variables and types
2. ✅ Functions
3. ✅ If statements
4. ✅ Loops
5. ✅ Classes and objects
6. ⬜ Inheritance
7. ⬜ Interfaces
8. ⬜ Generics
9. ⬜ LINQ
10. ⬜ Async/Await

## 🎯 Practice Exercises

1. **Add a new resolution preset**
   - Modify `ResolutionUtility.cs`
   - Add 1600x900 resolution

2. **Add a new keyboard shortcut**
   - In `CaptureController.HandleKeyboardInput()`
   - Make 'P' pause/unpause video

3. **Create a new manager**
   - Make `PerformanceManager.cs`
   - Track FPS and display it

4. **Extend settings**
   - Add a "brightness" setting
   - Save/load it with other settings

## 💡 Tips for Learning

1. **Read the comments** - Every function explains what it does
2. **Experiment** - Change values, see what happens
3. **Use Debug.Log()** - Print values to understand flow
4. **Break things** - Best way to learn is fixing errors
5. **Ask questions** - Comment your confusion, research it

## 🔗 Resources

- [Microsoft C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [Unity Learn](https://learn.unity.com/)
- [C# Yellow Book](https://www.robmiles.com/c-yellow-book) (Free PDF)

---

**Remember:** Every expert was once a beginner. Take it one concept at a time!
