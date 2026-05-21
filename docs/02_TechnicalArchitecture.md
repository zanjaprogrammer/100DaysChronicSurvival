# Technical Architecture — 100 Days Chronic Survival

## System Architecture Overview

```
┌──────────────────────────────────────────────────────┐
│                    GameManager                       │
│         (State Machine / Game Loop Control)          │
├──────────────────────────────────────────────────────┤
│                                                      │
│  ┌─────────────┐  ┌──────────────┐  ┌─────────────┐ │
│  │BodyComponent│  │  Disease     │  │   Card      │ │
│  │  Manager    │  │  Manager     │  │  Manager    │ │
│  │             │  │              │  │             │ │
│  │ 12 body     │  │ 3 diseases   │  │ Draw/Select │ │
│  │ components  │──│ progression  │──│ action cards│ │
│  │ chain react │  │ staging      │  │ rarity      │ │
│  └─────────────┘  └──────────────┘  └─────────────┘ │
│         │                │                │          │
│         ▼                ▼                ▼          │
│  ┌─────────────────────────────────────────────────┐ │
│  │               EventManager                      │ │
│  │     (Event-driven communication bus)             │ │
│  └─────────────────────────────────────────────────┘ │
│         │                │                │          │
│         ▼                ▼                ▼          │
│  ┌─────────────┐  ┌──────────────┐  ┌─────────────┐ │
│  │  UIManager  │  │   Battle     │  │  Random     │ │
│  │             │  │   Manager    │  │  Event Mgr  │ │
│  │ HUD, Cards  │  │              │  │             │ │
│  │ Disease bars│  │ Auto-battle  │  │ Body        │ │
│  │ Body stats  │  │ visual only  │  │ behaviors   │ │
│  └─────────────┘  └──────────────┘  └─────────────┘ │
│                          │                           │
│                          ▼                           │
│               ┌──────────────────┐                   │
│               │  Arena System    │                   │
│               │                  │                   │
│               │ Walkable Mask    │                   │
│               │ Pathfinding      │                   │
│               │ Spawn Points     │                   │
│               │ Visual Feedback  │                   │
│               └──────────────────┘                   │
└──────────────────────────────────────────────────────┘
```

---

## Data Flow

```
Body Components Change
        ↓
Chain Reaction System (interconnections)
        ↓
Disease Progression Updates
        ↓
Arena Visual Feedback Updates
        ↓
UI Updates (bars, meters, status)
```

---

## Key Design Decisions

### 1. Battle = Visual Feedback ONLY
Player tidak mengontrol battle. Sel bergerak, menyerang, dan mati secara otomatis berdasarkan kondisi body components. Semakin baik kondisi tubuh → semakin kuat visual sel imun. Semakin buruk → semakin banyak corruption visual.

### 2. Arena Pathfinding via Walkable Mask
Daripada menggunakan NavMesh (overkill untuk 2D), kita menggunakan **texture-based walkable mask**:
- Buat versi hitam-putih dari `ArenaBranched.png`
- Putih = area walkable (pembuluh darah)
- Hitam = area non-walkable (dinding jaringan)
- Script membaca pixel data untuk menentukan apakah posisi valid

### 3. Chain Reaction = Inti Edukasi
Semua body components saling terhubung. Ini menciptakan pembelajaran natural:
- Player merasakan langsung konsekuensi keputusan
- Tidak perlu popup edukasi — edukasi menyatu dengan gameplay

---

## Folder Structure

```
Assets/
├── Art/
│   ├── Background/          ← Arena map (ArenaBranched.png)
│   ├── UI/                  ← UI sprites, icons
│   └── Sprites/             ← Cell sprites, effects
├── Data/                    ← ScriptableObject instances
├── Prefabs/
│   ├── UI/                  ← UI prefabs
│   └── Battleground/        ← Cell prefabs, effects
├── Scripts/
│   ├── Arena/               ← NEW: Pathfinding, walkable mask
│   ├── Battle/              ← BattleManager, InfectionNode
│   ├── BodyComponents/      ← BodyComponent, BodyComponentManager
│   ├── Cards/               ← ActionCard, CardManager
│   ├── Core/                ← GameManager, EventManager
│   ├── Disease/             ← Disease, DiseaseManager
│   ├── Editor/              ← Editor tools (UIBuilder)
│   ├── ProceduralVisuals/   ← Blob effects
│   ├── UI/                  ← All UI scripts
│   └── Units/               ← Immune cells, enemies
├── Scenes/
└── Shaders/
```

---

## Platform Target
- **Primary:** Android (Mobile)
- **Resolution:** 1920x1080 reference, scale with screen
- **Input:** Touch only
- **Minimum API:** Android 7.0 (API 24)
