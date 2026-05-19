using UnityEngine;
using UnityEditor;
using ChronicSurvival.Cards;
using System.Collections.Generic;

namespace ChronicSurvival.Editor
{
    public class CardAssetCreator
    {
        [MenuItem("ChronicSurvival/Create Sample Cards")]
        public static void CreateSampleCards()
        {
            string folderPath = "Assets/Data/Cards";
            
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets/Data", "Cards");
            }

            // Positive Cards
            CreateCard("Balanced Meal", "Eat a nutritious, balanced meal", CardRarity.Common, CardType.Lifestyle,
                new List<CardEffect> {
                    new CardEffect { componentName = "Energy", value = 10 },
                    new CardEffect { componentName = "BloodSugar", value = -5 },
                    new CardEffect { componentName = "Metabolism", value = 5 }
                });

            CreateCard("Cardio Exercise", "30 minutes of cardio workout", CardRarity.Common, CardType.Lifestyle,
                new List<CardEffect> {
                    new CardEffect { componentName = "HeartStability", value = 10 },
                    new CardEffect { componentName = "BloodPressure", value = -8 },
                    new CardEffect { componentName = "Metabolism", value = 8 },
                    new CardEffect { componentName = "Stress", value = -5 }
                });

            CreateCard("Deep Sleep", "Get 8 hours of quality sleep", CardRarity.Common, CardType.Lifestyle,
                new List<CardEffect> {
                    new CardEffect { componentName = "SleepQuality", value = 15 },
                    new CardEffect { componentName = "Energy", value = 15 },
                    new CardEffect { componentName = "Stress", value = -10 },
                    new CardEffect { componentName = "ImmuneStrength", value = 5 }
                });

            CreateCard("Hydration", "Drink plenty of water", CardRarity.Common, CardType.Lifestyle,
                new List<CardEffect> {
                    new CardEffect { componentName = "Hydration", value = 20 },
                    new CardEffect { componentName = "Toxicity", value = -5 },
                    new CardEffect { componentName = "Energy", value = 5 }
                });

            CreateCard("Meditation", "Practice mindfulness meditation", CardRarity.Uncommon, CardType.Lifestyle,
                new List<CardEffect> {
                    new CardEffect { componentName = "Stress", value = -15 },
                    new CardEffect { componentName = "BloodPressure", value = -10 },
                    new CardEffect { componentName = "SleepQuality", value = 8 },
                    new CardEffect { componentName = "HormoneBalance", value = 5 }
                });

            CreateCard("Vitamin Supplement", "Take daily vitamins", CardRarity.Uncommon, CardType.Medical,
                new List<CardEffect> {
                    new CardEffect { componentName = "ImmuneStrength", value = 12 },
                    new CardEffect { componentName = "Energy", value = 8 },
                    new CardEffect { componentName = "Inflammation", value = -5 }
                });

            // Negative Cards (for testing/events)
            CreateCard("Junk Food", "Eat fast food and sweets", CardRarity.Common, CardType.Debuff,
                new List<CardEffect> {
                    new CardEffect { componentName = "BloodSugar", value = 15 },
                    new CardEffect { componentName = "Inflammation", value = 8 },
                    new CardEffect { componentName = "Metabolism", value = -5 },
                    new CardEffect { componentName = "Energy", value = -5 }
                });

            CreateCard("Smoking", "Smoke cigarettes", CardRarity.Common, CardType.Debuff,
                new List<CardEffect> {
                    new CardEffect { componentName = "OxygenLevel", value = -15 },
                    new CardEffect { componentName = "Toxicity", value = 20 },
                    new CardEffect { componentName = "ImmuneStrength", value = -10 },
                    new CardEffect { componentName = "Inflammation", value = 10 }
                });

            CreateCard("All-Nighter", "Stay up all night", CardRarity.Common, CardType.Debuff,
                new List<CardEffect> {
                    new CardEffect { componentName = "SleepQuality", value = -20 },
                    new CardEffect { componentName = "Energy", value = -15 },
                    new CardEffect { componentName = "Stress", value = 10 },
                    new CardEffect { componentName = "ImmuneStrength", value = -8 }
                });

            // Rare/Epic Cards
            CreateCard("Medical Checkup", "Complete health screening", CardRarity.Rare, CardType.Medical,
                new List<CardEffect> {
                    new CardEffect { componentName = "ImmuneStrength", value = 15 },
                    new CardEffect { componentName = "Inflammation", value = -10 },
                    new CardEffect { componentName = "Stress", value = -8 }
                });

            CreateCard("Vacation", "Take a relaxing vacation", CardRarity.Epic, CardType.Lifestyle,
                new List<CardEffect> {
                    new CardEffect { componentName = "Stress", value = -25 },
                    new CardEffect { componentName = "SleepQuality", value = 20 },
                    new CardEffect { componentName = "Energy", value = 20 },
                    new CardEffect { componentName = "HormoneBalance", value = 15 }
                });

            CreateCard("Emergency Treatment", "Receive emergency medical care", CardRarity.Legendary, CardType.Emergency,
                new List<CardEffect> {
                    new CardEffect { componentName = "ImmuneStrength", value = 30 },
                    new CardEffect { componentName = "Inflammation", value = -20 },
                    new CardEffect { componentName = "Toxicity", value = -15 },
                    new CardEffect { componentName = "Energy", value = 15 }
                });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[CardAssetCreator] Created 12 sample cards in Assets/Data/Cards/");
        }

        private static void CreateCard(string name, string description, CardRarity rarity, CardType type, List<CardEffect> effects)
        {
            var card = ScriptableObject.CreateInstance<ActionCard>();
            card.cardName = name;
            card.description = description;
            card.rarity = rarity;
            card.cardType = type;
            card.effects = effects;
            card.isUnlocked = true;
            card.unlockAtDay = 1;

            string fileName = name.Replace(" ", "");
            AssetDatabase.CreateAsset(card, $"Assets/Data/Cards/{fileName}.asset");
        }
    }
}
