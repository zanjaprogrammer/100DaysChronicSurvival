# Body Components System

## Overview
Body components adalah sistem status biologis yang saling terhubung. Ini adalah inti dari strategi game dan mekanisme edukasi. Setiap komponen mempengaruhi gameplay secara langsung dan menciptakan chain reactions yang realistis.

## Core Components

### 1. Energy ⚡
**Range**: 0-100
**Optimal**: 60-80
**Critical Low**: <20

**Represents**:
- Stamina tubuh
- Kualitas metabolisme
- Kapasitas aktivitas

**Affects**:
- Immune cell regeneration rate
- Immune attack speed (+1% per 10 energy)
- Movement speed of immune units
- Card selection pool (low energy = fewer exercise cards)

**Increased By**:
- ✅ Sleep (7-9 hours): +15-25
- ✅ Balanced meals: +10-15
- ✅ Light exercise: +5-10
- ✅ Hydration: +5
- ✅ Stress reduction: +5-10

**Decreased By**:
- ❌ Insomnia/Begadang: -15-25
- ❌ Stress: -10-15
- ❌ Junk food: -5-10
- ❌ Overexertion: -10-20
- ❌ Dehydration: -5-10
- ❌ High blood sugar: -5 per round

**Gameplay Impact**:
- Low energy = slower immune response
- Very low energy = immune cells move sluggishly
- Zero energy = immune cells stop regenerating

---

### 2. Blood Sugar 🍬
**Range**: 0-200 (mg/dL scaled)
**Optimal**: 70-100
**Danger Zone**: >140
**Critical**: >180

**Represents**:
- Glucose levels in bloodstream
- Metabolic balance
- Diabetes risk indicator

**Affects**:
- **Diabetes progression** (primary driver)
- Immune cell efficiency (high sugar = slower)
- Energy stability
- Inflammation levels
- Insulin efficiency degradation

**Increased By**:
- ❌ Sugary drinks: +30-50
- ❌ Junk food: +20-40
- ❌ Stress (cortisol): +10-15
- ❌ Lack of exercise: +5 per round
- ❌ Poor sleep: +10-20

**Decreased By**:
- ✅ Exercise: -20-30
- ✅ Insulin efficiency (passive)
- ✅ Balanced diet: -10-15
- ✅ Hydration: -5-10
- ✅ Medication cards: -30-50

**Gameplay Impact**:
- >140: Diabetes nodes start spawning
- >160: Glucose puddles appear (slow zones)
- >180: Insulin resistance enemies spawn
- High sugar = immune cells attack 20% slower

**Visual Feedback**:
- Normal: Clear bloodstream
- High: Sticky, syrupy visual effect
- Critical: Crystallized glucose particles

---

### 3. Blood Pressure ❤️
**Range**: 0-200 (mmHg scaled)
**Optimal**: 90-120
**Danger Zone**: >140
**Critical**: >180

**Represents**:
- Cardiovascular stress
- Vessel integrity
- Hypertension risk

**Affects**:
- **Hypertension progression** (primary driver)
- Battlefield stability (high pressure = chaos)
- Immune cell damage from environment
- Heart stability
- Vessel rupture events

**Increased By**:
- ❌ Stress: +15-25
- ❌ High sodium foods: +10-20
- ❌ Smoking: +15-20
- ❌ Alcohol: +10-15
- ❌ Lack of exercise: +5 per round
- ❌ Poor sleep: +10-15

**Decreased By**:
- ✅ Exercise (aerobic): -20-30
- ✅ Stress reduction: -15-25
- ✅ Meditation: -10-20
- ✅ Healthy diet: -10-15
- ✅ Medication cards: -30-40

**Gameplay Impact**:
- >140: Pressure waves appear periodically
- >160: Vessel rupture zones spawn
- >180: Shockwave attacks damage immune cells
- High pressure = battlefield becomes unstable

**Visual Feedback**:
- Normal: Steady pulse
- High: Rapid pulsing, red tint
- Critical: Screen shake, vessel cracks

---

### 4. Immune Strength 🛡️
**Range**: 0-100
**Optimal**: 70-100
**Critical Low**: <30

**Represents**:
- Overall immune system power
- White blood cell effectiveness
- Disease resistance

**Affects**:
- Immune cell damage output
- Immune cell health
- Regeneration rate
- Resistance to disease buffs
- Number of immune cells that can be deployed

**Increased By**:
- ✅ Good sleep: +10-20
- ✅ Balanced nutrition: +10-15
- ✅ Exercise (moderate): +5-15
- ✅ Stress reduction: +5-10
- ✅ Supplements: +5-10
- ✅ Low inflammation: +5 per round

