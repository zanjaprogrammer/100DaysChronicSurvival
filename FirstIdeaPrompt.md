Hello kiro good afternoon, im thinking of making an educational game using unity about surviving 100 days from 3 chronic disease: cancer, diabetes melitus, and hypertension

here is the overall idea of the game, you can expand however you want if there's some area/component of the game that you think it could be improved on 

hmmm oke sepertinya roguelike/survivor tapi dicampur dengan gameplay ala TABS bagus, nah ideku seperti ini 

user mengatur army immune sel melawan pathogen/penyakit tersebut di sebuah arena/battleground

dan penyakit ini perlahan akan muncul dan immunity cell ini akan otomatis menyerang penyakitnya

nah di setiap round selesai, tubuh player ini akan mengambil keputusan random yang akan ngeboost health atau mengambil keputusan buruk yang akan menurunkan kesehatan, misal seperti merokok, minum minuman gula banyak, dll atau tidur cepat, olahraga, dll

alhasil jika tubuh melakukan keputusan buruk imunitas tubuh bisa menurun kekuatannya atau penyakit bisa mendapat buff, 

agar mekanisme ini works dengan bagus maka kita butuh banyak komponen di tubuh yang harus kita manage agar lebih interesting gamenya, misal energy, insulin, etc, jangan cuma health saja karena kita mau agar edukasi simple tapi tidak terlalu simple

untuk komponen komponen di tubuh ini kamu beri ide lagi dong, dan cara untuk memanage komponen komponen tersebut yaitu dengan mekanisme ini:

setiap waktu yang ditentukan user akan diberi 3 pilihan berupa action card pilihan pilihan positif yang bisa ngeboost/menguatkan imun, dan di card ini akan ada indicator komponen tubuh mana yang di boost dan efek apa yang didapatkan oleh sistem imun tubuh

goalnya user yaitu menjaga agar tubuh tidak mendapat salah satu dari ketiga penyakit itu, nanti di ui user akan ada progress bar yang menunjukkan berapa banyak pathogen penyakit itu ada di tubuh user, jika melewati suatu angka maka dinyatakan tubuh terkena penyakit


sistem gameplaynya berarti survive as long as possible with the mechanics of an action card, and every round ends there will be a random action that the body takes that the user cannot control

CORE CONCEPT
Genre
Roguelike Survival
Auto Battler
Strategy
Card Management
HIGH CONCEPT

Pemain mengelola tubuh manusia dari serangan penyakit kronis:

Diabetes Melitus
Hipertensi
Kanker

Tubuh direpresentasikan sebagai battlefield biologis.

Pemain:

menyusun pasukan sistem imun,
menjaga kondisi tubuh,
mengambil keputusan gaya hidup,
dan bertahan selama mungkin sebelum salah satu penyakit berkembang terlalu parah.
CORE GAMEPLAY LOOP
1. Battle Phase

Di arena:

pathogen muncul bertahap,
sel imun menyerang otomatis,
player hanya mengatur positioning/build/loadout.

Mirip:

auto battler,
survivor,
sedikit RTS ringan.
2. Lifestyle Decision Phase

Setelah round selesai:
Player diberi:

3 Action Cards positif

Contoh:

Tidur cukup
Jogging
Minum air putih
Makan sayur
Meditasi
Berhenti merokok

Setiap card:

meningkatkan komponen tubuh tertentu,
memberi buff ke imun,
atau melemahkan penyakit.
3. Random Body Behavior Event

Setelah player memilih:
Tubuh melakukan keputusan random yang tidak bisa dikontrol.

Contoh negatif:

Craving gula
Begadang
Stress kerja
Junk food
Merokok

Contoh positif:

Jalan kaki
Tidur cepat
Minum air

Ini penting banget karena:

menciptakan tension,
player tidak bisa perfect,
ada elemen “real life unpredictability”.


KONDISI MENANG & KALAH
Goal

Survive for 100 days

