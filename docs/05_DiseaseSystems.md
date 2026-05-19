# Disease Systems

## Overview
Tiga penyakit kronis utama dengan mekanisme gameplay yang berbeda, mencerminkan karakteristik biologis masing-masing penyakit.

---

## Diabetes System

### Gameplay Identity
**"Snowball Overload"**
- Semakin lama gula tinggi → semakin brutal
- Overwhelming enemy numbers
- Metabolic collapse theme

### Disease Progression

**Stage 1: Pre-Diabetes (0-25%)**
- **Trigger**: Blood sugar >120 for 3+ rounds
- **Symptoms**:
  - Occasional sugar blob spawns
  - Mild insulin resistance
  - Energy fluctuations

**Enemy Types**:
- 🍬 Sugar Blobs (Basic): Slow, weak, numerous
  - Health: 50
  - Damage: 5
  - Speed: Slow
  - Special: Splits into 2 when killed

**Spawn Rate**: 1 enemy per 5 seconds
**Node Count**: 1 glucose node

---

**Stage 2: Type 2 Diabetes (26-50%)**
- **Trigger**: Blood sugar >140 for 5+ rounds OR insulin efficiency <60
- **Symptoms**:
  - Frequent sugar enemy waves
  - Insulin resistance zones appear
  - Energy drain accelerates
  - Glucose puddles form (slow zones)

**New Enemy Types**:
- 🍯 Glucose Crystals (Medium): Tanky, moderate damage
  - Health: 150
  - Damage: 15
  - Speed: Medium
  - Special: Creates glucose puddles on death

- 💉 Insulin Blockers (Support): Debuff immune cells
  - Health: 80
  - Damage: 10
  - Speed: Fast
  - Special: Reduces immune attack speed by 30%

**Spawn Rate**: 2 enemies per 5 seconds
**Node Count**: 2-3 glucose nodes
**Environmental Effects**:
- Glucose puddles: -50% movement speed
- Insulin resistance zones: -20% immune damage

---

**Stage 3: Advanced Diabetes (51-75%)**
- **Trigger**: Blood sugar >160 for 8+ rounds OR insulin efficiency <40
- **Symptoms**:
  - Overwhelming enemy swarms
  - Metabolic chaos
  - Multiple organ stress
  - Vision impairment (screen blur effect)

**New Enemy Types**:
- 🧪 Metabolic Parasites (Elite): Fast, high damage
  - Health: 200
  - Damage: 30
  - Speed: Very Fast
  - Special: Drains energy from immune cells

- 🌊 Sugar Tsunami (Swarm): Wave attacks
  - Health: 50 each
  - Damage: 8 each
  - Speed: Medium
  - Special: Comes in waves of 10-15

**Spawn Rate**: 4 enemies per 5 seconds
**Node Count**: 4-5 glucose nodes
**Environmental Effects**:
- Glucose flood: Entire arena slowed
- Insulin shutdown: Immune regen -50%
- Nerve damage: Random control disruption

---

**Stage 4: Critical Diabetes (76-100%)**
- **Trigger**: Blood sugar >180 OR insulin efficiency <20
- **Symptoms**:
  - System-wide failure
  - Ketoacidosis events
  - Organ damage
  - Imminent game over

**Boss Enemy**:
- 👹 **The Glucose Titan**
  - Health: 5000
  - Damage: 50
  - Speed: Slow
  - Special Abilities:
    - **Sugar Flood**: Fills arena with glucose (massive slow)
    - **Insulin Nullify**: Disables all immune buffs for 10s
    - **Metabolic Collapse**: AOE damage pulse
    - **Sweet Death**: Instant kill zone around boss
  - **Phases**:
    - Phase 1 (100-66% HP): Spawns sugar blobs
    - Phase 2 (66-33% HP): Creates glucose puddles
    - Phase 3 (33-0% HP): Berserk mode, faster attacks

**Spawn Rate**: 6+ enemies per 5 seconds
**Node Count**: 6-8 glucose nodes
**Environmental Effects**:
- Arena-wide glucose coating
- All immune cells slowed by 70%
- Energy drain: -10 per second
- Vision severely impaired

---

### Diabetes Mechanics

