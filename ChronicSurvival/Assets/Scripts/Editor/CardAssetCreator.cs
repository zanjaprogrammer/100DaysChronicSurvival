using UnityEngine;
using UnityEditor;
using ChronicSurvival.Cards;
using System.Collections.Generic;
using System.IO;

namespace ChronicSurvival.Editor
{
    public static class CardAssetCreator
    {
        const string CardsFolder = "Assets/Resources/Cards";
        const string DatabasePath = "Assets/Resources/ActionCardDatabase.asset";

        [MenuItem("ChronicSurvival/Create All Action Cards")]
        public static void CreateAllActionCards()
        {
            EnsureFolder(CardsFolder);

            var created = new List<ActionCard>();

            // ── Common lifestyle (GDD) ──
            created.Add(Make("Jogging30Menit", "Jogging 30 Menit",
                "Lari ringan meningkatkan sirkulasi dan menurunkan tekanan darah.",
                CardRarity.Common, CardType.Lifestyle, 1, 0.08f, 0f, 0f, 0f,
                Eff("Energy", 8), Eff("BloodPressure", -6), Eff("Stress", -4), Eff("Metabolism", 5)));

            created.Add(Make("MinumAir", "Minum Air Cukup",
                "Hidrasi optimal membantu metabolisme dan detoksifikasi ringan.",
                CardRarity.Common, CardType.Lifestyle, 1,
                Eff("Hydration", 18), Eff("Toxicity", -4), Eff("Energy", 4)));

            created.Add(Make("MakanSehat", "Makan Sehat",
                "Porsi sayur, protein, dan serat menstabilkan gula darah.",
                CardRarity.Common, CardType.Lifestyle, 1,
                Eff("BloodSugar", -8), Eff("Inflammation", -5), Eff("Energy", 6), Eff("InsulinEfficiency", 4)));

            created.Add(Make("TidurCepat", "Tidur Lebih Awal",
                "Tidur cukup memulihkan energi dan menurunkan stres.",
                CardRarity.Common, CardType.Lifestyle, 1, 0f, 0f, 0.05f, 0f,
                Eff("SleepQuality", 14), Eff("Energy", 10), Eff("Stress", -8), Eff("ImmuneStrength", 3)));

            created.Add(Make("OlahragaRingan", "Olahraga Ringan",
                "Gerakan tubuh ringan tanpa membebani jantung.",
                CardRarity.Common, CardType.Lifestyle, 1,
                Eff("Metabolism", 7), Eff("Stress", -6), Eff("HeartStability", 5), Eff("BloodPressure", -4)));

            created.Add(Make("BalancedMeal", "Makan Seimbang",
                "Nutrisi seimbang untuk metabolisme stabil.",
                CardRarity.Common, CardType.Lifestyle, 1,
                Eff("Energy", 10), Eff("BloodSugar", -5), Eff("Metabolism", 5)));

            created.Add(Make("Hydration", "Hidrasi Penuh",
                "Minum cukup air sepanjang hari.",
                CardRarity.Common, CardType.Lifestyle, 1,
                Eff("Hydration", 20), Eff("Toxicity", -5), Eff("Energy", 5)));

            created.Add(Make("DeepSleep", "Tidur Nyenyak",
                "8 jam tidur berkualitas untuk regenerasi tubuh.",
                CardRarity.Common, CardType.Lifestyle, 1, 0f, 0f, 0.08f, 0f,
                Eff("SleepQuality", 15), Eff("Energy", 15), Eff("Stress", -10), Eff("ImmuneStrength", 5)));

            created.Add(Make("CardioExercise", "Latihan Kardio",
                "Olahraga kardio 30 menit menguatkan jantung.",
                CardRarity.Common, CardType.Lifestyle, 1, 0.06f, 0f, 0f, 0f,
                Eff("HeartStability", 10), Eff("BloodPressure", -8), Eff("Metabolism", 8), Eff("Stress", -5)));

            created.Add(Make("SarapanBergizi", "Sarapan Bergizi",
                "Sarapan tinggi protein dan serat mengontrol gula pagi.",
                CardRarity.Common, CardType.Lifestyle, 1,
                Eff("BloodSugar", -6), Eff("Energy", 8), Eff("InsulinEfficiency", 5)));

            created.Add(Make("KurangiGaram", "Kurangi Garam",
                "Mengurangi natrium menurunkan tekanan darah.",
                CardRarity.Common, CardType.Lifestyle, 1,
                Eff("BloodPressure", -10), Eff("HeartStability", 6), Eff("Hydration", -3)));

            // ── Uncommon ──
            created.Add(Make("Meditation", "Meditasi",
                "Mindfulness menurunkan stres dan tekanan darah.",
                CardRarity.Uncommon, CardType.Lifestyle, 1, 0f, 0f, 0.05f, 0f,
                Eff("Stress", -15), Eff("BloodPressure", -10), Eff("SleepQuality", 8), Eff("HormoneBalance", 5)));

            created.Add(Make("VitaminSupplement", "Suplemen Vitamin",
                "Vitamin harian mendukung sistem imun.",
                CardRarity.Uncommon, CardType.Medical, 1, 0f, 0.05f, 0f, 0f,
                Eff("ImmuneStrength", 12), Eff("Energy", 8), Eff("Inflammation", -5)));

            created.Add(Make("TehHijau", "Teh Hijau",
                "Antioksidan membantu mengurangi inflamasi dan toksin.",
                CardRarity.Uncommon, CardType.Lifestyle, 3, 0.05f, 0f, 0f, 0f,
                Eff("Toxicity", -8), Eff("Inflammation", -6), Eff("ImmuneStrength", 4)));

            created.Add(Make("YogaStretching", "Yoga & Peregangan",
                "Relaksasi otot dan penurunan stres sistemik.",
                CardRarity.Uncommon, CardType.Lifestyle, 2, 0f, 0f, 0.1f, 0f,
                Eff("Stress", -12), Eff("BloodPressure", -7), Eff("SleepQuality", 6)));

            created.Add(Make("Probiotik", "Probiotik",
                "Kesehatan usus mendukung imunitas dan inflamasi rendah.",
                CardRarity.Uncommon, CardType.Medical, 4, 0f, 0f, 0.06f, 5f,
                Eff("ImmuneStrength", 10), Eff("Inflammation", -8), Eff("Metabolism", 4)));

            created.Add(Make("MindfulBreathing", "Pernapasan Mindful",
                "Teknik pernapasan meningkatkan oksigenasi.",
                CardRarity.Uncommon, CardType.Lifestyle, 2,
                Eff("OxygenLevel", 4), Eff("Stress", -10), Eff("HeartStability", 5)));

            // ── Rare ──
            created.Add(Make("MedicalCheckup", "Cek Kesehatan",
                "Skrining medis mendeteksi risiko lebih awal.",
                CardRarity.Rare, CardType.Medical, 5, 0.1f, 0.08f, 0f, 0f,
                Eff("ImmuneStrength", 15), Eff("Inflammation", -10), Eff("Stress", -8), Eff("BloodSugar", -5)));

            created.Add(Make("IntermittentFasting", "Puasa Intermiten",
                "Siklus puasa membantu sensitivitas insulin (berisiko jika berlebihan).",
                CardRarity.Rare, CardType.Lifestyle, 7,
                Eff("InsulinEfficiency", 12), Eff("BloodSugar", -10), Eff("Energy", -5), Eff("Metabolism", 8)));

            created.Add(Make("AntiInflammatoryDiet", "Diet Anti-Inflamasi",
                "Fokus omega-3 dan rempah untuk menekan peradangan.",
                CardRarity.Rare, CardType.Lifestyle, 6, 0f, 0f, 0.12f, 0f,
                Eff("Inflammation", -15), Eff("ImmuneStrength", 8), Eff("Toxicity", -6)));

            // ── Epic / Legendary ──
            created.Add(Make("Vacation", "Liburan Santai",
                "Istirahat total dari stres kerja dan pemulihan mendalam.",
                CardRarity.Epic, CardType.Lifestyle, 10, 0.12f, 0f, 0.15f, 0f,
                Eff("Stress", -25), Eff("SleepQuality", 20), Eff("Energy", 20), Eff("HormoneBalance", 15)));

            created.Add(Make("EmergencyTreatment", "Perawatan Darurat",
                "Intervensi medis intensif menstabilkan kondisi kritis.",
                CardRarity.Legendary, CardType.Emergency, 15, 0.15f, 0.15f, 0.2f, 0f,
                Eff("ImmuneStrength", 30), Eff("Inflammation", -20), Eff("Toxicity", -15), Eff("Energy", 15)));

            created.Add(Make("ImmuneBoostProtocol", "Protokol Imun Boost",
                "Kombinasi istirahat, nutrisi, dan suplemen untuk pertahanan maksimal.",
                CardRarity.Epic, CardType.Medical, 12, 0.18f, 0.1f, 0f, 10f,
                Eff("ImmuneStrength", 20), Eff("SleepQuality", 10), Eff("Stress", -12)));

            // Debuff cards (for future random events — not drawn in lifestyle pool)
            Make("JunkFood", "Junk Food", "Makanan cepat saji tinggi gula dan lemak.",
                CardRarity.Common, CardType.Debuff, 1,
                Eff("BloodSugar", 15), Eff("Inflammation", 8), Eff("Metabolism", -5), Eff("Energy", -5)).isUnlocked = false;

            Make("Smoking", "Merokok", "Rokok merusak paru-paru dan imunitas.",
                CardRarity.Common, CardType.Debuff, 1,
                Eff("OxygenLevel", -15), Eff("Toxicity", 20), Eff("ImmuneStrength", -10), Eff("Inflammation", 10)).isUnlocked = false;

            Make("AllNighter", "Begadang", "Kurang tidur menghancurkan regenerasi.",
                CardRarity.Common, CardType.Debuff, 1,
                Eff("SleepQuality", -20), Eff("Energy", -15), Eff("Stress", 10), Eff("ImmuneStrength", -8)).isUnlocked = false;

            SaveDatabase(created);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[CardAssetCreator] Created {created.Count} lifestyle cards in {CardsFolder}");
            EditorUtility.DisplayDialog("Cards Created",
                $"Berhasil membuat {created.Count} action card di:\n{CardsFolder}\n\nDatabase: {DatabasePath}\n\nJalankan 'Wire Card System In Scene' untuk menghubungkan CardManager.",
                "OK");
        }