Lose Condition

Jika salah satu disease meter mencapai batas tertentu:

Diabetes Activated
Hypertension Critical
Cancer Developed

Game over.

YANG PALING PENTING:
BODY COMPONENT SYSTEM

Ini inti strategi game-mu.

Jangan cuma HP.

Tubuh punya banyak “status biologis” yang saling terhubung.

IDE BODY COMPONENTS
1. Energy ⚡

Representasi:

stamina tubuh,
kualitas metabolisme.

Dipakai untuk:

regenerasi imun,
attack speed sel imun.

Turun karena:

begadang,
stress,
makanan buruk.
2. Blood Sugar 🍬

SUPER penting untuk diabetes.

Terlalu tinggi:

memperkuat diabetes pathogen,
memperlambat imun.

Terlalu rendah:

tubuh lemah.

Player harus menjaga stabil.

3. Blood Pressure ❤️

Untuk hipertensi.

Jika terlalu tinggi:

damage internal,
pembuluh darah rusak,
sel imun melemah.
4. Immune Strength 🛡️

Stat utama pasukan imun.

Mempengaruhi:

damage,
attack speed,
regen,
jumlah spawn imun.
5. Stress 😵

Ini bagus banget untuk gameplay.

Semakin tinggi stress:

hipertensi naik,
cancer mutation naik,
energy turun.
6. Sleep Quality 🌙

Bisa jadi hidden MVP mechanic.

Kalau buruk:

semua stat pelan-pelan rusak.
7. Inflammation 🔥

Ini keren karena bisa menghubungkan SEMUA penyakit.

Inflammation tinggi:

kanker berkembang,
diabetes memburuk,
hipertensi memburuk.

Ini bisa jadi “global danger stat”.

8. Insulin Efficiency 💉

Khusus diabetes.

Kalau turun:

gula makin sulit dikontrol.
9. Toxicity ☣️

Dari:

rokok,
junk food,
polusi.

Meningkatkan:

mutation,
cancer growth.
BAGIAN TERKEREN:
SEMUA KOMPONEN SALING TERHUBUNG

Contoh:

Stress ↑
→ Sleep ↓
→ Immune ↓
→ Inflammation ↑
→ Cancer Growth ↑

atau

Sugar ↑
→ Insulin Efficiency ↓
→ Energy ↓
→ Immune Attack Speed ↓

Ini bikin edukasinya terasa natural.

Player akan BELAJAR sendiri:
“Oh ternyata begadang bikin semuanya jelek.”

Tanpa perlu dipaksa membaca teks edukasi.


ACTION CARD SYSTEM


Struktur Card
Jogging 30 Minutes

Effects:

Energy
Insulin Efficiency
Stress

Immune Bonus:
+10% attack speed

Drink Sugary Soda

Effects:

Temporary Energy
Blood Sugar
Insulin Efficiency

Disease Buff:
Diabetes pathogen evolves faster

Deep Sleep

Effects:

Sleep Quality
Inflammation
Immune Regen
AGAR LEBIH “ROGUELIKE”

Tambahkan:

rarity card,
synergy,
build specialization.
CONTOH BUILD PLAYER
Healthy Lifestyle Build

Fokus:

sleep,
olahraga,
anti inflammation.

Tanky sustain build.

Aggressive Immune Build

Fokus:

attack speed imun,
immune multiplication.

High risk high reward.

Metabolism Control Build

Fokus:

insulin,
sugar control.

Counter diabetes.

RANDOM EVENTS YANG MENARIK
“Office Deadline”
Stress
Sleep
“Family Vacation”
Stress
Happiness
Energy
“Fast Food Discount”

Pilihan:

ambil buff cepat,
atau tetap disiplin.
IDE UNIT IMUN
Macrophage

Tank melee.

T-Cell

Single target killer.

B-Cell

Ranged antibody support.

NK Cell

Anti-cancer specialist.