**Glucose Buildup**:
- High blood sugar creates glucose nodes
- Nodes spawn sugar enemies continuously
- Destroying nodes temporarily stops spawns
- Nodes regenerate if blood sugar stays high

**Insulin Resistance**:
- Reduces effectiveness of blood sugar reduction
- Makes it harder to control diabetes
- Progressive mechanic (gets worse over time)
- Can be reversed with sustained lifestyle changes

**Metabolic Drain**:
- Diabetes enemies drain energy on hit
- Low energy = weaker immune response
- Creates negative feedback loop
- Requires energy management strategy

**Visual Design**:
- Sticky, syrupy aesthetic
- Golden/amber color palette
- Crystallized sugar particles
- Thick, viscous movement

---

## Hypertension System

### Gameplay Identity
**"Pressure Chaos"**
- Sudden spikes and bursts
- Environmental instability
- Cardiovascular stress theme

### Disease Progression

**Stage 1: Pre-Hypertension (0-25%)**
- **Trigger**: Blood pressure >130 for 3+ rounds
- **Symptoms**:
  - Occasional pressure waves
  - Mild vessel stress
  - Heart rate fluctuations

**Enemy Types**:
- 💥 Pressure Pulses (Basic): Fast, burst damage
  - Health: 60
  - Damage: 12
  - Speed: Fast
  - Special: Knockback on hit

**Spawn Rate**: 1 enemy per 6 seconds
**Node Count**: 1 pressure node
**Environmental Effects**:
- Minor screen shake
- Occasional pressure waves

---

**Stage 2: Stage 1 Hypertension (26-50%)**
- **Trigger**: Blood pressure >140 for 5+ rounds OR stress >60
- **Symptoms**:
  - Frequent pressure spikes
  - Vessel rupture zones
  - Heart instability
  - Headache effects (screen pulse)

**New Enemy Types**:
- 🌊 Pressure Waves (Medium): AOE knockback
  - Health: 100
  - Damage: 20
  - Speed: Medium
  - Special: Pushes all units in radius

- 🩸 Vessel Ruptures (Hazard): Environmental damage
  - Health: N/A (environmental)
  - Damage: 15 per second in zone
  - Special: Random spawn locations

**Spawn Rate**: 2 enemies per 6 seconds
**Node Count**: 2-3 pressure nodes
**Environmental Effects**:
- Pressure waves every 10 seconds
- Vessel rupture zones (3-4 active)
- Movement instability
- Random knockbacks

---

**Stage 3: Stage 2 Hypertension (51-75%)**
- **Trigger**: Blood pressure >160 for 8+ rounds OR heart stability <40
- **Symptoms**:
  - Severe pressure chaos
  - Multiple vessel ruptures
  - Arrhythmia events
  - Vision problems (red tint)

**New Enemy Types**:
- ⚡ Pressure Shocks (Elite): Instant burst damage
  - Health: 150
  - Damage: 40 (burst)
  - Speed: Very Fast
  - Special: Teleports to random location, explodes

- 🫀 Heart Strain (Boss-like): Persistent threat
  - Health: 300
  - Damage: 25
  - Speed: Slow
  - Special: Pulses damage to entire arena

**Spawn Rate**: 3 enemies per 6 seconds
**Node Count**: 4-5 pressure nodes
**Environmental Effects**:
- Constant screen shake
- Pressure waves every 5 seconds
- Vessel ruptures everywhere
- Immune cells take environmental damage
- Movement severely disrupted

---

**Stage 4: Hypertensive Crisis (76-100%)**
- **Trigger**: Blood pressure >180 OR heart stability <20
- **Symptoms**:
  - Cardiovascular collapse
  - Stroke risk
  - Organ failure
  - Imminent game over

**Boss Enemy**:
- 💀 **The Pressure Core**
  - Health: 4500
  - Damage: 60
  - Speed: Medium
  - Special Abilities:
    - **Systolic Burst**: Massive knockback + damage
    - **Diastolic Crush**: Pulls all units toward boss
    - **Cardiac Arrest**: Stuns all immune cells for 5s
    - **Vessel Collapse**: Destroys random arena sections
  - **Phases**:
    - Phase 1 (100-66% HP): Pressure waves
    - Phase 2 (66-33% HP): Vessel ruptures
    - Phase 3 (33-0% HP): Constant chaos

