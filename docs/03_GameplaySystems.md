# Gameplay Systems

## Auto Battler System

### Core Mechanics
**Inspiration**: TABS + Auto Chess + Vampire Survivors

**Battle Flow**:
1. **Pre-Battle Phase** (15 seconds)
   - Unit placement on grid
   - Loadout selection
   - Formation setup
   - Emergency card preparation

2. **Combat Phase** (30-60 seconds)
   - Automatic unit behavior
   - AI-driven targeting
   - Physics-based interactions
   - Real-time enemy spawning

3. **Post-Battle Phase** (10 seconds)
   - Damage assessment
   - Resource collection
   - Victory/defeat evaluation
   - Transition to card selection

### Unit Placement System
**Grid-Based Positioning**:
- 5x5 grid arena
- Strategic zones:
  - **Front Line**: High damage intake, protects back
  - **Mid Line**: Balanced position
  - **Back Line**: Safe, for ranged/support units
  - **Flanks**: Side positions for mobility

**Placement Rules**:
- Limited slots based on immune strength
- Unit cost system (energy-based)
- Formation bonuses for synergies
- Can't move units during battle (pre-placement only)

### Auto-Attack Behavior
**Targeting Priority**:
1. Closest enemy (default)
2. Lowest health enemy
3. Highest threat enemy
4. Random (chaos units)

**Attack Patterns**:
- **Melee**: Close range, high damage
- **Ranged**: Distance attacks, lower damage
- **AOE**: Area damage, crowd control
- **Support**: Buffs, healing, debuffs

### Combat Feedback
- **Damage numbers**: Floating text
- **Hit effects**: Visual impact
- **Sound design**: Satisfying combat audio
- **Screen shake**: For big impacts
- **Slow motion**: Critical moments

---

## Roguelike System

### Run Structure
**100 Days = 100 Rounds**

Each round consists of:
1. Battle Phase
2. Lifestyle Decision Phase
3. Random Event Phase
4. Day transition

**Difficulty Curve**:
- Days 1-20: Tutorial, easy enemies
- Days 21-40: First disease emerges
- Days 41-60: Second disease emerges
- Days 61-80: Third disease emerges
- Days 81-100: All diseases active, boss threats

### Procedural Generation
**Random Elements**:
- Starting genetic conditions
- Event pool shuffle
- Card draw order
- Enemy spawn patterns
- Node placement
- Boss variations

**Seeded Runs**:
- Share seeds with friends
- Compete on same conditions
- Daily challenge seeds
- Community challenges

### Permadeath & Meta Progression
**Permadeath**:
- Lose all run progress on death
- Keep meta progression unlocks
- Learn from mistakes

**Meta Progression**:
- Unlock new immune cells
- Unlock new action cards
- Unlock genetic modifiers
- Unlock challenge modes
- Permanent stat bonuses (small)

---

## Arena/Battleground System

### Organ-Based Arenas
**Different Battlefields**:

1. **Bloodstream Arena** (Default)
   - Open space
   - Blood vessel lanes
   - Flow mechanics (units move with current)

2. **Liver Arena** (Toxicity battles)
   - Detox zones (heal immune cells)
   - Toxic puddles (damage over time)
   - Filter mechanics

3. **Pancreas Arena** (Diabetes battles)
   - Insulin production zones
   - Glucose overflow areas
   - Beta cell protection objectives

4. **Heart Arena** (Hypertension battles)
   - Pressure wave mechanics
   - Valve choke points
   - Rhythm-based events

5. **Lung Arena** (Oxygen/Cancer battles)
   - Oxygen zones (buff immune)
   - Tumor growth areas
   - Breathing cycle mechanics

### Dynamic Battlefield Events
**Environmental Hazards**:
- **Pressure Waves**: Knockback, damage
- **Glucose Puddles**: Slow zones
- **Toxic Clouds**: Damage over time
- **Inflammation Zones**: Damage to all units
- **Vessel Ruptures**: Sudden damage spikes