TIAP PENYAKIT PUNYA GAYA MAIN BERBEDA
Diabetes

“Snowball disease”
Semakin lama gula tinggi → makin brutal.

Hipertensi

“Pressure mechanic”
Makin tinggi pressure → battlefield makin chaos.

Kanker

“Mutation mechanic”
Enemy evolve terus.




untuk mekanisme bagaimana masing masing penyakit akan muncul di battleground yaitu seperti ini:

saat user pertama kali memainkan game ini, tubuh user akan mendapatkan beberapa komponen bawaan dari ortu/gen misal meningkatkan resiko diabetes (spawn rate, strength of the disease), immunity boost terhadap penyakit tertentu, etc

lalu setelah user mendapatkan buff/debuff bawaan tersebut, game akan mentrigger event random pertama yang akan memicu pelonjakan salah satu dari ketiga penyakit tersebut, misal lifestyle buruk makan junk food banyak, nah ini yang akan menjadi obstacle pertama yang akan dihadapi user, seiring berjalannya game, dengan mekanisme random event, penyakit lain bisa mulai berkembang dan muncul di tubuh

jadi sebelum game dimulai, user diberi case random tubuh dan lifestyle yang dimainkan oleh user

di game ini akan ada komponen lagi yaitu disease risk (diabetes, hypertension, and cancer) semakin banyak disease risk terisi maka semakin banyak nodes yang akan muncul di battleground 

node ini semacam spawn point dari sel sel penyakit tersebut:

Contoh
Diabetes

Jika Blood Sugar tinggi:

node “glucose buildup” muncul,
spawn sugar parasites,
memperlambat imun.
Hipertensi

Jika stress tinggi:

node “high pressure vessel” muncul,
area menghasilkan shockwave,
pembuluh pecah spawn micro-damage enemy.
Kanker

Jika toxicity tinggi:

node mutation muncul,
menghasilkan sel kanker baru,
bisa evolve.


semakin disease progression meter terisi semakin kuat juga attack, health, properties, etc dari penyakit penyakit tersebut:

Setiap penyakit punya progress bar.

Semakin tinggi:

spawn rate meningkat,
enemy baru unlock,
mutation terjadi.
Example
Diabetes Stage 1

Spawn:

sugar blobs

Stage 2:

insulin resistant cells

Stage 3:

metabolic beast
Cancer

Stage 1:

unstable cells

Stage 2:

cloning cells

Stage 3:

tumor titan

Ini bikin sense progression kuat.

ENEMY SPAWN TIAP PENYAKIT HARUS BERBEDA

Ini SUPER penting.

DIABETES
Gameplay Identity:

“Overload”

Spawn behavior:

enemy banyak,
makin lama makin overwhelming.

Mechanic:

sugar puddle,
slow area,
insulin drain.

Feel:
Tubuh kewalahan metabolisme.

HIPERTENSI
Gameplay Identity:

“Pressure”

Spawn behavior:

ledakan tekanan,
shockwave,
burst enemy.

Mechanic:

area sempit,
knockback,
sudden spike.

Feel:
Tubuh tegang dan tidak stabil.

KANKER
Gameplay Identity:

“Mutation”

Spawn behavior:

enemy evolve,
split,
clone,
adapt.

Mechanic:

random mutation,
resistant enemies.

Feel:
penyakit sulit diprediksi.

6. FINAL EVOLUTION / BOSS SYSTEM

Kalau disease meter terlalu tinggi:
boss muncul.

Diabetes Boss

“The Glucose Titan”

Arena dipenuhi sugar flood.

Hypertension Boss

“The Pressure Core”

Pulse shockwave seluruh map.

Cancer Boss

“The Mutated Colony”

Boss terus evolve saat fight.

REKOMENDASI TERBAIK

Menurutku sistem paling kuat:

COMBINATION:
Infection Node
Disease Progression
Lifestyle-triggered Spawn

Karena:

jelas secara visual,
tactical,
edukatif,
dan seru.

