# Chronic Survival - UI/UX Redesign Blueprint

This blueprint outlines a complete overhaul of the game's user interface. The goal is to move from basic unity panels to a cohesive, immersive, and highly responsive **tactical cellular warfare / medical telemetry dashboard** aesthetic.

---

## 1. Design Concept & Philosophy

### A. The "Medical Telemetry Dashboard" Aesthetic
*   **Visual Direction:** High-tech lab UI meets dark-mode terminal. Translucent glassmorphic panels, glowing clean fonts, crisp borders, and clinical data representations.
*   **Color Palette:**
    *   **Background Base:** Deep Obsidian / Near-Black (#0A0C10 to #0E1117)
    *   **Panel Fill:** Semi-transparent Dark Navy/Slate (#161B22 at 75% opacity) with subtle background blur (Glassmorphism).
    *   **Neutral Text:** Clean White (#F0F6FC) and Cool Muted Gray (#8B949E).
    *   **Primary Active / Safe Stats:** Vibrant Cyan / Teal (#58A6FF / #39D353).
    *   **Warning / Danger States:** Warn Orange (#F0883E) and Critical Crimson (#FF7B72).
*   **Typography:** Monospaced or clean geometric sans-serif (e.g., JetBrains Mono, Roboto, or custom high-tech sci-fi fonts).

### B. "No-Icon" Stat Representation Rules
As requested, all body component stats and metrics must drop primitive emojis and graphical icons in favor of **pure text labels powered by explicit color coding** to indicate severity and status.
*   **Healthy Range (70-100%):** Teal / Bright Blue Text. Represents stable physiological conditions.
*   **Compromised Range (40-69%):** Amber / Orange Text. Indicates moderate strain or early infection onset.
*   **Critical Range (0-39%):** Deep Red Text (pulsing). Action must be taken immediately.

---

## 2. Global UI Flow & Hierarchy

```
[ Launch App ]
      │
      ▼
┌────────────────────────┐
│   MAIN MENU SCREEN     │  <-- Clean typography, ambient bio-mesh bg, cinematic transition
└──────────┬─────────────┘
           │ (Fades Out + Slides Main Panels)
           ▼
┌────────────────────────┐
│     BATTLEFIELD        │  <-- The Arena: Blood vessels, cells, infection nodes, moving blood streams
│      (Gameplay)        │
└──────────┬─────────────┘
           │
 ┌─────────┴─────────┐
 ▼                   ▼
┌────────────────────────┐ ┌────────────────────────┐
│    BATTLE HUD PANEL    │ │  CARD SELECTION UI     │ <-- Dimmed arena background, cards glide
│ (Left/Right Sidebars)  │ │ (Tactical Action Deck) │     up from bottom on player turn
└────────────────────────┘ └────────────────────────┘
```

---

## 3. Screen-by-Screen Detailed Layout Plans

### Screen A: Start / Main Menu Screen
*   **Structure:**
    *   **Background:** Dark animated background showing red and blue cells floating down a faint digital bloodstream (using a simple particle system with shallow depth of field).
    *   **Title Header:** Massive, ultra-clean modern text **"CHRONIC SURVIVAL"** in high-contrast crisp white, with a subtle horizontal scanning line glow effect.
    *   **Menu Options:** A vertical list aligned to the bottom-left or center. Clean borders with left-aligned monospaced text.
        *   `[ 01 // START SYSTEM ]` (Play Game)
        *   `[ 02 // ACCESS ARCHIVES ]` (Card Collection / Stats)
        *   `[ 03 // CALIBRATION ]` (Settings / Audio)
        *   `[ 04 // TERMINATE ]` (Quit)
*   **Animations:**
    *   **Hover:** Menu options slide right by `15px`, border color shifts from cool grey to vibrant cyan, accompanied by a soft sci-fi tick sound.
    *   **Transition on Click:** The title slides upwards off-screen, while menu options slide down. The background particles speed up into a warp stream, seamlessly fading into the gameplay view.

---

### Screen B: The Battle HUD (Left & Right Sidebars)
The HUD operates as an overlay split into distinct left and right tactical monitoring sidebars to keep the central battlefield fully clear for combat.

#### Left Sidebar: "PHYSIOLOGICAL OVERVIEW" (Body Components)
*   **Layout:** Vertical stack of text-only metrics. Every metric uses a dynamic status-colored layout. No raw icons.
*   **Structure for each Stat:**
    *   Row 1: State Label (e.g. `HEALTH`, `ENERGY`, `BLOOD GLUCOSE`) + numeric percentage aligned right.
    *   Row 2: A thin custom progress bar (4px height) matching the stat's state color.
*   **Dynamic Styling Rules:**
    ```
    [ HEALTH ] ────────────────────────────────────────── 78% (Teal Text / Teal Bar)
    [ ENERGY ] ────────────────────────────────────────── 62% (Orange Text / Orange Bar)
    [ TOXIN ] ─────────────────────────────────────────── 30% (Red Text / Red Bar, Pulsing)
    ```

#### Lower Left Sidebar: "DISEASE PROGRESSION"
*   **Layout:** Separated by a thin horizontal line from core stats.
*   **Metrics:** Shows tracked diseases.
    *   `DIABETES ── 45%`
    *   `HYPERTENSION ── 37%`
    *   `CANCER ── 28%`
*   **VFX:** If a disease crosses `80%`, the label flashes white and red alternately, warning the player of imminent multi-organ failure.

#### Right Sidebar: "LIVE BIO-FEEDBACK LOG"
*   **Layout:** A running vertical stack of tactical telemetry status items.
*   **Styling:**
    *   Active/Positive states: Clean blue labels with grey describer text.
    *   Negative/Inflammatory states: Orange/Red labels with warning indicators.
*   **Examples:**
    *   `GLUCOSE SPIKE // High blood sugar detected. Immune cells under minor strain.`
    *   `INFLAMMATION HIGH // Macrophages moving 15% slower due to systemic stress.`

---

### Screen C: Card Action UI (Tactical Deck)
When the game state shifts to card selection/actions, the arena dims, and three clean tactical action options slide up from the bottom of the screen.

*   **Card Anatomy & Design:**
    *   **Container:** A vertical glassmorphic card with rounded corners (`12px`), a thin border, and an extremely subtle drop shadow.
    *   **Header:** The card title in modern clean text, utilizing colors that classify the category (e.g., Green/Teal for diet/hydration, Blue for medical actions, Orange/Red for emergency adrenaline).
    *   **Center Section:** Visual illustration using clean shapes or minimal abstract line patterns instead of basic flat icons.
    *   **Footer Details:** Impact data presented in clean telemetry format:
        ```
        ---------------------------------
        IMPACT:
          +15% SYSTEM IMMUN (Teal text)
          -10% INFLAMMATION (Teal text)
        ---------------------------------
        [ INGEST / CONFIRM ]
        ```
*   **Animations:**
    *   **Deal Animation:** Cards deal sequentially from the bottom-right, sliding into position with a rotation bounce (`0.15s` offset between cards).
    *   **Hover Animation:** Card scales up by `1.08x`, lifts upward by `20px`, and its border glows intensely. Other cards slightly dim.
    *   **Selection / Play Animation:** The chosen card flashes white, slides quickly upward towards the center of the arena before dissolving into a clean pulse wave that expands outward, applying the card effects to the battlefield and updating the stats sidebar in real-time.

---

### Screen D: Arena & HUD Interaction Overlay
*   **Arena Legend (Bottom Right):** A highly readable, clean legend panel describing the blood flow behaviors, mutagen areas, and sticky zones using colored line path vectors instead of emojis.
*   **Time & Phase Indicator (Top Center):**
    *   Displays `ROUND TIME REMAINING` and `NEXT WAVE INCOMING` in a modern bold monospaced countdown clock (e.g., `00:25`).
    *   Underneath, a sleek randomized "Body State Behavior" ticker text provides immediate clinical context (e.g., `RANDOM STATE: MUTATION RISK HIGH`).

---

## 4. Animation and Tweening Specifications

To achieve the "premium tactical feel", all UI transitions must use standard easing equations (e.g., Sinusoidal or Cubic curves) rather than linear movement.

| UI Element | Trigger Event | Animation Description | Easing / Timing |
| :--- | :--- | :--- | :--- |
| **All Panels** | Screen Open | CanvasGroup Alpha `0 -> 1` + Vertical Slide down `30px` | `EaseOutCubic` (0.4s) |
| **Menu Options** | Mouse Hover | Horizontal offset shift `+15px` + Border color glow | `EaseOutQuad` (0.2s) |
| **Card Deck** | Hand Deal | Bounce up from bottom screen edge with individual offsets | `EaseOutBack` (0.5s) |
| **Active Cards** | Card Play | Quick scale-up to `1.2x` -> fade out -> screen shake + shockwave | `EaseInBack` (0.3s) |
| **Critical Stats** | Stat < 40% | Text color loops from Crimson Red to Orange-Red | `PingPong Linear` (1s loop) |

---

## 5. Technical Implementation Steps for chronic-survival

1.  **UI Canvas Overhaul:** Re-arrange the Unity Canvas into two clean layers:
    *   `Static HUD Layer` (Always rendered, low overhead)
    *   `Interactive Modals Layer` (Card decks, random events, pause overlays)
2.  **TextMeshPro Style Presets:** Establish unified TMPro style assets:
    *   `Title_Modern_Clean` (Main menu titles, huge headers)
    *   `Data_Mono_Safe` (Green/Teal stats data)
    *   `Data_Mono_Warn` (Orange/Amber warnings)
    *   `Data_Mono_Critical` (Pulsing Red dangers)
3.  **UI Controller Refactoring:**
    *   Extend `GameplayHUDController` to handle CanvasGroup fades and slider scale tweens.
    *   Ensure `UIManager.BindPanels` connects safely on dynamic re-builds without dropping references.