**Beneficial Zones**:
- **Oxygen Rich**: Immune buff
- **Hydration Zones**: Healing over time
- **Immune Boost Areas**: Temporary power-up
- **Safe Zones**: No enemy spawns

### Arena Evolution
**Progressive Changes**:
- Arena expands as disease progresses
- More hazard zones appear
- Corruption spreads visually
- Color palette shifts (healthy → diseased)

---

## Round System

### Round Structure
**Day/Night Cycle** (Optional):
- **Day Rounds**: Normal combat
- **Night Rounds**: Regeneration phase, slower combat
- Sleep quality affects night round effectiveness

### Round Progression
**Scaling Factors**:
- Enemy health: +5% per round
- Enemy damage: +3% per round
- Enemy spawn rate: +2% per round
- Boss chance: +1% per round after day 50

**Milestone Rounds**:
- **Day 10**: First mini-boss
- **Day 25**: Disease activation warning
- **Day 50**: Mid-game crisis event
- **Day 75**: Multiple disease synergy
- **Day 100**: Final survival check

### Time Pressure
**Round Timer**:
- 60 seconds per battle (adjustable)
- Overtime = enemy buff
- Quick clear = bonus rewards
- Time management strategy

---

## Action Card System

### Card Structure
**Card Components**:
```
┌─────────────────────┐
│  CARD TITLE         │
│  [Icon]             │
├─────────────────────┤
│ Body Effects:       │
│  ↑ Energy +15       │
│  ↓ Stress -20       │
│  ↑ Sleep +10        │
├─────────────────────┤
│ Immune Bonus:       │
│  +10% Attack Speed  │
├─────────────────────┤
│ Disease Impact:     │
│  Diabetes -5%       │
├─────────────────────┤
│ [Rarity] [Synergy]  │
└─────────────────────┘
```

### Card Categories

#### Positive Lifestyle Cards
**Nutrition Cards**:
- 🥗 Balanced Meal: ↑ Energy, ↓ Blood Sugar, ↑ Metabolism
- 💧 Hydration: ↑ Hydration, ↓ Toxicity, ↓ Inflammation
- 🥦 Antioxidant Foods: ↓ Inflammation, ↓ Toxicity, ↑ Immune
- 🐟 Omega-3 Rich: ↓ Inflammation, ↑ Heart Stability
- 🍎 Fiber Intake: ↓ Blood Sugar, ↑ Metabolism

**Exercise Cards**:
- 🏃 Cardio 30min: ↑ Energy, ↓ Blood Sugar, ↓ Blood Pressure, ↓ Stress
- 💪 Strength Training: ↑ Metabolism, ↑ Insulin Efficiency, ↑ Energy
- 🧘 Yoga: ↓ Stress, ↑ Sleep Quality, ↓ Blood Pressure
- 🚶 Light Walk: ↑ Energy, ↓ Stress, ↓ Blood Sugar
- 🏊 Swimming: ↑ Energy, ↓ Stress, ↑ Heart Stability

**Rest Cards**:
- 😴 Deep Sleep 8h: ↑ Sleep Quality, ↑ ALL stats recovery
- 🛌 Power Nap: ↑ Energy, ↓ Stress (small)
- 🌙 Sleep Schedule: ↑ Sleep Quality, ↑ Hormone Balance
- 🧘 Meditation: ↓ Stress, ↑ Sleep Quality, ↓ Blood Pressure

**Medical Cards**:
- 💊 Vitamin Supplement: ↑ Immune Strength, ↓ Inflammation
- 🩺 Health Checkup: Reveal disease progression, small buffs
- 💉 Medication: Strong disease reduction (rare)
- 🧪 Blood Test: Information + small immune boost

