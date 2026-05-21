# 100 Days Chronic Survival — Game Design Document

## High Concept

Game **survival-strategy edukasi** tentang menjaga tubuh manusia agar tidak terkena 3 penyakit kronis: **Diabetes Melitus**, **Hipertensi**, dan **Kanker**.

Player **bukan** mengontrol karakter secara langsung, tetapi **mengelola kondisi tubuh manusia** melalui *decision making*, *body component management*, dan *chain reaction biologis*.

**Battle antar sel hanya menjadi visual feedback otomatis** untuk menunjukkan kondisi tubuh pemain — bukan gameplay utama.

> **Identitas Game:** "Survival simulation tentang tubuh manusia" — bukan "game perang sel."

---

## Genre
- Roguelike Survival Management
- Strategy / Card Choice System
- Biological Simulation

---

## Core Mechanic
> "Manage the body to survive as long as possible."

Player harus menjaga berbagai komponen tubuh agar tetap stabil. Jika kondisi tubuh memburuk → penyakit berkembang → disease meter meningkat → visual tubuh rusak → game over.

**Goal:** Survive for 100 days  
**Lose Condition:** Salah satu disease meter mencapai batas kritis

---

## Gameplay Loop (Per Round / Hari)

### Phase 1: Time Progression
Waktu berjalan per hari/round. Seluruh body components berubah secara dinamis.

### Phase 2: Body Simulation (Otomatis)
Tubuh melakukan simulasi otomatis berdasarkan kondisi saat ini:
- Blood sugar naik/turun
- Stress berubah
- Immune strength berubah
- Inflammation menyebar/berkurang

### Phase 3: Card Choice
Player diberikan **3 pilihan action card** berisi keputusan gaya hidup:
- Tidur cepat, olahraga, minum air, makan sehat, meditasi, cek kesehatan
- Setiap card memengaruhi body components, memberi buff/debuff, dan memengaruhi perkembangan penyakit

### Phase 4: Random Body Behavior (Tidak bisa dikontrol)
Setelah player memilih card, tubuh melakukan tindakan random:
- **Negatif:** Craving gula, junk food, begadang, merokok, stress kerja
- **Positif:** Jalan kaki, tidur nyenyak, recovery
- Menciptakan unpredictability, tension, dan realism

### Phase 5: Chain Reaction
Semua body components saling mempengaruhi:
```
Stress ↑ → Sleep ↓ → Immune ↓ → Inflammation ↑ → Hypertension ↑
Blood Sugar ↑ → Insulin Efficiency ↓ → Energy ↓ → Diabetes Meter ↑
```

### Phase 6: Disease Progression
Setiap penyakit memiliki Disease Progress Meter. Jika kondisi tubuh buruk → meter meningkat → mencapai threshold → game over.

---

## Body Component System

| Komponen | Deskripsi | Efek jika buruk |
|----------|-----------|-----------------|
| Health | Kesehatan umum | Semua sistem melemah |
| Energy | Stamina, metabolisme | Regenerasi imun turun |
| Blood Sugar | Kadar gula darah | Diabetes berkembang |
| Blood Pressure | Tekanan darah | Hipertensi berkembang |
| Stress | Level stress | Memperburuk semua penyakit |
| Sleep Quality | Kualitas tidur | Semua stat perlahan rusak |
| Inflammation | Level peradangan | Global danger — semua penyakit memburuk |
| Immune Strength | Kekuatan sistem imun | Sel imun melemah |
| Insulin Efficiency | Efisiensi insulin | Gula darah sulit dikontrol |
| Toxicity | Level racun tubuh | Kanker berkembang, mutation meningkat |
| Hydration | Level hidrasi | Metabolisme dan energy turun |
| Metabolism | Kecepatan metabolisme | Efisiensi tubuh menurun |

**Semua komponen saling terhubung** — ini inti edukasi game.

---

## Disease System

### Diabetes — "Overload"
- **Trigger:** Blood Sugar tinggi, Insulin Efficiency rendah
- **Visual:** Sugar sludge, glucose corruption, sticky bloodstream
- **Spawn:** Sugar blobs → Insulin resistant cells → Metabolic beast
- **Feel:** Tubuh perlahan kewalahan metabolisme

### Hipertensi — "Pressure"
- **Trigger:** Blood Pressure tinggi, Stress tinggi
- **Visual:** Shockwave, vessel pressure, red pulse effects
- **Spawn:** Pressure nodes, burst enemies, micro-damage
- **Feel:** Tubuh tegang dan tidak stabil

### Kanker — "Mutation"
- **Trigger:** Toxicity tinggi, Inflammation tinggi
- **Visual:** Cancer cells, tumor growth, mutation spread
- **Spawn:** Unstable cells → Cloning cells → Tumor titan
- **Feel:** Penyakit sulit diprediksi, terus bermutasi

---

## Bloodstream Battlefield (Visual Only)

Arena berupa **aliran darah bercabang** (organic tunnels / bloodstream corridors).

**Sel HANYA bisa bergerak di area pembuluh darah** — menggunakan pathfinding mask berdasarkan arena map.

Di arena secara otomatis:
- Sel darah merah bergerak sepanjang aliran
- Sel imun patroli otomatis
- Corruption muncul berdasarkan kondisi
- Cancer cell berkembang
- Metabolic sludge menyebar
- Pressure effect muncul

### Visual Feedback
- **Tubuh Sehat:** Aliran darah lancar, sel imun aktif, warna cerah
- **Tubuh Memburuk:** Bloodstream menggelap, corruption menyebar, sel imun melemah

---

## Pre-Game Setup
Sebelum game dimulai:
1. User diberi **case random tubuh** (genetik bawaan dari ortu/gen)
   - Meningkatkan resiko penyakit tertentu (spawn rate, strength)
   - Immunity boost terhadap penyakit tertentu
2. Game mentrigger **event random pertama** yang memicu pelonjakan salah satu penyakit
3. Seiring game berjalan, random events memicu penyakit lain berkembang

---

## Action Card System

### Struktur Card
- **Nama** (contoh: "Jogging 30 Menit")
- **Efek** terhadap body components (+Energy, -Stress, +Insulin Efficiency)
- **Immune Bonus** (+10% attack speed)
- **Rarity** (Common, Rare, Legendary)

### Contoh Build Player
- **Healthy Lifestyle Build:** Focus sleep, olahraga, anti inflammation (tanky sustain)
- **Aggressive Immune Build:** Focus attack speed, immune multiplication (high risk/reward)
- **Metabolism Control Build:** Focus insulin, sugar control (counter diabetes)

---

## Player Interaction
Player **TIDAK:**
- Mengontrol battle
- Menggerakkan unit
- Menyerang enemy

Player **HANYA:**
- Mengontrol keputusan dan kondisi tubuh melalui card choices

---

## Edukasi
Game mengajarkan secara natural melalui visual consequence dan gameplay consequence:
- Hubungan gaya hidup dengan penyakit
- Konsekuensi biologis dari keputusan kecil
- Efek jangka panjang
- Hubungan antar sistem tubuh

> Bukan lewat quiz atau teks panjang — player belajar sendiri melalui simulasi.