**Spawn Rate**: 5+ enemies per 6 seconds
**Node Count**: 6-8 pressure nodes
**Environmental Effects**:
- Arena constantly unstable
- Pressure waves every 2 seconds
- Vessel ruptures cover 50% of arena
- Random unit displacement
- Heart attack events (instant damage)

---

### Hypertension Mechanics

**Pressure Wave System**:
- Periodic AOE knockback
- Frequency increases with blood pressure
- Disrupts unit positioning
- Can push units into hazards
- Visual telegraph (red pulse)

**Vessel Rupture Zones**:
- Random spawn locations
- Damage over time to units inside
- Blocks movement paths
- Heals over time if blood pressure drops
- Visual: Cracked, bleeding vessels

**Instability Mechanic**:
- High blood pressure = chaotic battlefield
- Units move erratically
- Attacks miss more often
- Abilities have longer cooldowns
- Creates frustration (intentional design)

**Heart Stability**:
- Separate stat from blood pressure
- Degrades with sustained hypertension
- Low stability = arrhythmia events
- Can cause sudden damage spikes
- Requires specific cards to restore

**Visual Design**:
- Red and dark purple palette
- Pulsing, throbbing effects
- Cracked vessel textures
- Intense screen shake
- Pressure distortion effects

---

## Cancer System

### Gameplay Identity
**"Unpredictable Mutation"**
- Enemies evolve and adapt
- Exponential growth
- Cellular chaos theme

### Disease Progression

**Stage 1: Cellular Damage (0-25%)**
- **Trigger**: Toxicity >40 OR inflammation >50 for 3+ rounds
- **Symptoms**:
  - Occasional mutated cells
  - DNA damage
  - Immune confusion

**Enemy Types**:
- 🦠 Damaged Cells (Basic): Weak, but evolves
  - Health: 70
  - Damage: 8
  - Speed: Slow
  - Special: 10% chance to mutate on hit

**Spawn Rate**: 1 enemy per 8 seconds
**Node Count**: 1 mutation node
**Mutation Chance**: 10%

---

**Stage 2: Tumor Formation (26-50%)**
- **Trigger**: Toxicity >60 OR inflammation >70 for 5+ rounds
- **Symptoms**:
  - Rapid cell division
  - Tumor nodes appear
  - Immune evasion
  - Angiogenesis (blood vessel growth to tumors)

**New Enemy Types**:
- 🧬 Mutated Cells (Medium): Adaptive
  - Health: 120
  - Damage: 15
  - Speed: Medium
  - Special: Gains resistance to damage type that hit it

- 🔬 Cloning Cells (Support): Spawns copies
  - Health: 80
  - Damage: 5
  - Speed: Slow
  - Special: Spawns 1 copy every 10 seconds

**Spawn Rate**: 2 enemies per 8 seconds
**Node Count**: 2-3 mutation nodes
**Mutation Chance**: 25%
**Environmental Effects**:
- Tumor zones: Spawns extra enemies
- Angiogenesis: Heals cancer enemies

---

**Stage 3: Metastasis (51-75%)**
- **Trigger**: Toxicity >80 OR cancer progression >50%
- **Symptoms**:
  - Cancer spreads to multiple organs
  - Rapid mutation
  - Immune system overwhelmed
  - Cachexia (energy drain)

**New Enemy Types**:
- 🧪 Metastatic Cells (Elite): Teleports
  - Health: 200
  - Damage: 25
  - Speed: Fast
  - Special: Teleports to random location, spawns clone

- 🧫 Tumor Mass (Tank): Massive health
  - Health: 500
  - Damage: 20
  - Speed: Very Slow
  - Special: Spawns enemies continuously

- 🦠 Resistant Strain (Elite): Immune to damage types
  - Health: 150
  - Damage: 30
  - Speed: Medium
  - Special: Immune to 2 random damage types

**Spawn Rate**: 4 enemies per 8 seconds
**Node Count**: 4-6 mutation nodes
**Mutation Chance**: 50%
**Environmental Effects**:
- Multiple tumor zones
- Angiogenesis network
- Immune suppression zones (-30% immune damage)
- Mutation aura (enemies evolve faster)