**Social Cards**:
- 👥 Social Support: ↓ Stress, ↑ Hormone Balance
- 😊 Laughter Therapy: ↓ Stress, ↑ Immune Strength
- 🎨 Creative Hobby: ↓ Stress, ↑ Sleep Quality
- 🌳 Nature Walk: ↓ Stress, ↑ Energy, ↑ Oxygen

#### Negative Lifestyle Events (Random)
**Junk Food Events**:
- 🍔 Fast Food: ↑ Blood Sugar, ↑ Inflammation, ↓ Energy
- 🍕 Late Night Pizza: ↑ Blood Sugar, ↓ Sleep Quality
- 🍰 Sugar Binge: ↑↑ Blood Sugar, ↓ Insulin Efficiency
- 🥤 Soda Addiction: ↑ Blood Sugar, ↑ Toxicity

**Stress Events**:
- 💼 Work Deadline: ↑ Stress, ↓ Sleep Quality
- 😰 Anxiety Attack: ↑↑ Stress, ↑ Blood Pressure
- 📱 Social Media Doom: ↑ Stress, ↓ Sleep Quality
- 🚗 Traffic Jam: ↑ Stress, ↑ Blood Pressure

**Substance Events**:
- 🚬 Smoking: ↑ Toxicity, ↑ Inflammation, ↓ Immune, ↓ Oxygen
- 🍺 Alcohol: ↑ Toxicity, ↓ Immune, ↓ Sleep Quality, ↓ Hydration
- ☕ Caffeine Overload: ↑ Stress, ↓ Sleep Quality, ↓ Hydration

**Sleep Events**:
- 🌙 Insomnia: ↓↓ Sleep Quality, ↑ Stress, ↓ Energy
- 📱 Screen Before Bed: ↓ Sleep Quality, ↑ Stress
- 🎮 Gaming All Night: ↓↓ Sleep Quality, ↓ Energy, ↑ Stress

### Card Rarity System
**Common** (60% drop rate):
- Basic lifestyle choices
- Small stat changes (+/- 10-15)
- Single component focus

**Uncommon** (25% drop rate):
- Better lifestyle choices
- Medium stat changes (+/- 15-25)
- Two component focus
- Small synergies

**Rare** (12% drop rate):
- Excellent lifestyle choices
- Large stat changes (+/- 25-40)
- Multiple component focus
- Strong synergies
- Unique effects

**Legendary** (3% drop rate):
- Perfect lifestyle choices
- Massive stat changes (+/- 40-60)
- All component impact
- Powerful synergies
- Game-changing effects
- Permanent bonuses

### Card Synergies
**Combo Examples**:

**"Athlete Build"**:
- Cardio + Strength Training + Balanced Meal
- Bonus: +20% immune attack speed, +15 energy

**"Zen Master"**:
- Meditation + Yoga + Deep Sleep
- Bonus: Stress cannot exceed 30, +20% immune health

**"Clean Living"**:
- Hydration + Antioxidants + No Smoking
- Bonus: Toxicity reduction doubled, -30% cancer progression

**"Metabolic Control"**:
- Fiber Intake + Exercise + Sleep Schedule
- Bonus: Blood sugar auto-stabilizes, +25% insulin efficiency

### Emergency Recovery Cards
**Crisis Cards** (appear when critical):
- 🚑 Emergency Medical: Instant -30% to highest disease
- 💊 Intensive Treatment: Restore 50% of all body components
- 🏥 Hospital Stay: Full recovery, but skip 3 rounds
- ⚡ Adrenaline Rush: Temporary massive immune boost

---

## Random Body Behavior System

### Event Probability System
**Base Probabilities**:
- Negative events: 60%
- Positive events: 30%
- Neutral events: 10%

**Modified by Body State**:
- High stress: +20% negative events
- Good sleep: +15% positive events
- Low energy: +10% negative events
- High immune: +10% positive events

### Event Categories
**Cravings**:
- Sugar craving (high blood sugar = higher chance)
- Junk food craving (low energy = higher chance)
- Caffeine craving (low energy + stress = higher chance)

