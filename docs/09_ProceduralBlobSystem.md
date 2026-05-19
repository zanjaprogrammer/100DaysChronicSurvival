# Procedural Blob System - Neon Slime Cells

## 🎨 Overview
Sistem visual procedural untuk generate cells dengan efek neon dan physics slime yang dynamic. Tidak perlu sprite AI-generated, semua dibuat dengan code!

---

## ✨ Features

### 1. **Procedural Mesh Generation**
- Generate blob shapes dengan Perlin noise
- Organic, irregular shapes
- Customizable segments (detail level)
- Real-time mesh deformation

### 2. **Neon Glow Effect**
- Custom shader dengan Fresnel effect
- Pulsing glow animation
- Dual-layer rendering (core + glow)
- Customizable colors dan intensity

### 3. **Slime Physics**
- Jiggle effect (wobble saat bergerak)
- Squash & stretch (deformasi saat bergerak)
- Spring-based physics
- Responsive to movement

### 4. **Trail Effects**
- Particle trail saat bergerak
- Fade out animation
- Velocity-based emission
- Customizable colors

---

## 🛠️ Components

### **BlobGenerator.cs**
Main component untuk generate blob mesh dan animasi.

**Key Features**:
- Procedural mesh generation
- Perlin noise untuk organic shapes
- Animated vertices (breathing, wobbling)
- Preset configurations untuk different cell types
- Dual-layer rendering (core + glow)

**Inspector Settings**:
```
Blob Shape Settings:
- Segments: 32 (detail level)
- Base Radius: 0.5
- Radius Variation: 0.15
- Noise Scale: 2.0
- Noise Speed: 1.0

Neon Glow Settings:
- Core Color: Blue
- Glow Color: Light Blue
- Glow Intensity: 2.0
- Glow Size: 1.3

Animation Settings:
- Animate: true
- Pulse Speed: 1.0
- Pulse Amount: 0.1
- Wobble Speed: 0.5
```

**Presets Available**:
- `Macrophage`: Large, slow, deep blue
- `TCell`: Medium, fast, bright blue
- `BCell`: Small, medium speed, light blue
- `NKCell`: Medium, fast, blue-purple
- `Neutrophil`: Small, very fast, cyan
- `SugarBlob`: Small, slow, golden
- `PressurePulse`: Small, very fast, red
- `DamagedCell`: Small, fast, purple

**Usage**:
```csharp
// Create blob
GameObject blobObj = new GameObject("ImmuneCell");
BlobGenerator blob = blobObj.AddComponent<BlobGenerator>();

// Apply preset
blob.ApplyPreset(BlobPreset.Macrophage);

// Or customize manually
blob.SetColors(Color.blue, Color.cyan);
blob.SetSize(0.6f);
blob.SetAnimationSpeed(1.0f);
```

---

### **SlimeBlobPhysics.cs**
Adds slime-like physics untuk dynamic movement.

**Key Features**:
- Jiggle effect (spring physics)
- Squash & stretch deformation
- Movement-responsive
- Velocity-based animation

**Inspector Settings**:
```
Jiggle Settings:
- Jiggle Amount: 0.1
- Jiggle Speed: 5.0
- Jiggle Damping: 0.8

Squash & Stretch:
- Enable: true
- Amount: 0.2
- Speed: 10.0

Movement Response:
- Movement Influence: 1.0
- Rotation Influence: 0.5
```

**Usage**:
```csharp
// Add physics to blob
SlimeBlobPhysics physics = blobObj.AddComponent<SlimeBlobPhysics>();

// Add impulse (for hit reactions)
physics.AddImpulse(new Vector2(1, 0));

// Customize
physics.SetJiggleAmount(0.2f);
physics.SetSquashStretchAmount(0.3f);
```

---

### **BlobTrailEffect.cs**
Creates particle trail untuk moving blobs.

**Key Features**:
- Velocity-based particle emission
- Fade out animation
- Customizable colors
- Performance-optimized

**Inspector Settings**:
```
Trail Settings:
- Enable Trail: true
- Trail Color: Light Blue (transparent)
- Trail Lifetime: 0.5s
- Emission Rate: 20 particles/sec
- Min Velocity: 0.5 (threshold)

Particle Settings:
- Particle Size: 0.2
- Size Variation: 0.1
- Size Over Lifetime: Curve (1 → 0)
```