---

**Stage 4: Terminal Cancer (76-100%)**
- **Trigger**: Toxicity >90 OR cancer progression >75%
- **Symptoms**:
  - System-wide metastasis
  - Organ failure
  - Immune collapse
  - Imminent game over

**Boss Enemy**:
- 🧬 **The Mutated Colony**
  - Health: 6000 (regenerates)
  - Damage: 45
  - Speed: Medium
  - Special Abilities:
    - **Rapid Mutation**: Changes form every 30s
    - **Cell Division**: Spawns mini-bosses
    - **Immune Evasion**: Invisible to immune cells periodically
    - **Metastatic Burst**: Teleports around arena
    - **Adaptive Evolution**: Gains resistance to damage
  - **Phases**:
    - Phase 1: Tumor form (slow, tanky)
    - Phase 2: Metastatic form (fast, spawns clones)
    - Phase 3: Resistant form (immune to most damage)
    - Phase 4: Chaos form (random abilities)

**Spawn Rate**: 6+ enemies per 8 seconds
**Node Count**: 8-10 mutation nodes
**Mutation Chance**: 75%
**Environmental Effects**:
- Arena covered in tumors
- Constant enemy spawning
- Immune suppression everywhere
- Mutation storms (mass evolution)
- Angiogenesis heals all cancer enemies

---

### Cancer Mechanics

**Mutation System**:
- Enemies can evolve during battle
- Mutations grant new abilities:
  - Increased health
  - Increased damage
  - New attack patterns
  - Resistances
  - Special abilities
- Visual change when mutated
- Unpredictable threat escalation

**Cloning/Division**:
- Some enemies spawn copies
- Exponential growth if not controlled
- Creates overwhelming situations
- Requires AOE damage to counter
- Visual: Mitosis animation

**Tumor Nodes**:
- Act as enemy spawn points
- Grow over time if not destroyed
- Heal nearby cancer enemies
- Spread to new locations
- Priority targets for player

**Angiogenesis**:
- Blood vessels grow toward tumors
- Provides healing to cancer enemies
- Can be cut off by destroying vessels
- Visual: Red veins spreading

**Immune Evasion**:
- Some cancer cells hide from immune system
- Become invisible or untargetable
- Requires special units (NK Cells) to detect
- Frustrating but realistic mechanic

**Metastasis**:
- Cancer spreads to new organs/arenas
- Opens multiple battlefronts
- Requires strategic prioritization
- Late-game challenge

**Visual Design**:
- Dark purple and sickly green palette
- Organic, grotesque aesthetic
- Pulsing tumor masses
- Chaotic, unpredictable animations
- Mutation visual effects (DNA strands, cell division)

---

## Disease Progression Mechanics

### Progression Triggers

**Diabetes**:
- Primary: High blood sugar (sustained)
- Secondary: Low insulin efficiency
- Tertiary: Poor diet choices, sedentary lifestyle

**Hypertension**:
- Primary: High stress (sustained)
- Secondary: High blood pressure
- Tertiary: Poor cardiovascular health, smoking

**Cancer**:
- Primary: High toxicity (sustained)
- Secondary: High inflammation
- Tertiary: Smoking, poor diet, genetic factors

### Progression Speed

**Base Progression**:
- +1% per round if trigger conditions met
- +0.5% passive if above threshold
- -0.5% per round if conditions improved

**Accelerated Progression**:
- Multiple triggers active: +2% per round
- Critical body component: +3% per round
- Synergy with other diseases: +1.5% per round

**Slowed Progression**:
- Optimal body components: -1% per round
- Specific counter cards: -2% per round
- Immune system strong: -0.5% per round

### Disease Meter UI

**Visual Representation**:
```
Diabetes:  [████████░░] 80% ⚠️ CRITICAL
Hypertension: [████░░░░░░] 40% ⚠️ WARNING
Cancer:    [██░░░░░░░░] 20% ✓ STABLE
```

**Color Coding**:
- 0-25%: Green (Safe)
- 26-50%: Yellow (Warning)
- 51-75%: Orange (Danger)
- 76-100%: Red (Critical)