**Decreased By**:
- ❌ Poor sleep: -10-20
- ❌ High stress: -10-15
- ❌ Smoking: -15-25
- ❌ Alcohol: -10-20
- ❌ Junk food: -5-10
- ❌ High inflammation: -5 per round
- ❌ High toxicity: -10 per round

**Gameplay Impact**:
- <50: Immune cells deal 50% damage
- <30: Immune cells have 50% health
- <10: Immune cells stop spawning
- High immune = unlock elite units

---

### 5. Stress 😵
**Range**: 0-100
**Optimal**: 0-30
**Danger Zone**: >60
**Critical**: >80

**Represents**:
- Psychological pressure
- Cortisol levels
- Mental health status

**Affects**:
- Blood pressure (direct correlation)
- Cancer mutation rate
- Energy levels
- Sleep quality
- Hormone balance
- Immune strength

**Increased By**:
- ❌ Work pressure events: +15-25
- ❌ Poor sleep: +10-15
- ❌ Caffeine overload: +5-10
- ❌ Social conflicts: +10-20
- ❌ Disease progression: +5 per disease stage
- ❌ Low energy: +5 per round

**Decreased By**:
- ✅ Meditation: -20-30
- ✅ Exercise: -15-25
- ✅ Good sleep: -10-20
- ✅ Social support: -10-15
- ✅ Hobbies/relaxation: -10-20
- ✅ Nature exposure: -5-15

**Gameplay Impact**:
- >60: Hypertension nodes spawn faster
- >70: Cancer mutation rate +50%
- >80: Random negative events more frequent
- High stress = all stats degrade faster

---

### 6. Sleep Quality 🌙
**Range**: 0-100
**Optimal**: 70-100
**Critical Low**: <40

**Represents**:
- Rest effectiveness
- Recovery capacity
- Circadian rhythm health

**Affects**:
- ALL other components (master stat)
- Immune regeneration
- Energy recovery
- Inflammation reduction
- Hormone balance
- Stress reduction

**Increased By**:
- ✅ Consistent sleep schedule: +20-30
- ✅ 7-9 hours sleep: +15-25
- ✅ Stress reduction: +10-15
- ✅ Exercise (not before bed): +5-10
- ✅ Dark, quiet environment: +5-10

**Decreased By**:
- ❌ Insomnia: -20-30
- ❌ Caffeine late: -10-15
- ❌ Screen time before bed: -10-20
- ❌ Stress: -10-15
- ❌ Alcohol: -15-20
- ❌ Irregular schedule: -10-15

**Gameplay Impact**:
- <50: All stats degrade 2x faster
- <30: Immune regen stops at night
- <10: Emergency "exhaustion" debuff
- High sleep = bonus recovery between rounds

**Special Mechanic**:
- "Sleep Debt" accumulates if <60 for multiple rounds
- Sleep debt increases all negative effects
- Can be cleared with consecutive good sleep

---

### 7. Inflammation 🔥
**Range**: 0-100
**Optimal**: 0-20
**Danger Zone**: >50
**Critical**: >80

**Represents**:
- Chronic inflammation
- Cellular damage
- Immune overreaction

**Affects**:
- **ALL disease progression** (universal amplifier)
- Cancer growth rate
- Diabetes severity
- Hypertension damage
- Immune efficiency (paradox: high inflammation = weaker immune)

**Increased By**:
- ❌ Junk food: +10-20
- ❌ Smoking: +15-25
- ❌ Alcohol: +10-15
- ❌ Stress: +10-15
- ❌ Poor sleep: +10-20
- ❌ Sedentary lifestyle: +5 per round
- ❌ High blood sugar: +5 per round
- ❌ Toxicity: +10 per round

**Decreased By**:
- ✅ Anti-inflammatory foods: -15-25
- ✅ Exercise: -10-20
- ✅ Good sleep: -15-25
- ✅ Stress reduction: -10-15
- ✅ Omega-3 supplements: -10-20
- ✅ Hydration: -5-10

**Gameplay Impact**:
- >50: All diseases progress 50% faster
- >70: "Cytokine Storm" event possible
- >80: Immune cells damage themselves
- High inflammation = global danger multiplier

**Visual Feedback**:
- Normal: Clear battlefield
- High: Red haze, heat distortion
- Critical: Fire particles, burning effect

---

### 8. Insulin Efficiency 💉
**Range**: 0-100
**Optimal**: 80-100
**Danger Zone**: <50
**Critical**: <30

**Represents**:
- Insulin receptor sensitivity
- Glucose metabolism effectiveness
- Diabetes resistance