**Usage**:
```csharp
// Add trail effect
BlobTrailEffect trail = blobObj.AddComponent<BlobTrailEffect>();

// Customize
trail.SetTrailColor(Color.cyan);
trail.SetEmissionRate(30f);
trail.SetTrailEnabled(true);
```

---

### **NeonBlob.shader**
Custom shader untuk neon glow effect.

**Features**:
- Fresnel-based rim lighting
- Pulsing animation
- Soft edges (alpha falloff)
- Center glow
- URP compatible

**Shader Properties**:
```
_MainColor: Core color
_GlowColor: Glow/rim color
_GlowIntensity: Glow brightness (0-5)
_FresnelPower: Rim sharpness (0.1-5)
_PulseSpeed: Animation speed (0-5)
_PulseAmount: Pulse intensity (0-1)
```

**Material Setup**:
```csharp
// Create material
Material neonMat = new Material(Shader.Find("Custom/NeonBlob"));
neonMat.SetColor("_MainColor", Color.blue);
neonMat.SetColor("_GlowColor", Color.cyan);
neonMat.SetFloat("_GlowIntensity", 2f);
neonMat.SetFloat("_FresnelPower", 2f);
```

---

## 🎮 How to Use

### **Quick Setup (Prefab-style)**

1. **Create Immune Cell**:
```csharp
public GameObject CreateImmuneCell(BlobPreset preset, Vector3 position)
{
    // Create GameObject
    GameObject cell = new GameObject($"Cell_{preset}");
    cell.transform.position = position;
    
    // Add blob generator
    BlobGenerator blob = cell.AddComponent<BlobGenerator>();
    blob.ApplyPreset(preset);
    
    // Add physics
    SlimeBlobPhysics physics = cell.AddComponent<SlimeBlobPhysics>();
    
    // Add trail
    BlobTrailEffect trail = cell.AddComponent<BlobTrailEffect>();
    trail.SetTrailColor(blob.glowColor);
    
    return cell;
}
```

2. **Spawn in Scene**:
```csharp
void Start()
{
    // Spawn Macrophage
    CreateImmuneCell(BlobPreset.Macrophage, new Vector3(0, 0, 0));
    
    // Spawn T-Cell
    CreateImmuneCell(BlobPreset.TCell, new Vector3(2, 0, 0));
    
    // Spawn enemy
    CreateImmuneCell(BlobPreset.SugarBlob, new Vector3(-2, 0, 0));
}
```

---

### **Advanced Customization**

**Custom Colors**:
```csharp
BlobGenerator blob = GetComponent<BlobGenerator>();

// Custom immune cell (green)
blob.SetColors(
    new Color(0.2f, 0.8f, 0.3f, 1f),  // Core: Green
    new Color(0.4f, 1f, 0.5f, 0.5f)   // Glow: Light green
);
```

**Custom Animation**:
```csharp
// Slow, calm blob
blob.SetAnimationSpeed(0.3f);

// Fast, energetic blob
blob.SetAnimationSpeed(2.5f);
```

**Custom Physics**:
```csharp
SlimeBlobPhysics physics = GetComponent<SlimeBlobPhysics>();

// Very jiggly (like jelly)
physics.SetJiggleAmount(0.5f);

// Stiff (like solid)
physics.SetJiggleAmount(0.05f);
```

---

## 🎨 Visual Presets

### **Immune Cells (Blue Tones)**

