# Development Roadmap — 100 Days Chronic Survival

## Status Legenda
- ✅ Selesai
- 🔧 Sedang dikerjakan
- ⬜ Belum dimulai

---

## Phase 1: Core Architecture ✅
**Target:** Foundation scripts & project structure

- [x] GameManager (Singleton, state machine)
- [x] EventManager (Event-driven communication)
- [x] GameState enum (MainMenu, Battle, CardSelection, RandomEvent, Paused, GameOver, Victory)
- [x] Project folder structure (Scripts/, Art/, Prefabs/, Data/)

---

## Phase 2: Body Component System ✅
**Target:** 12 komponen tubuh yang saling terhubung

- [x] BodyComponent ScriptableObject base class
- [x] BodyComponentManager (manages all 12 components)
- [x] ComponentState enum (Optimal, Normal, Warning, Critical)
- [x] Chain reaction / interconnection system
- [x] Passive decay system

---

## Phase 3: Disease System ✅
**Target:** 3 penyakit dengan progression meter

- [x] Disease base class
- [x] DiabetesDisease, HypertensionDisease, CancerDisease
- [x] DiseaseManager (singleton)
- [x] DiseaseStage enum (None, Stage1, Stage2, Stage3, Critical)
- [x] Disease progression berdasarkan body component conditions

---

## Phase 4: Card System ✅
**Target:** Action card untuk lifestyle decisions

- [x] ActionCard ScriptableObject
- [x] CardManager (draw & select)
- [x] Card effects pada body components
- [x] Rarity system (Common, Rare, Legendary)

---

## Phase 5: Battle System ✅ (Partial)
**Target:** Auto-battle visual feedback

- [x] BattleManager
- [x] UnitSpawner
- [x] InfectionNode (disease spawn points)
- [x] Basic immune cell types (Macrophage, T-Cell, B-Cell, NK Cell, Neutrophil)
- [x] Basic enemy types per disease
- [x] Procedural blob visuals

---

## Phase 6: Arena & Pathfinding ✅
**Target:** Sel bergerak HANYA di area pembuluh darah

- [x] Arena background imported (ArenaBranched.png)
- [x] **Arena Walkable Mask** — buat texture hitam-putih dari arena untuk menentukan area walkable
- [x] **BloodstreamPathfinder** — pathfinding system menggunakan walkable mask
- [x] **Arena boundary collider** — PolygonCollider2D/mask-based bounding di pembuluh darah
- [x] Spawn points hanya di dalam area pembuluh
- [x] Sel darah merah ambient movement di aliran darah

---

## Phase 7: UI System ✅
**Target:** Mobile-ready UI yang sesuai mockup

- [x] UIManager (panel switching berdasarkan GameState)
- [x] BodyStatBar, BodyStatsPanel
- [x] DiseaseProgressUI, DiseaseProgressPanel
- [x] CardUI, CardSelectionUI
- [x] RandomEventUI
- [x] MainMenuUI
- [x] DayCounterUI
- [x] **Canvas setup di scene** — merakit semua UI secara visual lewat SceneSetupHelper
- [x] **Mobile-friendly scaling** — CanvasScaler dengan ScreenMatch
- [x] **Card selection overlay** — dark overlay + 3 card layout
- [x] **Game Over / Victory screen**
- [x] **Pause menu**
- [x] **Pre-game setup screen** (genetic traits selection)

---

## Phase 8: Visual Feedback System 🔧
**Target:** Arena berubah berdasarkan kondisi tubuh

- [ ] Arena visual overlay berubah berdasarkan:
  - Blood Sugar tinggi → sugar sludge muncul di pembuluh
  - Inflammation tinggi → area kemerahan
  - Toxicity tinggi → area gelap/corruption
- [ ] Sel imun melemah secara visual saat Immune Strength rendah
- [ ] Particle effects (pressure shockwave, glucose particles, mutation glow)
- [ ] Ambient sel darah merah bergerak sepanjang aliran

---

## Phase 9: Random Event System ⬜
**Target:** Random body behavior yang tidak bisa dikontrol

- [ ] RandomEvent ScriptableObject
- [ ] RandomEventManager (trigger setelah card selection)
- [ ] Pool of positive & negative events
- [ ] Event UI popup dengan deskripsi & efek
- [ ] Probability system (weighted random)

---

## Phase 10: Pre-Game / Genetic Setup ⬜
**Target:** Case random tubuh di awal game

- [ ] GeneticTrait ScriptableObject
- [ ] Trait selection screen (random 2-3 traits)
- [ ] Traits memengaruhi starting body component values
- [ ] Traits memengaruhi disease risk multipliers

---

## Phase 11: Audio & Polish ⬜
**Target:** Sound effects, music, final touches

- [ ] Background music (ambient biological)
- [ ] SFX untuk card selection, disease progression, events
- [ ] Screen shake saat disease stage naik
- [ ] Haptic feedback untuk mobile

---

## Phase 12: Android Build & Testing ⬜
**Target:** Build APK & testing

- [ ] Android build settings
- [ ] Touch input optimization
- [ ] Performance profiling
- [ ] Resolution testing (multiple screen sizes)
- [ ] APK build & device testing

---

## Prioritas Sekarang (Phase 8 & 9)
1. ⭐ **Visual Feedback System** — Menghubungkan visual pembuluh darah dengan status tubuh (misalnya, kemerahan saat inflamasi).
2. ⭐ **Random Event System** — Memperbanyak pool event dan membuat weighted random.
3. ⭐ **Audio & Polish** — Menambahkan BGM dan SFX biological ambient.