**Threshold Markers**:
- 25%: Stage 1 activation
- 50%: Stage 2 activation
- 75%: Stage 3 activation
- 80%: Boss warning
- 100%: Game over

---

## Disease Evolution

### Adaptive Difficulty

**Enemy Scaling**:
- Health: +5% per stage
- Damage: +10% per stage
- Speed: +5% per stage
- Abilities: Unlock new abilities per stage

**Environmental Scaling**:
- Hazard frequency: +20% per stage
- Hazard damage: +15% per stage
- Safe zones: -10% per stage

### Disease Synergies

**Diabetes + Hypertension**:
- "Metabolic Syndrome"
- Both diseases progress 25% faster
- Shared enemies (cardiovascular damage)
- Combined environmental effects

**Diabetes + Cancer**:
- "Glucose Feeding"
- High blood sugar accelerates cancer growth
- Cancer enemies gain energy drain
- Tumor growth rate increased

**Hypertension + Cancer**:
- "Stress-Induced Mutation"
- High stress increases mutation rate
- Pressure waves damage DNA
- Cancer spreads faster

**All Three Active**:
- "System Collapse"
- All diseases progress 50% faster
- Boss enemies can spawn simultaneously
- Environmental chaos
- Near-impossible difficulty

---

## Disease Mutation

### Mutation Types

**Diabetes Mutations**:
- **Insulin Immunity**: Enemies immune to insulin effects
- **Sugar Absorption**: Enemies heal from glucose
- **Metabolic Chaos**: Random stat changes

**Hypertension Mutations**:
- **Pressure Resistance**: Enemies unaffected by knockback
- **Vessel Corruption**: Enemies spawn from ruptures
- **Cardiac Mimicry**: Enemies pulse like heart

**Cancer Mutations**:
- **Rapid Division**: Double spawn rate
- **Multi-Resistance**: Immune to multiple damage types
- **Metastatic Leap**: Teleportation ability
- **Angiogenic**: Self-healing
- **Stem Cell**: Revives once

### Mutation Triggers

**Random Mutations**:
- 5% chance per enemy spawn (base)
- Increased by high inflammation
- Increased by high toxicity
- Increased by disease stage

**Triggered Mutations**:
- Enemy survives >30 seconds: Mutate
- Enemy takes >500 damage: Mutate
- Enemy kills immune cell: Mutate
- Boss phase transition: Mutate

---

## Disease Buffs

### Buff Sources

**Environmental**:
- Disease nodes provide buffs to nearby enemies
- Hazard zones buff enemies inside
- Boss presence buffs all enemies

**Body Component-Based**:
- High blood sugar: Diabetes enemies +20% health
- High stress: Hypertension enemies +30% damage
- High toxicity: Cancer enemies +50% mutation rate

**Synergy-Based**:
- Multiple diseases active: All enemies +15% stats
- Critical body component: Enemies +25% stats
- Low immune strength: Enemies +20% stats

### Buff Types

**Stat Buffs**:
- Health increase
- Damage increase
- Speed increase
- Armor/resistance

**Ability Buffs**:
- Cooldown reduction
- Range increase
- AOE increase
- Duration increase

**Special Buffs**:
- Regeneration
- Immunity to debuffs
- Reflect damage
- Lifesteal

---

## Disease Spread

### Spread Mechanics

**Node Spreading**:
- Existing nodes spawn new nodes
- Spread rate based on disease progression
- Can be prevented by destroying nodes quickly
- Visual: Corruption spreading across arena

**Arena Corruption**:
- Disease gradually corrupts battlefield
- Corrupted areas buff enemies
- Corrupted areas debuff immune cells
- Can be cleansed with specific cards

**Organ Infection**:
- Diseases spread to new organs/arenas
- Each organ has different mechanics
- Multi-front battles in late game
- Strategic prioritization required

### Containment Strategies

**Node Destruction**:
- Priority targeting
- AOE damage effective
- Temporary spawn prevention
- Rewards for destruction

**Corruption Cleansing**:
- Specific lifestyle cards
- Immune cell abilities
- Time-based decay (if conditions improve)
- Visual feedback for cleansing

**Disease Reversal**:
- Sustained healthy lifestyle
- Specific card combinations
- Immune system strengthening
- Possible but difficult