**Macrophage** 🛡️:
- Color: Deep Blue (#2E5EAA)
- Glow: Light Blue (#4A90E2)
- Size: Large (0.6)
- Speed: Slow (0.5)
- Feel: Tanky, protective

**T-Cell** ⚔️:
- Color: Bright Blue (#4A90E2)
- Glow: Lighter Blue (#7CB9E8)
- Size: Medium (0.45)
- Speed: Fast (1.2)
- Feel: Aggressive, sharp

**B-Cell** 🏹:
- Color: Light Blue (#7CB9E8)
- Glow: Very Light Blue (#AADDFF)
- Size: Medium (0.4)
- Speed: Medium (0.8)
- Feel: Ranged, supportive

**NK Cell** 💀:
- Color: Blue-Purple (#6B5B95)
- Glow: Purple (#9E8DB5)
- Size: Medium (0.5)
- Speed: Very Fast (1.5)
- Feel: Deadly, specialized

**Neutrophil** ⚡:
- Color: Cyan (#00CED1)
- Glow: Light Cyan (#66E8E9)
- Size: Small (0.35)
- Speed: Very Fast (2.0)
- Feel: Quick, responsive

---

### **Disease Enemies**

**Sugar Blob** 🍬 (Diabetes):
- Color: Golden Amber (#FFB347)
- Glow: Light Gold (#FFD78A)
- Size: Small (0.3)
- Speed: Slow (0.6)
- Feel: Sticky, sweet

**Pressure Pulse** 💥 (Hypertension):
- Color: Bright Red (#FF0000)
- Glow: Pink (#FF6969)
- Size: Small (0.35)
- Speed: Very Fast (2.5)
- Feel: Explosive, dangerous

**Damaged Cell** 🦠 (Cancer):
- Color: Light Purple (#9370DB)
- Glow: Purple (#C49EF6)
- Size: Small (0.35)
- Speed: Fast (1.8)
- Feel: Unstable, mutating

---

## 🔧 Performance Optimization

### **LOD System** (Optional)
```csharp
public class BlobLOD : MonoBehaviour
{
    [SerializeField] private float highDetailDistance = 10f;
    [SerializeField] private float mediumDetailDistance = 20f;
    
    private BlobGenerator blob;
    private Camera mainCamera;
    
    void Start()
    {
        blob = GetComponent<BlobGenerator>();
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        
        if (distance < highDetailDistance)
        {
            blob.segments = 32; // High detail
        }
        else if (distance < mediumDetailDistance)
        {
            blob.segments = 16; // Medium detail
        }
        else
        {
            blob.segments = 8; // Low detail
        }
    }
}
```

### **Object Pooling**
```csharp
public class BlobPool : MonoBehaviour
{
    [SerializeField] private GameObject blobPrefab;
    [SerializeField] private int poolSize = 50;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    
    void Start()
    {
        // Pre-instantiate blobs
        for (int i = 0; i < poolSize; i++)
        {
            GameObject blob = Instantiate(blobPrefab);
            blob.SetActive(false);
            pool.Enqueue(blob);
        }
    }
    
    public GameObject GetBlob()
    {
        if (pool.Count > 0)
        {
            GameObject blob = pool.Dequeue();
            blob.SetActive(true);
            return blob;
        }
        return Instantiate(blobPrefab);
    }
    
    public void ReturnBlob(GameObject blob)
    {
        blob.SetActive(false);
        pool.Enqueue(blob);
    }
}
```

---

## 🎯 Integration with Game Systems

### **Unit System Integration**
```csharp
public class ImmuneCell : MonoBehaviour
{
    private BlobGenerator blob;
    private SlimeBlobPhysics physics;
    
    void Start()
    {
        blob = GetComponent<BlobGenerator>();
        physics = GetComponent<SlimeBlobPhysics>();
    }
    
    // Called when unit takes damage
    public void OnDamage(float damage)
    {
        // Visual feedback
        physics.AddImpulse(Random.insideUnitCircle * damage * 0.1f);
        
        // Flash effect
        StartCoroutine(DamageFlash());
    }
    
    IEnumerator DamageFlash()
    {
        Color original = blob.coreColor;
        blob.SetColors(Color.red, Color.white);
        yield return new WaitForSeconds(0.1f);
        blob.SetColors(original, blob.glowColor);
    }
}
```

### **Disease Progression Visual**
```csharp
public void UpdateDiseaseVisual(float progression)
{
    BlobGenerator blob = GetComponent<BlobGenerator>();
    
    // Change color based on disease progression
    Color healthyColor = new Color(0.2f, 0.5f, 1f, 1f); // Blue
    Color sickColor = new Color(0.8f, 0.2f, 0.2f, 1f);  // Red
    
    Color currentColor = Color.Lerp(healthyColor, sickColor, progression);
    blob.SetColors(currentColor, currentColor * 1.5f);
    
    // Increase animation speed when sick
    blob.SetAnimationSpeed(1f + progression * 2f);
}
```

---

## 📊 Technical Specs

### **Performance**
- **Vertices per blob**: 33 (32 segments + center)
- **Triangles per blob**: 32
- **Draw calls**: 2 per blob (core + glow)
- **Update cost**: ~0.1ms per blob (60 FPS)
- **Recommended max**: 100-200 blobs on screen

### **Memory**
- **Mesh size**: ~2KB per blob
- **Material size**: ~1KB per material
- **Total per blob**: ~5KB

### **Compatibility**
- Unity 2021.3 LTS or newer
- Universal Render Pipeline (URP)
- 2D or 3D projects
- Mobile-friendly (with LOD)

---

## 🚀 Future Enhancements

### **Planned Features**:
1. **Blob Merging**: Cells can merge together
2. **Blob Splitting**: Cells can split (mitosis)
3. **Blob Deformation**: React to collisions
4. **Advanced Shaders**: More glow effects
5. **Blob Sounds**: Audio feedback
6. **Blob Shadows**: 2D shadows
7. **Blob Outlines**: Edge detection

---

## 🎨 Art Direction

### **Why Procedural Blobs?**
✅ **Dynamic**: Always moving, never static
✅ **Unique**: Each blob slightly different
✅ **Scalable**: Easy to create variations
✅ **Performance**: Optimized for many units
✅ **Consistent**: Same style across all cells
✅ **No Assets**: No need for sprite sheets
✅ **Customizable**: Easy to tweak colors/sizes

### **Visual Style**:
- **Neon aesthetic**: Glowing, vibrant
- **Organic shapes**: Soft, rounded
- **Slime physics**: Jiggly, bouncy
- **Smooth animations**: Breathing, pulsing
- **Clear silhouettes**: Easy to identify

---

## 📝 Tips & Best Practices

### **Do's**:
✅ Use presets untuk consistency
✅ Pool blobs untuk performance
✅ Adjust LOD based on distance
✅ Use trail effects sparingly
✅ Test on target hardware

### **Don'ts**:
❌ Don't create too many segments (>64)
❌ Don't animate every frame (use LOD)
❌ Don't forget to disable trails when not moving
❌ Don't use too many particles
❌ Don't skip object pooling

---

## 🎮 Example: Complete Cell Setup

```csharp
using UnityEngine;
using ChronicSurvival.ProceduralVisuals;

public class CellFactory : MonoBehaviour
{
    public GameObject CreateCell(CellType type, Vector3 position)
    {
        // Create GameObject
        GameObject cellObj = new GameObject($"Cell_{type}");
        cellObj.transform.position = position;
        
        // Add blob generator
        BlobGenerator blob = cellObj.AddComponent<BlobGenerator>();
        
        // Add physics
        SlimeBlobPhysics physics = cellObj.AddComponent<SlimeBlobPhysics>();
        
        // Add trail
        BlobTrailEffect trail = cellObj.AddComponent<BlobTrailEffect>();
        
        // Configure based on type
        switch (type)
        {
            case CellType.Macrophage:
                blob.ApplyPreset(BlobPreset.Macrophage);
                physics.SetJiggleAmount(0.05f); // Less jiggly (tanky)
                trail.SetTrailColor(new Color(0.4f, 0.7f, 1f, 0.3f));
                break;
                
            case CellType.TCell:
                blob.ApplyPreset(BlobPreset.TCell);
                physics.SetJiggleAmount(0.15f); // More jiggly (fast)
                trail.SetEmissionRate(30f); // More particles
                break;
                
            case CellType.SugarBlob:
                blob.ApplyPreset(BlobPreset.SugarBlob);
                physics.SetJiggleAmount(0.3f); // Very jiggly (slime)
                trail.SetTrailColor(new Color(1f, 0.8f, 0.4f, 0.3f));
                break;
        }
        
        return cellObj;
    }
}

public enum CellType
{
    Macrophage,
    TCell,
    BCell,
    NKCell,
    Neutrophil,
    SugarBlob,
    PressurePulse,
    DamagedCell
}
```

---

Sistem blob procedural ini memberikan visual yang dynamic, organic, dan performant tanpa perlu asset sprites! 🎨✨