**Behavioral**:
- Spontaneous exercise (high energy = higher chance)
- Social interaction (low stress = higher chance)
- Screen time (high stress = higher chance)

**Environmental**:
- Weather effects (seasonal)
- Pollution exposure (location-based)
- Allergen exposure (seasonal)

### Player Mitigation
**Reducing Negative Events**:
- Maintain high sleep quality
- Keep stress low
- Build positive habits (card synergies)
- Unlock "Willpower" upgrades (meta progression)

**Cannot Eliminate**:
- Always some randomness
- Reflects real-life unpredictability
- Part of the challenge

---

## Build/Synergy System

### Build Archetypes

**1. Tank/Sustain Build**
- Focus: Defense, regeneration, longevity
- Key cards: Sleep, Hydration, Balanced Meals
- Key units: Macrophages (tanks)
- Strategy: Outlast enemies, slow and steady
- Counters: Diabetes (sustain through waves)

**2. Aggressive Immune Build**
- Focus: High damage, fast clear
- Key cards: Exercise, Supplements, Protein
- Key units: NK Cells, T-Cells (damage dealers)
- Strategy: Kill before being killed
- Counters: Cancer (burst down mutations)

**3. Metabolism Control Build**
- Focus: Blood sugar management
- Key cards: Fiber, Exercise, Sleep
- Key units: Insulin-boosting support units
- Strategy: Prevent diabetes activation
- Counters: Diabetes (specialized counter)

**4. Cardiovascular Build**
- Focus: Blood pressure control
- Key cards: Meditation, Cardio, Omega-3
- Key units: Heart-stabilizing units
- Strategy: Prevent hypertension
- Counters: Hypertension (specialized counter)

**5. Anti-Cancer Build**
- Focus: Toxicity control, mutation prevention
- Key cards: Antioxidants, Hydration, No Smoking
- Key units: NK Cells (anti-cancer specialists)
- Strategy: Prevent mutations
- Counters: Cancer (specialized counter)

**6. Balanced Build**
- Focus: Jack of all trades
- Key cards: Mix of all categories
- Key units: Balanced army composition
- Strategy: Adapt to threats
- Counters: All diseases moderately

### Synergy Mechanics
**Card Synergies**:
- Playing related cards in sequence = bonus effects
- Building "streaks" of healthy choices
- Combo counters for visual feedback

**Unit Synergies**:
- Certain units buff each other when nearby
- Formation bonuses
- Type advantages (e.g., NK Cells + T-Cells)

---

## Lifestyle Decision System

### Decision Framework
**Every Round**:
- Player chooses 1 of 3 cards
- Cards are drawn from pool based on:
  - Current body state
  - Previous choices
  - Disease progression
  - Rarity roll

**Strategic Depth**:
- Short-term vs long-term benefits
- Risk vs reward
- Build commitment vs adaptation
- Resource management

### Consequence Visualization
**Immediate Feedback**:
- Stat changes animate
- Visual effects on body components
- Audio feedback
- Immune units react (buff/debuff animations)

**Long-term Tracking**:
- "Lifestyle Score" meter
- Habit streak counters
- Build progress indicators
- Disease risk predictions

---

## Risk vs Reward System

### High-Risk Cards
**Examples**:
- "Extreme Workout": Huge energy boost, but risk injury
- "Fasting": Great for insulin, but risk low energy
- "Experimental Treatment": Massive disease reduction, but side effects

**Risk Mechanics**:
- Percentage chance of negative outcome
- Higher reward = higher risk
- Can be mitigated with other stats

### Reward Scaling
**Performance Bonuses**:
- Quick battle clear: Better card options
- No damage taken: Rare card chance
- Perfect round: Legendary card chance
- Streak bonuses: Cumulative rewards

### Gambling Mechanics
**"Risky Choice" Cards**:
- Unknown effects until played
- Could be amazing or terrible
- Adds excitement and unpredictability
- Optional (can always choose safe cards)