        [MenuItem("ChronicSurvival/Wire Card System In Scene")]
        public static void WireCardSystemInScene()
        {
            GameObject managers = GameObject.Find("_Managers");
            if (managers == null)
            {
                managers = new GameObject("_Managers");
                Undo.RegisterCreatedObjectUndo(managers, "Create Managers");
            }

            var cardManager = GetOrAdd<CardManager>(managers);
            var cardBuffManager = GetOrAdd<CardBuffManager>(managers);

            var database = AssetDatabase.LoadAssetAtPath<ActionCardDatabase>(DatabasePath);
            if (database == null)
            {
                Debug.LogWarning("[CardAssetCreator] Database not found. Run 'Create All Action Cards' first.");
            }

            var guids = AssetDatabase.FindAssets("t:ActionCard", new[] { CardsFolder });
            var cards = new List<ActionCard>();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var card = AssetDatabase.LoadAssetAtPath<ActionCard>(path);
                if (card != null && card.cardType != CardType.Debuff)
                {
                    cards.Add(card);
                }
            }

            SerializedObject so = new SerializedObject(cardManager);
            so.FindProperty("cardDatabase").objectReferenceValue = database;
            var listProp = so.FindProperty("allCards");
            listProp.ClearArray();
            for (int i = 0; i < cards.Count; i++)
            {
                listProp.InsertArrayElementAtIndex(i);
                listProp.GetArrayElementAtIndex(i).objectReferenceValue = cards[i];
            }
            so.FindProperty("autoLoadFromDatabase").boolValue = true;
            so.FindProperty("autoLoadFromResources").boolValue = true;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(cardManager);
            EditorUtility.SetDirty(cardBuffManager);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            Debug.Log($"[CardAssetCreator] Wired CardManager with {cards.Count} cards.");
            EditorUtility.DisplayDialog("Scene Wired",
                $"CardManager + CardBuffManager pada _Managers.\n{cards.Count} kartu ter-assign.", "OK");
        }