**Affects**:
- Blood sugar control (passive reduction)
- Diabetes progression rate
- Energy stability
- Metabolism speed

**Increased By**:
- ✅ Exercise: +10-20
- ✅ Weight management: +5-15
- ✅ Low sugar diet: +10-15
- ✅ Intermittent fasting: +5-10
- ✅ Sleep quality: +5-10

**Decreased By**:
- ❌ High blood sugar (sustained): -5 per round
- ❌ Sedentary lifestyle: -5 per round
- ❌ Junk food: -10-15
- ❌ Stress: -5-10
- ❌ Poor sleep: -5-10
- ❌ Aging (passive): -1 per 10 days

**Gameplay Impact**:
- <50: Blood sugar rises 2x faster
- <30: Insulin resistance enemies spawn
- <10: "Metabolic Syndrome" debuff
- High efficiency = natural sugar control

**Special Mechanic**:
- "Insulin Resistance" threshold at <40
- Once resistant, harder to recover
- Requires sustained lifestyle changes

---

### 9. Toxicity ☣️
**Range**: 0-100
**Optimal**: 0-10
**Danger Zone**: >50
**Critical**: >80

**Represents**:
- Accumulated toxins
- Cellular damage
- Carcinogen exposure

**Affects**:
- **Cancer progression** (primary driver)
- Mutation rate
- Immune strength
- Inflammation
- Liver function

**Increased By**:
- ❌ Smoking: +20-30
- ❌ Alcohol: +15-25
- ❌ Processed foods: +10-15
- ❌ Air pollution: +5-10
- ❌ Pesticides: +5-10
- ❌ Lack of detox: +2 per round

**Decreased By**:
- ✅ Hydration: -10-15
- ✅ Antioxidant foods: -10-20
- ✅ Exercise (sweating): -5-15
- ✅ Liver support: -10-15
- ✅ Clean diet: -5-10
- ✅ Time (passive): -2 per round

**Gameplay Impact**:
- >50: Cancer mutation nodes spawn
- >70: Carcinogen zones appear
- >80: Rapid tumor growth
- High toxicity = cancer enemies evolve faster

---

### 10. Metabolism 🔥
**Range**: 0-100
**Optimal**: 60-80
**Critical Low**: <30

**Represents**:
- Metabolic rate
- Energy conversion efficiency
- Calorie processing

**Affects**:
- Energy generation
- Blood sugar control
- Weight management
- Insulin efficiency

**Increased By**:
- ✅ Exercise (especially strength): +10-20
- ✅ Muscle mass: +5-15
- ✅ Protein intake: +5-10
- ✅ Good sleep: +5-10
- ✅ Hydration: +5

**Decreased By**:
- ❌ Sedentary lifestyle: -5 per round
- ❌ Crash diets: -10-20
- ❌ Poor sleep: -10-15
- ❌ Aging (passive): -1 per 10 days
- ❌ Stress: -5-10

**Gameplay Impact**:
- High metabolism = faster energy recovery
- Low metabolism = energy drains faster
- Affects card effectiveness

---

### 11. Hydration 💧
**Range**: 0-100
**Optimal**: 70-100
**Critical Low**: <30

**Represents**:
- Body water levels
- Cellular function
- Detoxification capacity

**Affects**:
- Energy levels
- Blood pressure
- Toxicity removal
- Metabolism
- Immune function

**Increased By**:
- ✅ Drinking water: +20-30
- ✅ Hydrating foods: +10-15
- ✅ Electrolyte balance: +5-10

**Decreased By**:
- ❌ Caffeine: -5-10
- ❌ Alcohol: -10-20
- ❌ Exercise (sweating): -5-10
- ❌ Hot environment: -5 per round
- ❌ Time (passive): -3 per round

**Gameplay Impact**:
- <50: All stats degrade faster
- <30: Toxicity removal stops
- <10: "Dehydration" critical debuff

---

### 12. Oxygen Level 🫁
**Range**: 0-100
**Optimal**: 90-100
**Critical Low**: <70

**Represents**:
- Blood oxygenation
- Respiratory health
- Cellular respiration

**Affects**:
- Energy production
- Immune cell effectiveness
- Brain function (decision quality)
- Heart stability

**Increased By**:
- ✅ Deep breathing: +10-20
- ✅ Aerobic exercise: +5-15
- ✅ Fresh air: +5-10
- ✅ Good posture: +5

**Decreased By**:
- ❌ Smoking: -15-25
- ❌ Pollution: -5-10
- ❌ Sedentary: -5 per round
- ❌ Stress (shallow breathing): -5-10
- ❌ High altitude: -10-20