        static ActionCard Make(string fileName, string title, string desc,
            CardRarity rarity, CardType type, int unlockDay, params CardEffect[] effects)
        {
            return Make(fileName, title, desc, rarity, type, unlockDay, 0f, 0f, 0f, 0f, effects);
        }

        static ActionCard Make(string fileName, string title, string desc,
            CardRarity rarity, CardType type, int unlockDay,
            float immuneAtk, float immuneDmg = 0f, float immuneHp = 0f, float immuneStr = 0f,
            params CardEffect[] effects)
        {
            string path = $"{CardsFolder}/{fileName}.asset";
            ActionCard card = AssetDatabase.LoadAssetAtPath<ActionCard>(path);
            if (card == null)
            {
                card = ScriptableObject.CreateInstance<ActionCard>();
                AssetDatabase.CreateAsset(card, path);
            }

            card.cardName = title;
            card.description = desc;
            card.rarity = rarity;
            card.cardType = type;
            card.unlockAtDay = unlockDay;
            card.isUnlocked = true;
            card.effects = new List<CardEffect>(effects);
            card.immuneAttackSpeedBonus = immuneAtk;
            card.immuneDamageBonus = immuneDmg;
            card.immuneMaxHealthBonus = immuneHp;
            card.immuneStrengthBonus = immuneStr;

            EditorUtility.SetDirty(card);
            return card;
        }

        static CardEffect Eff(string component, float value) =>
            new CardEffect { componentName = component, value = value };

        static void SaveDatabase(List<ActionCard> cards)
        {
            ActionCardDatabase db = AssetDatabase.LoadAssetAtPath<ActionCardDatabase>(DatabasePath);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<ActionCardDatabase>();
                AssetDatabase.CreateAsset(db, DatabasePath);
            }

            db.cards = new List<ActionCard>(cards);
            EditorUtility.SetDirty(db);
        }

        static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace("\\", "/");
                string folderName = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        static T GetOrAdd<T>(GameObject go) where T : Component
        {
            T c = go.GetComponent<T>();
            if (c == null) c = Undo.AddComponent<T>(go);
            return c;
        }
    }
}