**Gameplay Impact**:
- <80: Immune cells move slower
- <70: Energy regeneration halved
- <50: "Hypoxia" emergency state

---

### 13. Heart Stability ❤️‍🩹
**Range**: 0-100
**Optimal**: 80-100
**Danger Zone**: <50
**Critical**: <30

**Represents**:
- Cardiac rhythm
- Heart muscle health
- Cardiovascular resilience

**Affects**:
- Blood pressure stability
- Oxygen delivery
- Immune cell deployment speed
- Resistance to pressure damage

**Increased By**:
- ✅ Aerobic exercise: +10-20
- ✅ Stress reduction: +10-15
- ✅ Omega-3: +5-10
- ✅ Good sleep: +5-10

**Decreased By**:
- ❌ High blood pressure: -5 per round
- ❌ Smoking: -10-20
- ❌ Stress: -10-15
- ❌ Sedentary: -5 per round
- ❌ Poor diet: -5-10

**Gameplay Impact**:
- <50: Blood pressure spikes more frequent
- <30: "Arrhythmia" events occur
- <10: Risk of "Cardiac Event" game over

---

### 14. Hormone Balance ⚖️
**Range**: 0-100
**Optimal**: 60-80
**Danger Zone**: <40 or >80

**Represents**:
- Endocrine system health
- Hormonal regulation
- Metabolic signaling

**Affects**:
- Stress management
- Sleep quality
- Metabolism
- Immune function
- Mood stability

**Increased By**:
- ✅ Consistent sleep: +10-20
- ✅ Balanced diet: +10-15
- ✅ Exercise: +5-15
- ✅ Stress reduction: +10-15

**Decreased By**:
- ❌ Poor sleep: -10-20
- ❌ Chronic stress: -10-15
- ❌ Junk food: -5-10
- ❌ Sedentary: -5 per round

**Gameplay Impact**:
- Imbalanced hormones = erratic stat changes
- Affects random event probabilities
- Influences card effectiveness

---

## Component Interconnections

### Chain Reaction Examples

**Negative Spiral**:
```
Stress ↑ (60+)
  → Sleep Quality ↓ (40-)
    → Immune Strength ↓ (50-)
      → Inflammation ↑ (50+)
        → Cancer Growth ↑ (30%)
          → More Stress ↑ (70+)
            → Blood Pressure ↑ (140+)
              → Hypertension Risk ↑
```

**Positive Synergy**:
```
Exercise (30 min)
  → Energy ↑ (+15)
  → Insulin Efficiency ↑ (+15)
  → Stress ↓ (-20)
    → Sleep Quality ↑ (+10)
      → Immune Strength ↑ (+15)
        → Inflammation ↓ (-15)
          → All Disease Progression ↓ (20%)
```

**Diabetes Chain**:
```
Sugary Food
  → Blood Sugar ↑ (+40)
    → Insulin Efficiency ↓ (-10)
      → Energy ↓ (-10)
        → Immune Attack Speed ↓ (20%)
          → Diabetes Nodes Spawn
            → More Sugar Enemies
              → Blood Sugar ↑ (+20)
                → LOOP CONTINUES
```

**Hypertension Chain**:
```
Work Stress Event
  → Stress ↑ (+25)
    → Blood Pressure ↑ (+20)
      → Heart Stability ↓ (-15)
        → Pressure Waves Spawn
          → Immune Cells Damaged
            → Immune Strength ↓ (-10)
              → More Stress ↑ (+10)
                → LOOP CONTINUES
```

**Cancer Chain**:
```
Smoking Event
  → Toxicity ↑ (+25)
  → Inflammation ↑ (+20)
    → Immune Strength ↓ (-15)
      → Cancer Mutation Rate ↑ (50%)
        → Mutated Cells Spawn
          → Inflammation ↑ (+10)
            → LOOP CONTINUES
```

## UI Representation

### Component Display
- **Color-coded bars**: Green (optimal), Yellow (warning), Red (danger)
- **Trend arrows**: ↑ increasing, ↓ decreasing, → stable
- **Interconnection lines**: Show active relationships
- **Threshold markers**: Visual indicators for danger zones
- **Tooltip details**: Hover for exact values and effects

### Critical Warnings
- **Flashing indicators**: When component enters danger zone
- **Audio cues**: Different sounds for each component type
- **Screen effects**: Visual feedback (red tint for pressure, blur for low oxygen)
- **Prediction arrows**: Show projected changes based on current trends

### Educational Tooltips
- **Simple explanations**: What the component represents
- **Current effects**: How it's affecting gameplay right now
- **Improvement tips**: Suggestions for raising/lowering values
- **Real-world context**: Brief health education
